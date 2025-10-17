using Content.Goobstation.Maths.FixedPoint;
using Content.Server.Actions;
using Content.Server.Administration.Logs;
using Content.Server.Power.Components;
using Content.Server.Silicons.StationAi;
using Content.Server.Store.Systems;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxGoob.MALF.Events;
using Content.Shared._CorvaxGoob.MALF.Systems;
using Content.Shared.Store.Components;

namespace Content.Server._CorvaxGoob.Malf.Systems;

public sealed partial class MalfSystem : SharedMalfSystem
{
    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly IAdminLogManager _adminLogManager = default!;
    [Dependency] private readonly StationAiSystem _stationAi = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<MalfComponent, MalfAiOpenShopAction>(OnToggleShop);
        SubscribeLocalEvent<ApcComponent, OnHackedEvent>(OnAPCHacked);
    }

    private void OnStartup(Entity<MalfComponent> entity, ref ComponentStartup args)
    {
        _actions.AddAction(entity, entity.Comp.MalfToggleShopAction);

        // Add the starting amount of processing power to the store balance.
        AddCpu(entity, entity.Comp.BaseCpu);
    }

    private void OnToggleShop(Entity<MalfComponent> entity, ref MalfAiOpenShopAction args)
    {
        if (!TryComp<StoreComponent>(entity, out var store))
            return;

        _store.ToggleUi(entity, entity, store);
    }

    public void AddCpu(Entity<MalfComponent> entity, FixedPoint2 amount)
    {
        if (!TryComp<StoreComponent>(entity, out var store))
            return;

        if (!_store.TryAddCurrency(new Dictionary<string, FixedPoint2> { { entity.Comp.CpuCurrencyPrototype, amount } }, entity))
            return;

        _store.UpdateUserInterface(entity, entity, store);
    }

    public bool TryRemoveProcessingPower(Entity<MalfComponent> entity, FixedPoint2 amount)
    {
        // There is no method in the store class for
        // removing currency that I know of and I don't
        // want to touch store code so here this goes.

        if (!TryComp<StoreComponent>(entity, out var store))
            return false;

        if (!store.Balance.ContainsKey(entity.Comp.CpuCurrencyPrototype))
            return false;

        if (store.Balance[entity.Comp.CpuCurrencyPrototype] < amount)
            return false;

        store.Balance[entity.Comp.CpuCurrencyPrototype] -= amount;

        _store.UpdateUserInterface(entity, entity, store);

        return true;
    }

    private void OnAPCHacked(Entity<ApcComponent> ent, ref OnHackedEvent args)
    {
        var malfComp = args.HackerEntity.Comp;

        ent.Comp.NeedStateUpdate = true;

        AddCpu(args.HackerEntity, malfComp.HackApcReward);
    }

    public bool IsAIAliveAndOnStation(EntityUid entity, EntityUid targetStation)
    {
        return _stationAi.TryGetCore(entity, out var core)
               && _station.GetOwningStation(core) is { } station
               && station == targetStation;
    }
}
