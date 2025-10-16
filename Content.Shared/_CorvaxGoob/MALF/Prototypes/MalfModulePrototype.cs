using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxGoob.MALF.Prototypes;

[Prototype("malfModule")]
[DataDefinition]
public sealed partial class MalfModulePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField] public object? Event;
    [DataField] public List<EntProtoId>? ActionPrototypes;
}
