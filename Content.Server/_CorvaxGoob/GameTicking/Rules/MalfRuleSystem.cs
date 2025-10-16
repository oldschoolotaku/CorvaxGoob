using System.Text;
using Content.Server._CorvaxGoob.GameTicking.Rules.Components;
using Content.Server._CorvaxGoob.Objectives.Components;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Roles;
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
        ("/Audio/_CorvaxGoob/Malf/Ambience/Antag/Malf/malf_gain.ogg");

    private readonly ProtoId<NpcFactionPrototype> _malfFactionId = "MalfAI";
    private readonly ProtoId<NpcFactionPrototype> _nanotrasenFactionId = "NanoTrasen";
    private readonly ProtoId<StoreCategoryPrototype> _malfStoreId = "MalfStore";
    private readonly ProtoId<CurrencyPrototype> _currency = "CPU";

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfRuleComponent, AfterAntagEntitySelectedEvent>(OnAntagSelect);
    }

    private void OnAntagSelect(Entity<MalfRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        TryMakeMalf(args.EntityUid, ent.Comp);
    }

    private void TryMakeMalf(EntityUid target, MalfRuleComponent rule)
    {
        if (!_mind.TryGetMind(target, out var mindId, out var mind))
            return;

        if (!HasComp<StationAiCoreComponent>(target))
            return;

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
        store.Categories.Add(_malfStoreId);
        store.CurrencyWhitelist.Add(_currency);

        rule.Minds.Add(mindId);

        foreach(var objective in rule.Objectives)
        {
            _mind.TryAddObjective(mindId, mind,objective);
        }
    }

    public void OnTextPrepend(Entity<MalfRuleComponent> ent, ref ObjectivesTextPrependEvent args)
    {
        var sb = new StringBuilder();

        var borgsControlled = 0;
        var lifeFormsEscaped = 0;
        var mostBorgsControlledName = string.Empty;

        foreach (var malf in EntityQuery<MalfComponent>())
        {
            if (!_mind.TryGetMind(malf.Owner, out var mindId, out var mind))
                continue;

            var name = _objective.GetTitle((mindId, mind), Name(malf.Owner));

            if (_mind.TryGetObjectiveComp<MalfPreventOrganicLifeformsConditionComponent>(mindId,
                    out var organics, mind))
            {
                if (organics.NotPrevented > lifeFormsEscaped)
                    lifeFormsEscaped = organics.NotPrevented;
            }
            if (_mind.TryGetObjectiveComp<MalfHaveSyncedCyborgsConditionComponent>(mindId, out var cyborgs, mind))
            {
                if (cyborgs.BorgsControlled > borgsControlled)
                    borgsControlled = cyborgs.BorgsControlled;
                mostBorgsControlledName = name;
            }

            // Че мне в голову пришло это писать?
            var message = $"roundend-prepend-malf-{(lifeFormsEscaped == 0 ? "lifeforms-not-escaped"  : "lifeforms-escaped")}";

            var str = Loc.GetString(message, ("name", name));
            sb.AppendLine(str);
        }

        sb.AppendLine("\n" + Loc.GetString("roundend-prepend-malf-controlled-borgs-named", ("name", mostBorgsControlledName), ("number", borgsControlled)));

        args.Text = sb.ToString();
    }
}
