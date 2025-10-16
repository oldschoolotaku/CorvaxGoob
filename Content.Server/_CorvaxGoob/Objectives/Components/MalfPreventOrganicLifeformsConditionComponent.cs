namespace Content.Server._CorvaxGoob.Objectives.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class MalfPreventOrganicLifeformsConditionComponent : Component
{
    [DataField]
    public int NotPrevented;

    /// <summary>
    /// Should only humans escape on evac shuttle or nobody should
    /// </summary>
    [DataField]
    public bool HumansOnly = false;
}
