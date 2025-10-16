using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.Silicons.StationAi;

namespace Content.Shared._CorvaxGoob.MALF.Systems;

public sealed class MalfHackableSystem : EntitySystem
{
    [Dependency] private readonly EntityManager _entityManager = default!;

    //TODO: Finish this
    public override void Initialize()
    {
        base.Initialize();
    }

    private void TryHack(Entity<MalfHackableComponent> entity)
    {
        if (!TryComp<MalfComponent>(entity, out var comp))
            return;

        if(!TryComp<StationAiCoreComponent>(entity, out var ai))
            return;

    }
}
