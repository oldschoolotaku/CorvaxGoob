using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Server._CorvaxGoob.GameTicking.Rules.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class MalfRuleComponent : Component
{
    public readonly List<EntityUid> Minds = new();

    public readonly ProtoId<StoreCategoryPrototype> Store = "MalfStore";

    public readonly List<ProtoId<EntityPrototype>> Objectives = new()
    {
        "MalfPreventShutdownObjective"
    };
}
