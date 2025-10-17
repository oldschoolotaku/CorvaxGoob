using Content.Server.Store.Systems;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxGoob.MALF.Events;
using Content.Shared.Store.Components;

namespace Content.Server._CorvaxGoob.Malf.Abilities;

// And then, he said - let there be shitcode
public sealed class MalfAbilitySystem : EntitySystem
{
    [Dependency] private readonly StoreSystem _store = default!;
    public override void Initialize()
    {
        base.Initialize();

        //SubscribeLocalEvent<MalfComponent, MalfAiOpenShopAction>(OnStore);
    }

    private void OnStore(Entity<MalfComponent> ent, ref MalfAiOpenShopAction args)
    {
        if (!TryComp<StoreComponent>(ent, out var store))
            return;

        _store.ToggleUi(ent, ent, store);
    }
}
