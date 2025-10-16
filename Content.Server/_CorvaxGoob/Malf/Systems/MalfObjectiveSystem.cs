using Content.Server._CorvaxGoob.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Objectives.Components;

namespace Content.Server._CorvaxGoob.Malf.Systems;

public sealed partial class MalfObjectiveSystem : EntitySystem
{
    [Dependency] private readonly NumberObjectiveSystem _number = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfHaveSyncedCyborgsConditionComponent, ObjectiveGetProgressEvent>(OnGetSyncedBorgProgress);
    }

    private void OnGetSyncedBorgProgress(Entity<MalfHaveSyncedCyborgsConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        var target = _number.GetTarget(ent);
        if (target != 0)
            args.Progress = MathF.Min(ent.Comp.BorgsControlled / target, 1f);
        else args.Progress = 1;
    }
}
