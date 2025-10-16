using System.Text;
using Content.Server._CorvaxGoob.GameTicking.Rules.Components;
using Content.Server._CorvaxGoob.Objectives.Components;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Server.Silicons.Laws;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxNext.Silicons.Borgs;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Roles;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Silicons.StationAi;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._CorvaxGoob.GameTicking.Rules;

/// <summary>
/// This handles...
/// </summary>
public sealed class MalfRuleSystem : GameRuleSystem<MalfRuleComponent>
{
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedRoleSystem _role = default!;
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly ObjectivesSystem _objective = default!;

    private readonly SoundSpecifier _briefingSound = new SoundPathSpecifier
        ("/Audio/_CorvaxGoob/Malf/Ambience/Antag/Malf/malf.ogg");

    private readonly ProtoId<NpcFactionPrototype> _malfFactionId = "MalfAI";
    private readonly ProtoId<NpcFactionPrototype> _nanotrasenFactionId = "NanoTrasen";
    private readonly ProtoId<CurrencyPrototype> _currency = "CPU";

    private static readonly EntProtoId MindRole = "MindRoleMalf";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfRuleComponent, AfterAntagEntitySelectedEvent>(OnAntagSelect);
        SubscribeLocalEvent<MalfRuleComponent, ObjectivesTextPrependEvent>(OnTextPrepend);
    }

    private void OnAntagSelect(Entity<MalfRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        TryMakeMalf(args.EntityUid, ent.Comp);
    }

    private void TryMakeMalf(EntityUid target, MalfRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return;

        if (!HasComp<StationAiHeldComponent>(target)) //really to be sure target is AI
            return;

        if (!HasComp<SiliconLawBoundComponent>(target))
            return;

        _role.MindAddRole(mindId, MindRole.Id, mind, true);

        if (HasComp<MetaDataComponent>(target))
        {
            var shortBriefing = Loc.GetString("malf-role-greeting-short");

            _antag.SendBriefing(target, Loc.GetString("malf-role-greeting-shit"), Color.Cyan, null);
            _antag.SendBriefing(target, Loc.GetString("malf-role-greeting"), Color.Lime, _briefingSound);

            if (_role.MindHasRole<HereticRoleComponent>(mindId, out var mr))
                AddComp(mr.Value, new RoleBriefingComponent { Briefing = shortBriefing }, overwrite: true);
        }

        _npcFaction.RemoveFaction(target, _nanotrasenFactionId, false);
        _npcFaction.AddFaction(target, _malfFactionId);

        EnsureComp<MalfComponent>(target);

        var store = EnsureComp<StoreComponent>(target);
        foreach (var category in rule.StoreCategories)
        {
            store.Categories.Add(category);
        }

        store.CurrencyWhitelist.Add(_currency);

        rule.Minds.Add(mindId);

        foreach(var objective in rule.Objectives)
        {
            _mind.TryAddObjective(mindId, mind,objective);
        }
    }

    private void OnTextPrepend(Entity<MalfRuleComponent> ent, ref ObjectivesTextPrependEvent args)
    {
        var sb = new StringBuilder();

        var borgsControlled = 0;
        var mostBorgsControlledName = string.Empty;

        foreach (var malf in EntityQuery<MalfComponent>())
        {
            if (!_mind.TryGetMind(malf.Owner, out var mindId, out var mind))
                continue;

            var name = _objective.GetTitle((mindId, mind), Name(malf.Owner));

            if (_mind.TryGetObjectiveComp<MalfHaveSyncedCyborgsConditionComponent>(mindId, out var cyborgs, mind))
            {
                if (cyborgs.BorgsControlled > borgsControlled)
                    borgsControlled = cyborgs.BorgsControlled;
                mostBorgsControlledName = name;
            }
        }

        sb.AppendLine("\n" + Loc.GetString("roundend-prepend-malf-controlled-borgs-named", ("name", mostBorgsControlledName), ("number", borgsControlled)));

        args.Text = sb.ToString();
    }
}
