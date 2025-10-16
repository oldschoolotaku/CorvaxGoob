using Content.Goobstation.Maths.FixedPoint;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Prototypes;

namespace Content.Server._CorvaxGoob.Malf.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class MalfSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfComponent, ComponentInit>(OnCompInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
    }

    public void UpdateCpu(EntityUid uid, MalfComponent comp, int amount)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        var store2 = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> { { "CPU", amount } };
        store.Balance = store2;
    }

    private void OnCompInit(Entity<MalfComponent> ent, ref ComponentInit args)
    {

    }
}
