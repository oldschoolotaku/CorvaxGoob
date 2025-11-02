using Robust.Shared.GameStates;

namespace Content.Shared._White.AlertLevelLock.Components;

/// <summary>
/// Component that locks entities based on the current station alert level.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class StationAlertLevelLockComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    [DataField, AutoNetworkedField]
    public bool Locked = true;

    /// <summary>
    /// Set of alert levels that will cause this entity to be locked.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<string> LockedAlertLevels = [];

    /// <summary>
    /// Tracks the previous alert level. Needed for system to work fine with Corvax SOP
    /// </summary>
    /// TODO: Make it able to save previous code and check it properly
    [DataField, AutoNetworkedField]
    public string PreviousAlertLevel = "green";

    [DataField, AutoNetworkedField]
    public EntityUid? StationId;
}
