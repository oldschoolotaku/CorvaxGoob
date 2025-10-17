using Robust.Shared.Utility;

namespace Content.Shared._CorvaxGoob.MALF.Components;

[RegisterComponent]
public sealed partial class MalfHackableComponent : Component
{
    [DataField]
    public bool Hacked = false;

    public TimeSpan SecondsToHack = TimeSpan.FromSeconds(10);
}
