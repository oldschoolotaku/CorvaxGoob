using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared._CorvaxGoob.MALF.Components;


[RegisterComponent, NetworkedComponent]
public sealed partial class MalfActionComponent : Component
{
    /// <summary>
    /// How much ability can be used before becoming useless
    /// </summary>
    [DataField]
    public int UsesLeft = 3;

    /// <summary>
    /// Is ability infinite?
    /// </summary>
    [DataField]
    public bool Infinite = false;
}
