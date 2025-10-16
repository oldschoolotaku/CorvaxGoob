using Content.Shared._CorvaxGoob.MALF.Prototypes;
using Content.Shared.StatusIcon;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxGoob.MALF.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MalfComponent : Component
{
    [DataField]
    public int BaseCpu = 30;

    [DataField]
    public EntProtoId BaseAction = "ActionMalfOpenStore";

    [DataField]
    public List<ProtoId<MalfAbilityPrototype>> ModulesBought = new();

    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon = "MalfFaction";

    [DataField]
    public List<EntityUid> ProvidedActions = new();
}
