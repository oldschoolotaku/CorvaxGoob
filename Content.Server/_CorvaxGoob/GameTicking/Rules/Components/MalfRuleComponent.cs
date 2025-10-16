using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Server._CorvaxGoob.GameTicking.Rules.Components;

[RegisterComponent]
public sealed partial class MalfRuleComponent : Component
{
    public readonly List<EntityUid> Minds = new(); //somehow, I had three AIs at once

    public readonly List<ProtoId<StoreCategoryPrototype>> StoreCategories = new()
    {
        "MalfDestructive",
        "MalfUtility",
        "MalfUpgrade",
    };

    public readonly List<ProtoId<EntityPrototype>> Objectives = new()
    {
        "MalfPreventShutdownObjective"
    };
}
