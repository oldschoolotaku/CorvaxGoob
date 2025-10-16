using Content.Goobstation.Maths.FixedPoint;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Silicons.StationAi;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxGoob.MALF.Systems;

public sealed class MalfHackableSystem : EntitySystem
{
    [Dependency] private readonly EntityManager _entityManager = default!;

    //TODO: Finish this
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfHackableComponent, MalfHackingApcEvent>(TryHack);
    }

    private void TryHack(Entity<MalfHackableComponent> ent, ref MalfHackingApcEvent args)
    {
        if (!TryComp<MalfComponent>(ent, out var comp))
            return;

        TryHackApc(ent);
    }

    private void TryHackApc(Entity<MalfHackableComponent> entity)
    {
        if (!TryComp<StationAiWhitelistComponent>(entity, out _))
            return;

        entity.Comp.Hacked = true;
    }
}

public sealed partial class MalfHackingApcEvent : SimpleDoAfterEvent
{

}
