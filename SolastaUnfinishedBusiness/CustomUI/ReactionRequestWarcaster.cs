using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static SolastaUnfinishedBusiness.Feats.OtherFeats;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class ReactionRequestWarcaster : ReactionRequest
{
    internal const string Name = "ReactionWarcaster";
    private readonly ActionDefinition _attackAction;
    private readonly List<ActionModifier> _attackModifiers = [];
    private readonly GuiCharacter _guiTarget;
    private readonly string _type;

    internal ReactionRequestWarcaster(string name, CharacterActionParams reactionParams)
        : base(name, reactionParams)
    {
        _attackAction = reactionParams.ActionDefinition;
        _attackModifiers.SetRange(reactionParams.ActionModifiers);
        BuildSuboptions();
        _type = string.IsNullOrEmpty(ReactionParams.StringParameter2)
            ? name
            : ReactionParams.StringParameter2;
        _guiTarget = new GuiCharacter(reactionParams.targetCharacters[0]);
    }

    internal ReactionRequestWarcaster(CharacterActionParams reactionParams)
        : this(Name, reactionParams)
    {
    }

    public override int SelectedSubOption
    {
        get
        {
            var spell = (ReactionParams.RulesetEffect as RulesetEffectSpell)?.SpellDefinition;

            if (spell == null)
            {
                return 0;
            }

            return ReactionParams.SpellRepertoire.KnownSpells.FindIndex(s => s == spell) + 1;
        }
    }


    public override string SuboptionTag => _type;

    public override bool IsStillValid
    {
        get
        {
            var targetCharacter = ReactionParams.TargetCharacters[0];

            return ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters
                .Contains(targetCharacter) && targetCharacter.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false };
        }
    }

    private void BuildSuboptions()
    {
        SubOptionsAvailability.Clear();
        SubOptionsAvailability.Add(0, !reactionParams.BoolParameter4);

        var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
        var actingCharacter = reactionParams.ActingCharacter;
        var cantrips = GetValidCantrips(battleManager, actingCharacter, reactionParams.targetCharacters[0]);

        if (cantrips != null && cantrips.Count != 0)
        {
            reactionParams.SpellRepertoire = new RulesetSpellRepertoire();

            var i = 1;

            foreach (var c in cantrips)
            {
                reactionParams.SpellRepertoire.KnownSpells.Add(c);
                SubOptionsAvailability.Add(i, true);
                i++;
            }
        }

        foreach (var pair in SubOptionsAvailability.Where(pair => pair.Value))
        {
            SelectSubOption(pair.Key);
            break;
        }
    }

    internal static List<SpellDefinition> GetValidCantrips(
        GameLocationBattleManager battle,
        GameLocationCharacter character,
        GameLocationCharacter target)
    {
        if (battle is not { IsBattleInProgress: true })
        {
            return null;
        }

        var rulesetCharacter = character.RulesetCharacter;

        // should not trigger if a wildshape form
        if (rulesetCharacter is not RulesetCharacterHero)
        {
            return null;
        }

        // use marker to identify WarCaster
        if (!rulesetCharacter.HasSubFeatureOfType<WarCasterMarker>())
        {
            return null;
        }

        var cantrips = new List<SpellDefinition>();

        rulesetCharacter.EnumerateReadyAttackCantrips(cantrips);

        cantrips.RemoveAll(cantrip =>
        {
            if (cantrip.ActivationTime != RuleDefinitions.ActivationTime.Action &&
                cantrip.ActivationTime != RuleDefinitions.ActivationTime.BonusAction)
            {
                return true;
            }

            if (cantrip.EffectDescription.TargetType != RuleDefinitions.TargetType.Individuals &&
                cantrip.EffectDescription.TargetType != RuleDefinitions.TargetType.IndividualsUnique)
            {
                return true;
            }

            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var actionModifier = new ActionModifier();

            attackParams.FillForMagic(character,
                character.LocationPosition,
                cantrip.EffectDescription,
                cantrip.Name,
                target,
                target.LocationPosition,
                actionModifier);

            return !battle.IsValidAttackForReadiedAction(attackParams, false, RuleDefinitions.CoverType.ThreeQuarter);
        });

        return cantrips;
    }


    public override void SelectSubOption(int option)
    {
        ReactionParams.RulesetEffect?.Terminate(false);

        var targetCharacters = reactionParams.TargetCharacters;

        while (targetCharacters.Count > 1)
        {
            reactionParams.TargetCharacters.RemoveAt(targetCharacters.Count - 1);
            reactionParams.ActionModifiers.RemoveAt(reactionParams.ActionModifiers.Count - 1);
        }

        var actingCharacter = reactionParams.ActingCharacter;

        if (option == 0)
        {
            reactionParams.ActionDefinition = _attackAction;
            reactionParams.ActionModifiers.SetRange(_attackModifiers);
            reactionParams.RulesetEffect = null;
        }
        else
        {
            reactionParams.ActionDefinition = ServiceRepository.GetService<IGameLocationActionService>()
                .AllActionDefinitions[ActionDefinitions.Id.CastReaction];

            var spell = reactionParams.SpellRepertoire.KnownSpells[option - 1];
            var rulesService = ServiceRepository.GetService<IRulesetImplementationService>();
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (!rulesetCharacter.CanCastCantrip(spell, out var spellRepertoire))
            {
                return;
            }

            var spellEffect = rulesService.InstantiateEffectSpell(rulesetCharacter, spellRepertoire,
                spell, spell.SpellLevel, false);

            ReactionParams.RulesetEffect = spellEffect;

            var spellTargets = spellEffect.ComputeTargetParameter();

            if (!reactionParams.RulesetEffect.EffectDescription.IsSingleTarget || spellTargets <= 0)
            {
                return;
            }

            var target = reactionParams.TargetCharacters.FirstOrDefault();
            var mod = reactionParams.ActionModifiers.FirstOrDefault();

            while (target != null && mod != null && reactionParams.TargetCharacters.Count < spellTargets)
            {
                reactionParams.TargetCharacters.Add(target);
                // Technically casts after first might need to have different mods, but not by much since we attacking same target.
                reactionParams.ActionModifiers.Add(mod.Clone());
            }
        }
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&{_type}Title");
    }

    public override string FormatDescription()
    {
        return Gui.Format($"Reaction/&{_type}Description", _guiTarget.Name);
    }

    public override string FormatReactTitle()
    {
        return Gui.Localize($"Reaction/&{_type}ReactTitle");
    }

    public override string FormatReactDescription()
    {
        return Gui.Localize($"Reaction/&{_type}ReactDescription");
    }

    public override void OnSetInvalid()
    {
        base.OnSetInvalid();
        ReactionParams.RulesetEffect?.Terminate(false);
    }
}
