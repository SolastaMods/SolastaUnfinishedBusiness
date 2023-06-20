using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal class PatronEldritchSurge : AbstractSubclass
{
    private const string Name = "PatronEldritchSurge";

    private const string PowerBlastPursuitName = $"Power{Name}BlastPursuit";

    // LEVEL 06 Blast Pursuit

    private static readonly FeatureDefinitionAdditionalAction AdditionalActionBlastPursuit =
        FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BlastPursuit")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(AllowDuplicates.Mark)
            .SetActionType(ActionType.Main)
            .SetRestrictedActions(
                Id.AttackMain,
                Id.CastMain,
                Id.DashMain,
                Id.DisengageMain,
                Id.Dodge)
            .SetMaxAttacksNumber(1)
            .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .AllowMultipleInstances()
        .SetFeatures(AdditionalActionBlastPursuit)
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionRaging)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new ExtraActionBlastPursuit(),
                    new TargetReducedToZeroHpBlastPursuit())
                .AddToDB())
        .AddToDB();

    // LEVEL 10 Blast Overload

    private static readonly ConditionDefinition ConditionBlastOverload = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastOverload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
        .AddFeatures(
            FeatureDefinitionAdditionalDamageBuilder
                .Create(AdditionalDamageInvocationAgonizingBlast, $"AdditionalDamage{Name}BlastOverload")
                .SetGuiPresentationNoContent(true)
                .SetNotificationTag("BlastOverload")
                .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
                .AddToDB(),
            FeatureDefinitionMagicAffinityBuilder
                .Create($"MagicAffinity{Name}BlastOverload")
                .SetGuiPresentationNoContent(true)
                .SetCastingModifiers(0, SpellParamsModifierType.None, 0, SpellParamsModifierType.FlatValue, true)
                .AddToDB())
        .AddToDB();

    internal PatronEldritchSurge()
    {
        // LEVEL 03

        // Blast Exclusive

        EldritchBlast.SetCustomSubFeatures(new ModifyMagicEffectEldritchBlast());

        var bonusCantripsEldritchSurgeBlastExclusive = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}BlastExclusive")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(EldritchBlast)
            .AddToDB();

        // LEVEL 06

        // Blast Pursuit

        var powerBlastPursuit = FeatureDefinitionPowerBuilder
            .Create(PowerBlastPursuitName)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetGuiPresentation(Category.Feature, EldritchBlast)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastPursuit))
                .Build())
            .AddToDB();

        powerBlastPursuit.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastPursuit, ConditionBlastPursuit));

        // LEVEL 10

        // Blast Overload

        var powerBlastOverload = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BlastOverload")
            .SetGuiPresentation(Category.Feature, EldritchBlast)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(FeatureDefinitionPowers.PowerBarbarianRageStart)
                .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionBlastOverload))
                .Build())
            .AddToDB();

        powerBlastOverload.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastOverload, ConditionBlastOverload));

        // LEVEL 14

        // Blast Reload

        var featureBlastReload = FeatureDefinitionBuilder
            .Create($"Feature{Name}BlastReload")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomBehaviorBlastReload())
            .AddToDB();

        // LEVEL 18

        // Permanent Blast Pursuit

        var featureSetPermanentBlastPursuit = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}PermanentBlastPursuit")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(AdditionalActionBlastPursuit)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1, bonusCantripsEldritchSurgeBlastExclusive)
            .AddFeaturesAtLevel(6, powerBlastPursuit)
            .AddFeaturesAtLevel(10, powerBlastOverload)
            .AddFeaturesAtLevel(14, featureBlastReload)
            .AddFeaturesAtLevel(18, featureSetPermanentBlastPursuit)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static int GetBlastPursuitExtraActionCount(RulesetActor rulesetActor, int additionalCount = 0)
    {
        return additionalCount +
               rulesetActor.ConditionsByCategory
                   .SelectMany(x => x.Value)
                   .Count(x => x.conditionDefinition == ConditionExtraActionBlastPursuit);
    }

    private sealed class ModifyMagicEffectEldritchBlast : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter rulesetCharacter,
            RulesetEffect rulesetEffect)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) == 0)
            {
                return effectDescription;
            }

            var totalLevel = rulesetHero.classesHistory.Count;
            var warlockClassLevel = rulesetHero.GetClassLevel(CharacterClassDefinitions.Warlock);
            var determinantLevel = warlockClassLevel - (2 * (totalLevel - warlockClassLevel));
            var increaseLevels = new[] { 3, 8, 13, 18 };
            var additionalBeamCount = increaseLevels.Count(level => determinantLevel >= level);
            var blastPursuitExtraActionCount = GetBlastPursuitExtraActionCount(rulesetHero);
            var overloadStatus = rulesetHero.HasConditionOfType(ConditionBlastOverload) ? 1 : 0;

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;
            effectDescription.rangeParameter = Math.Max(1, 24 - (6 * (blastPursuitExtraActionCount + overloadStatus)));

            return effectDescription;
        }
    }

    private sealed class CustomBehaviorBlastPursuitOrOverload : IActionFinished, IPowerUseValidity
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _triggerPower;

        public CustomBehaviorBlastPursuitOrOverload(
            FeatureDefinitionPower triggerPower,
            ConditionDefinition conditionDefinition)
        {
            _triggerPower = triggerPower;
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnActionFinished(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUsePower characterActionUsePower ||
                characterActionUsePower.activePower.PowerDefinition != _triggerPower)
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;
            var rulesetHero = rulesetCharacter?.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var slotLevel = SharedSpellsContext.GetWarlockSpellLevel(rulesetHero);

            SharedSpellsContext.GetWarlockSpellRepertoire(rulesetHero)?.SpendSpellSlot(slotLevel);
        }

        public bool CanUsePower(RulesetCharacter rulesetCharacter, FeatureDefinitionPower power)
        {
            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero is not { IsDeadOrDyingOrUnconscious: false })
            {
                return false;
            }

            return !rulesetHero.HasConditionOfType(_conditionDefinition.Name) &&
                   SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) >= 1;
        }
    }

    private sealed class TargetReducedToZeroHpBlastPursuit : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode, RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (activeEffect is not RulesetEffectSpell spell ||
                spell.spellDefinition != EldritchBlast ||
                !rulesetAttacker.HasConditionOfType(ConditionBlastPursuit))
            {
                yield break;
            }

            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionBlastPursuit.Name, out var rulesetCondition))
            {
                rulesetCondition.EndOccurence = TurnOccurenceType.EndOfTurn;
                rulesetCondition.RemainingRounds += 1; // always use setter to avoid issues with game effects manager
            }

            var rulesetEffectPower =
                attacker.RulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.Name == PowerBlastPursuitName);

            if (rulesetEffectPower != null)
            {
                rulesetEffectPower.RemainingRounds += 1; // always use setter to avoid issues with game effects manager
            }
        }
    }

    private sealed class ExtraActionBlastPursuit : IActionExecutionHandled
    {
        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // skip if wrong scope or action type
            if (scope != ActionScope.Battle || actionParams.ActionDefinition.ActionType != ActionType.Main)
            {
                return;
            }

            // skip if still has attacks available
            if (gameLocationCharacter.GetActionAvailableIterations(Id.AttackMain) > 0)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;
            var rulesetHero = rulesetCharacter?.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var eldritchSurgeLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);
            var warlockRemainingSlots = SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero);
            var blastPursuitExtraActionCount =
                GetBlastPursuitExtraActionCount(rulesetCharacter, eldritchSurgeLevel >= 18 ? 1 : 0);

            var isValidBlastPursuit =
                rulesetCharacter.HasConditionOfType(ConditionBlastPursuit) &&
                blastPursuitExtraActionCount == 0;

            var isValidPermanentBlastPursuit =
                eldritchSurgeLevel >= 18 &&
                blastPursuitExtraActionCount <= 1 &&
                warlockRemainingSlots <= 0;

            var isEldritchBlast =
                actionParams.activeEffect is RulesetEffectSpell spell && spell.spellDefinition == EldritchBlast;

            if (!isValidBlastPursuit && (!isValidPermanentBlastPursuit || !isEldritchBlast))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                ConditionExtraActionBlastPursuit.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0
            );

            var title = ConditionExtraActionBlastPursuit.GuiPresentation.Title;

            rulesetCharacter.ShowLabel(title, Gui.ColorPositive);
            rulesetCharacter.LogCharacterActivatesAbility(title, "Feedback/&BlastPursuitExtraAction", true);
        }
    }

    private sealed class CustomBehaviorBlastReload :
        IActionExecutionHandled, ICharacterTurnStartListener, IQualifySpellToRepertoireLine
    {
        // use a map to ensure any collateral scenarios with other warlocks casting cantrips on this warlock turn
        private readonly Dictionary<RulesetCharacter, List<SpellDefinition>> _cantripsUsedThisTurn = new();

        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // only collect cantrips
            if (actionParams.activeEffect is not RulesetEffectSpell rulesetEffectSpell ||
                rulesetEffectSpell.SpellDefinition.SpellLevel != 0)
            {
                return;
            }

            // only collect cantrips if an Eldritch Surge of at least level 14
            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;
            var eldritchSurgeLevel = rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } || eldritchSurgeLevel < 14)
            {
                return;
            }

            _cantripsUsedThisTurn.TryAdd(rulesetCharacter, new List<SpellDefinition>());
            _cantripsUsedThisTurn[rulesetCharacter].TryAdd(rulesetEffectSpell.SpellDefinition);
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // clean up cantrips map on every turn start
            _cantripsUsedThisTurn.Clear();
        }

        public void QualifySpells(
            RulesetCharacter rulesetCharacter,
            SpellRepertoireLine spellRepertoireLine,
            IEnumerable<SpellDefinition> spells)
        {
            // _cantripsUsedThisTurn only has entries for Eldritch Surge of at least level 14
            if (spellRepertoireLine.actionType != ActionType.Bonus ||
                !_cantripsUsedThisTurn.TryGetValue(rulesetCharacter, out var cantrips))
            {
                return;
            }

            spellRepertoireLine.relevantSpells.AddRange(cantrips.Intersect(spells));
        }
    }
}
