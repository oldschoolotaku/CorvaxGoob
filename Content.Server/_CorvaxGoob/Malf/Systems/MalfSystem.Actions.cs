using Content.Server.Doors.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Station.Systems;
using Content.Server.SurveillanceCamera;
using Content.Server.Wires;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxGoob.MALF.Events;
using Content.Shared._CorvaxGoob.MALF.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Electrocution;
using Content.Shared.Explosion.Components;
using Content.Shared.Explosion.Components.OnTrigger;
using Content.Shared.StationAi;
using Robust.Server.GameObjects;

namespace Content.Server._CorvaxGoob.Malf.Systems;


public sealed partial class MalfSystem : SharedMalfSystem
{
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;
    [Dependency] private readonly TransformSystem _xform = default!;
    [Dependency] private readonly DoorSystem _door = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedEyeSystem _eye = default!;
    [Dependency] private readonly WiresSystem _wires = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public void InitializeActions()
    {
        SubscribeLocalEvent<MalfComponent, MachineOverloadActionEvent>(OnOverloadAction);
        SubscribeLocalEvent<MalfComponent, HostileLockdownActionEvent>(OnLockdownEvent);

        SubscribeLocalEvent<MalfComponent, UpgradeCamerasActionEvent>(OnCameraUpgrade);
        //SubscribeLocalEvent<MalfComponent, ReactivateCameraActionEvent>(OnReactivateCamera);

        //SubscribeLocalEvent<MalfComponent, DoomsDayActionEvent>(OnDoomsDayStart);
    }

    /*private void OnDoomsDayStart(Entity<MalfComponent> ent, ref DoomsDayActionEvent args)
    {
        var gamerule = EntityQuery<MalfRuleComponent>().First();

        // The AI must be on-station in order to activate the device.
        if (!IsAIAliveAndOnStation(ent, gamerule.Station))
        {
            _popup.PopupEntity(Loc.GetString(gamerule.OnlyOnStationLoc), ent, ent, Content.Shared.Popups.PopupType.LargeCaution);
            return;
        }

        // There can only be one dooms day device active at a time.
        if (gamerule.DoomDeviceActive)
        {
            _popup.PopupEntity(Loc.GetString(gamerule.OnlyOneDeviceLoc), ent, ent, Content.Shared.Popups.PopupType.LargeCaution);
            return;
        }

        StartDoomsDayDevice(ent);

        args.Handled = true;
    }

    private void OnReactivateCamera(Entity<MalfComponent> ent, ref ReactivateCameraActionEvent args)
    {
        if (!_stationAi.TryGetCore(ent, out var core) || core.Comp?.RemoteEntity == null)
            return;

        var query = _lookup.GetEntitiesInRange<SurveillanceCameraComponent>(Transform(core.Comp.RemoteEntity.Value).Coordinates, ent.Comp.CameraRepairRadius, LookupFlags.Static);

        foreach (var cam in query)
        {
            if (ReactivateCamera(cam))
            {
                args.Handled = true;
                return;
            }
        }
    }*/

    public bool ReactivateCamera(Entity<SurveillanceCameraComponent> entity)
    {
        var camRepaired = false;

        if (!TryComp<WiresComponent>(entity, out var wiresComp))
            return false;

        var wires = _wires.TryGetWires<BaseWireAction>(entity, wiresComp);

        foreach (var wire in wires)
        {
            if (!wire.IsCut || wire.Action == null || !wire.Action.Mend(entity, wire))
                continue;

            wire.IsCut = false;

            camRepaired = true;
        }

        return camRepaired;
    }

    private void OnCameraUpgrade(Entity<MalfComponent> ent, ref UpgradeCamerasActionEvent args)
    {
        if (!_stationAi.TryGetCore(ent, out var core))
            return;

        args.Handled = true;

        var query = EntityQueryEnumerator<StationAiVisionComponent, TransformComponent>();

        var gridUid = _xform.GetGrid(Transform(core).Coordinates);

        while (query.MoveNext(out var uid, out var comp, out var xform))
        {
            // If its not on the same station as the AI then skip it.
            if (_xform.GetGrid(xform.Coordinates) != gridUid)
                continue;

            //_stationAi.SetVisionOcclusion((uid, comp), false);
        }

        if (!TryComp<EyeComponent>(ent, out var eye))
            return;

        // Disable light drawing
        // to give night vision.
        _eye.SetDrawLight((ent.Owner, eye), false);
    }

    private void OnLockdownEvent(Entity<MalfComponent> ent, ref HostileLockdownActionEvent args)
    {
        var aiGrid = _xform.GetGrid(Transform(ent).Coordinates);

        LockdownStation(aiGrid);

        args.Handled = true;
    }

    public void LockdownStation(EntityUid? gridUid)
    {
        var airlockQuery = EntityQueryEnumerator<DoorBoltComponent, DoorComponent, TransformComponent>();

        while (airlockQuery.MoveNext(out var airlockUid, out var bolt, out var door, out var xform))
        {
            if (_xform.GetGrid(xform.Coordinates) != gridUid)
                continue;

            if (door.State == DoorState.Open)
            {
                _door.StartClosing(airlockUid, door);
                _door.Crush(airlockUid, door);
            }
            else
                _door.SetBoltsDown((airlockUid, bolt), true);

            if (!TryComp<ElectrifiedComponent>(airlockUid, out var electrified))
                continue;

            electrified.Enabled = true;
        }
    }

    private void OnOverloadAction(Entity<MalfComponent> ent, ref MachineOverloadActionEvent args)
    {
        if (HasComp<ActiveTimerTriggerComponent>(args.Target))
            return;

        if (!TryComp<ApcPowerReceiverComponent>(args.Target, out var machine))
            return;

        if (!machine.Powered)
            return;

        // Explosive hack begin.

        // Basically you *can* add an explosive comp to an entity BUT
        // you can't change any of the variables outside of the ExplosiveSystem SO
        // I'm storing a "default" explosive component on the action entity itself and
        // just copying that over.

        if (!TryComp<ExplosiveComponent>(args.Action, out var explosive))
            return;

        CopyComp(args.Action, args.Target, explosive);

        // Explosive hack end.

        EnsureComp<ExplosiveComponent>(args.Target);
        EnsureComp<ExplodeOnTriggerComponent>(args.Target);

        _trigger.HandleTimerTrigger(args.Target, ent, ent.Comp.SecondsToOverload, 1.0f, 0.0f, ent.Comp.BuzzingSound);

        args.Handled = true;
    }
}
