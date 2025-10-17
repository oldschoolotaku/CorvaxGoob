using Content.Server.Store.Systems;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.Store.Components;

namespace Content.Server._CorvaxGoob.Malf.Abilities;

// And then, he said - let there be shitcode
public sealed class MalfAbilitySystem : EntitySystem
{
    [Dependency] private readonly StoreSystem _store = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfComponent, EventMalfOpenStore>(OnStore);
    }

    private void OnStore(Entity<MalfComponent> ent, ref EventMalfOpenStore args)
    {
        if (!TryComp<StoreComponent>(ent, out var store))
            return;

        _store.ToggleUi(ent, ent, store);
    }
}
