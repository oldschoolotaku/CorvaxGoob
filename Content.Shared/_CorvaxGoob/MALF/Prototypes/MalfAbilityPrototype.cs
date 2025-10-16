using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxGoob.MALF.Prototypes;

[Prototype("malfAbility")]
[DataDefinition]
public sealed partial class MalfAbilityPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField] public object? Event;
    [DataField] public EntProtoId? ActionPrototype;
}
