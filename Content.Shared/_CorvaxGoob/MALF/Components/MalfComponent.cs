using Content.Goobstation.Maths.FixedPoint;
using Content.Shared._CorvaxGoob.MALF.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.StatusIcon;
using Content.Shared.Store;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._CorvaxGoob.MALF.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class MalfComponent : Component
{
    [DataField(customTypeSerializer: typeof(PrototypeIdSerializer<CurrencyPrototype>))]
    public string CpuCurrencyPrototype = "Cpu";

    [DataField]
    public FixedPoint2 BaseCpu = 30;

    [DataField]
    public FixedPoint2 HackApcReward = 10;

    [DataField]
    public float SecondsToOverload = 5.0f;

    [DataField]
    public List<ProtoId<MalfModulePrototype>> BaseModules = new()
    {
        "MalfStore"
    };

    [DataField, ValidatePrototypeId<EntityPrototype>]
    public string MalfToggleShopAction = "ActionMalfOpenStore";

    [DataField]
    public List<EntityUid> ProvidedActions = new();

    public DoAfterId? HackDoAfterId;

    [DataField]
    public SoundCollectionSpecifier BuzzingSound = new("sparks", AudioParams.Default);
}
