using Robust.Shared.Utility;

namespace Content.Shared._CorvaxGoob.MALF.Components;

[RegisterComponent]
public sealed partial class MalfHackableComponent : Component
{
    /// <summary>
    /// Amount of CPU MALF will get for hacking
    /// </summary>
    [DataField]
    public int Cpu = 10;

    [DataField]
    public bool Hacked = false;

    /// <summary>
    /// Used for changing APC Sprite
    /// </summary>
    [DataField]
    public SpriteSpecifier HackedOverlay;
}
