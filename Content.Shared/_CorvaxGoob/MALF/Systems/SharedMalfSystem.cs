using Content.Shared._CorvaxGoob.MALF.Components;
using Content.Shared._CorvaxGoob.MALF.Events;
using Content.Shared.DoAfter;
using Content.Shared.Verbs;
using Robust.Shared.Utility;

namespace Content.Shared._CorvaxGoob.MALF.Systems;

public abstract partial class SharedMalfSystem : EntitySystem
{
     [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<MalfHackableComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
        SubscribeLocalEvent<MalfComponent, HackDoAfterEvent>(OnHackDoAfterComplete);
    }

    private void OnGetVerbs(Entity<MalfHackableComponent> hackable, ref GetVerbsEvent<AlternativeVerb> args)
    {
        var malfEntity = args.User;

        if (!TryComp<MalfComponent>(malfEntity, out var malf))
            return;

        if (hackable.Comp.Hacked)
            return;

        var verb = new AlternativeVerb
        {
            Priority = 1,
            Act = () => StartHackDoAfter(hackable, new Entity<MalfComponent>(malfEntity, malf)),
            Text = Loc.GetString("malf-ai-hack-verb-text"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/unlock.svg.192dpi.png"))
        };

        args.Verbs.Add(verb);
    }

    private void StartHackDoAfter(Entity<MalfHackableComponent> hackable, Entity<MalfComponent> malfEntity)
    {
        if (_doAfter.IsRunning(malfEntity.Comp.HackDoAfterId))
            _doAfter.Cancel(malfEntity.Comp.HackDoAfterId);

        EnsureComp<DoAfterComponent>(hackable);

        var doAfterArgs = new DoAfterArgs(EntityManager, hackable, hackable.Comp.SecondsToHack, new HackDoAfterEvent(), malfEntity, hackable, showTo: malfEntity);

        if (!_doAfter.TryStartDoAfter(doAfterArgs, out var id))
            return;

        malfEntity.Comp.HackDoAfterId = id;
    }

    private void OnHackDoAfterComplete(Entity<MalfComponent> ent, ref HackDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<MalfHackableComponent>(args.Target, out var hackable))
            return;

        hackable.Hacked = true;

        var ev = new OnHackedEvent(ent);

        RaiseLocalEvent(args.Target.Value, ref ev);
    }
}
