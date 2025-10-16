namespace Content.Server._CorvaxGoob.Objectives.Components;

[RegisterComponent]
public sealed partial class MalfPreventShutdownConditionComponent : Component
{
    [DataField]
    public bool Deactivated = false;
}
