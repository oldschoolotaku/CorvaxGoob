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
    public EntProtoId BaseAction = "ActionHereticOpenStore";

    [DataField]
    public List<ProtoId<MalfAbilityPrototype>> ModulesBought = new();

    [DataField]
    public ProtoId<FactionIconPrototype> StatusIcon = "MalfFaction";

    [DataField]
    public List<EntityUid> ProvidedActions = new();

    [ViewVariables(VVAccess.ReadOnly)]
    public bool NoLifeformsOnEvac = true;

    [ViewVariables(VVAccess.ReadOnly)]
    public bool NoNotHumansOnEvac = true;
}
