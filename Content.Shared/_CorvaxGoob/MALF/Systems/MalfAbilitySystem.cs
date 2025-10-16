using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxGoob.MALF.Prototypes;
using Content.Shared.Actions;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Shared._CorvaxGoob.MALF.Systems;

public sealed class MalfAbilitySystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public MalfModulePrototype GetAbility(ProtoId<MalfModulePrototype> id)
        => _proto.Index(id);

    public void AddAbility(EntityUid uid, MalfComponent comp, ProtoId<MalfModulePrototype> id)
    {
        var data = GetAbility(id);

        if (data.Event != null)
            RaiseLocalEvent(uid, data.Event, true);

        if (data.ActionPrototypes != null && data.ActionPrototypes.Count > 0)
        {
            foreach (var act in data.ActionPrototypes)
            {
                if (_action.AddAction(uid, act) is {} action)
                    comp.ProvidedActions.Add(action);
                else
                    Log.Error($"Failed to give malf {ToPrettyString(uid)} action {act}!");
            }
        }

        Dirty(uid, comp);
    }

    public void RemoveAbility(EntityUid uid, MalfComponent comp, ProtoId<MalfModulePrototype> id)
    {
        var data = GetAbility(id);

        if (data.ActionPrototypes != null && data.ActionPrototypes.Count > 0)
        {
            foreach (var act in data.ActionPrototypes)
            {
                comp.ProvidedActions.RemoveAll(action =>
                {
                    if (Prototype(action)?.ID is not {} id || id != act)
                        return false;

                    _action.RemoveAction(action);
                    return true;
                });
            }
        }
    }
}
