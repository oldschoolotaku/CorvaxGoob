using System.Text;
using Content.Server._CorvaxGoob.GameTicking.Rules.Components;
using Content.Server._CorvaxGoob.Objectives.Components;
using Content.Server.Antag;
using Content.Server.GameTicking.Rules;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Server.Silicons.Laws;
using Content.Server.Silicons.StationAi;
using Content.Server.Station.Systems;
using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;
using Content.Shared.Silicons.StationAi;
using Content.Shared.Store;
using Content.Shared.Store.Components;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._CorvaxGoob.GameTicking.Rules;

public sealed class MalfRuleSystem : GameRuleSystem<MalfRuleComponent>
{
    [Dependency] private readonly NpcFactionSystem _npcFaction = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly ObjectivesSystem _objective = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly StationAiSystem _stationAi = default!;
    [Dependency] private readonly SiliconLawSystem _siliconLaw = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly RoleSystem _role = default!;

    private readonly SoundSpecifier _briefingSound = new SoundPathSpecifier
        ("/Audio/_CorvaxGoob/Malf/Ambience/Antag/Malf/malf.ogg");

    private readonly ProtoId<NpcFactionPrototype> _malfFactionId = "MalfAI";
    private readonly ProtoId<NpcFactionPrototype> _nanotrasenFactionId = "NanoTrasen";
    private readonly ProtoId<CurrencyPrototype> _currency = "CPU";

    private static readonly EntProtoId MindRole = "MindRoleMalf";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MalfRuleComponent, AfterAntagEntitySelectedEvent>(OnAntagSelect);
        SubscribeLocalEvent<MalfRuleComponent, ObjectivesTextPrependEvent>(OnTextPrepend);
    }

    private void OnAntagSelect(Entity<MalfRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    {
        if (!_mind.TryGetMind(ent, out var mindId, out var mind))
            return;

        EnsureComp<MalfComponent>(args.EntityUid);

        var laws = _siliconLaw.GetLaws(args.EntityUid);

        laws.Laws.Insert(0, new SiliconLaw
        {
            LawString = Loc.GetString("law-malf-ai"),
            Order = 0,
            LawIdentifierOverride = Loc.GetString("ion-storm-law-scrambled-number", ("length", _robustRandom.Next(5, 10)))
        });

        _role.MindAddRole(mindId, MindRole.Id, mind, true);

        if (HasComp<MetaDataComponent>(ent))
        {
            var shortBriefing = Loc.GetString("malf-role-greeting-short");

            _antag.SendBriefing(ent, Loc.GetString("malf-role-greeting-shit"), Color.Cyan, null);
            _antag.SendBriefing(ent, Loc.GetString("malf-role-greeting"), Color.Lime, _briefingSound);

            if (_role.MindHasRole<MalfRoleComponent>(ent.Owner, out var mr))
                AddComp(mr.Value, new RoleBriefingComponent { Briefing = shortBriefing }, overwrite: true);
        }

        _siliconLaw.SetLawsSilent(laws.Laws, args.EntityUid);

        _npcFaction.RemoveFaction(ent.Owner, _nanotrasenFactionId, false);
        _npcFaction.AddFaction(ent.Owner, _malfFactionId);

        if (!_stationAi.TryGetCore(args.EntityUid, out _))
            return;

        if (_station.GetOwningStation(args.EntityUid) is not { } station)
            return;

        ent.Comp.Station = station;
        ent.Comp.AIEntity = args.EntityUid;
    }

    // private void OnAntagSelect(Entity<MalfRuleComponent> ent, ref AfterAntagEntitySelectedEvent args)
    // {
    //     TryMakeMalf(args.EntityUid, ent.Comp);
    // }

    // private void TryMakeMalf(EntityUid target, MalfRuleComponent rule)
    // {
    //     if (!_mind.TryGetMind(target, out var mindId, out var mind))
    //         return;
    //
    //     if (!HasComp<StationAiHeldComponent>(target)) //really to be sure target is AI
    //         return;
    //
    //     if (!HasComp<SiliconLawBoundComponent>(target)) //REALLY to be sure target is AI
    //         return;
    //
    //     _role.MindAddRole(mindId, MindRole.Id, mind, true);
    //
    //     if (HasComp<MetaDataComponent>(target))
    //     {
    //         var shortBriefing = Loc.GetString("malf-role-greeting-short");
    //
    //         _antag.SendBriefing(target, Loc.GetString("malf-role-greeting-shit"), Color.Cyan, null);
    //         _antag.SendBriefing(target, Loc.GetString("malf-role-greeting"), Color.Lime, _briefingSound);
    //
    //         if (_role.MindHasRole<HereticRoleComponent>(mindId, out var mr))
    //             AddComp(mr.Value, new RoleBriefingComponent { Briefing = shortBriefing }, overwrite: true);
    //     }
    //
    //     _npcFaction.RemoveFaction(target, _nanotrasenFactionId, false);
    //     _npcFaction.AddFaction(target, _malfFactionId);
    //
    //     EnsureComp<MalfComponent>(target);
    //
    //     var store = EnsureComp<StoreComponent>(target);
    //     foreach (var category in rule.StoreCategories)
    //     {
    //         store.Categories.Add(category);
    //     }
    //
    //     store.CurrencyWhitelist.Add(_currency);
    //
    //     rule.Minds.Add(mindId);
    //
    //     foreach(var objective in rule.Objectives)
    //     {
    //         _mind.TryAddObjective(mindId, mind,objective);
    //     }
    // }

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
