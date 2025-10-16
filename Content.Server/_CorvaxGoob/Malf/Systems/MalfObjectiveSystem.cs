using Content.Server._CorvaxGoob.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Content.Shared.Mind;
using Content.Shared.Mobs.Systems;
using Content.Shared.Objectives.Components;
using Content.Shared.Roles;
using Robust.Shared.Player;

namespace Content.Server._CorvaxGoob.Malf.Systems;

public sealed partial class MalfObjectiveSystem : EntitySystem
{
    //[Dependency] private readonly NumberObjectiveSystem _number = default!;
    [Dependency] private readonly EmergencyShuttleSystem _emergencyShuttle = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfHaveSyncedCyborgsConditionComponent, ObjectiveGetProgressEvent>(OnGetSyncedBorgProgress);
        SubscribeLocalEvent<MalfPreventOrganicLifeformsConditionComponent, ObjectiveGetProgressEvent>(OnGetPreventLifeformsProgress);
    }

    private void OnGetSyncedBorgProgress(Entity<MalfHaveSyncedCyborgsConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        // var target = _number.GetTarget(ent);
        // if (target != 0)
        //     args.Progress = MathF.Min(ent.Comp.Researched / target, 1f);
        // else args.Progress = 1f;
    }
    private void OnGetPreventLifeformsProgress(Entity<MalfPreventOrganicLifeformsConditionComponent> ent, ref ObjectiveGetProgressEvent args)
    {
        args.Progress = GetPreventLifeformsProgress(args.MindId, args.Mind);
    }

    private float? GetPreventLifeformsProgress(EntityUid mindId, MindComponent mind)
    {
        // Not escaping alive if you're deleted/dead
        if (mind.OwnedEntity == null || _mind.IsCharacterDeadIc(mind))
            return 0f;

        // There no emergency shuttles
        if (!_emergencyShuttle.EmergencyShuttleArrived)
            return 0f;

        // Check hijack for each emergency shuttle
        foreach (var stationData in EntityQuery<StationEmergencyShuttleComponent>())
        {
            if (stationData.EmergencyShuttle == null)
                continue;

            if (IsShuttleHijacked(stationData.EmergencyShuttle.Value))
                return 1f;
        }

        return 0f;
    }

    private bool IsShuttleHijacked(EntityUid shuttleGridId)
    {
        var gridPlayers = Filter.BroadcastGrid(shuttleGridId).Recipients;
        var humanoids = GetEntityQuery<HumanoidAppearanceComponent>();

        var hijacked = false;
        foreach (var player in gridPlayers)
        {
            if (player.AttachedEntity == null ||
                !_mind.TryGetMind(player.AttachedEntity.Value, out var crewMindId, out _))
                continue;

            var isHumanoid = humanoids.HasComponent(player.AttachedEntity.Value);
            if (!isHumanoid) // Only humanoids count as enemies
                continue;

            var isPersonIncapacitated = _mobState.IsIncapacitated(player.AttachedEntity.Value);
            if (isPersonIncapacitated) // Allow dead and crit
                continue;

            return false;
        }

        return hijacked;
    }
}
