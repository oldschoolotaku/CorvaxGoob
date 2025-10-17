﻿using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._CorvaxGoob.MALF.Events;

[Serializable, NetSerializable]
public sealed partial class HackDoAfterEvent : SimpleDoAfterEvent;

[ByRefEvent]
public readonly struct OnHackedEvent(Entity<MalfComponent> hacker)
{
    public readonly Entity<MalfComponent> HackerEntity = hacker;
};

public sealed partial class MalfAiOpenShopAction : InstantActionEvent;
public sealed partial class ReactivateCameraActionEvent : InstantActionEvent;
public sealed partial class UpgradeCamerasActionEvent : InstantActionEvent;
public sealed partial class MachineOverloadActionEvent : EntityTargetActionEvent;
public sealed partial class HostileLockdownActionEvent : InstantActionEvent;
public sealed partial class DoomsDayActionEvent : InstantActionEvent;
public sealed partial class TurretUpgradeActionEvent : InstantActionEvent;
public sealed partial class DestoryRCDsActionEvent : InstantActionEvent;
public sealed partial class BlackoutActionEvent : InstantActionEvent;
public sealed partial class MakeRoboticFactoryActionEvent : EntityTargetActionEvent;
