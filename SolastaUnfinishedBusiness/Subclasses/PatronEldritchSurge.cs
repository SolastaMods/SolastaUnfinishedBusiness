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
    // Original name to keep save integrity
    private static readonly FeatureDefinitionAdditionalAction AdditionalActionBlastPursuit =
        FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BlastPursuit")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(AllowDuplicates.Mark)
            .SetActionType(ActionType.Main)
            .SetForbiddenActions(Id.AttackMain)
            .AddToDB();

    private static readonly FeatureDefinitionAdditionalAction AdditionalActionBlastPursuitOff =
        FeatureDefinitionAdditionalActionBuilder
            .Create($"AdditionalAction{Name}BlastPursuitOff")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(AllowDuplicates.Mark)
            .SetActionType(ActionType.Bonus)
            .SetForbiddenActions(Id.AttackOff)
            .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(AdditionalActionBlastPursuit)
        .AddToDB();

    private static readonly ConditionDefinition ConditionExtraActionBlastPursuitOff = ConditionDefinitionBuilder
        .Create($"Condition{Name}ExtraActionBlastPursuitOff")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeraldOfBattle)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetFeatures(AdditionalActionBlastPursuitOff)
        .AddToDB();

    private static readonly ConditionDefinition ConditionBlastPursuit = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastPursuit")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHasted)
        .SetConditionParticleReference(ConditionDefinitions.ConditionShine.conditionParticleReference)
        .AddFeatures(
            FeatureDefinitionBuilder
                .Create($"Feature{Name}BlastPursuit")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new TargetReducedToZeroHpBlastPursuit())
                .AddToDB())
        .AddToDB();

    // LEVEL 10 Blast Overload

    private static readonly ConditionDefinition ConditionBlastOverload = ConditionDefinitionBuilder
        .Create($"Condition{Name}BlastOverload")
        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeroism)
        .CopyParticleReferences(ConditionDefinitions.ConditionRaging)
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

    // LEVEL 14 Blast Reload

    public static FeatureDefinition FeatureBlastReload = FeatureDefinitionBuilder
        .Create($"Feature{Name}BlastReload")
        .SetGuiPresentation(Category.Feature)
        .SetCustomSubFeatures(new CustomBehaviorBlastReload())
        .AddToDB();

    internal PatronEldritchSurge()
    {
        // LEVEL 01

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
                .SetDurationData(DurationType.Round, 2, TurnOccurenceType.StartOfTurn)
                .SetParticleEffectParameters(SpellDefinitions.Haste)
                .SetEffectForms(
                    EffectFormBuilder.ConditionForm(ConditionBlastPursuit),
                    EffectFormBuilder.ConditionForm(ConditionExtraActionBlastPursuit),
                    EffectFormBuilder.ConditionForm(ConditionExtraActionBlastPursuitOff))
                .Build())
            .AddToDB();

        powerBlastPursuit.effectDescription.effectParticleParameters.casterParticleReference = null;

        powerBlastPursuit.SetCustomSubFeatures(
            new CustomBehaviorBlastPursuitOrOverload(powerBlastPursuit, ConditionBlastPursuit),
            new ExtraActionBlastPursuit());

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
        // Moved to static property

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronEldritchSurge, 256))
            .AddFeaturesAtLevel(1, bonusCantripsEldritchSurgeBlastExclusive)
            .AddFeaturesAtLevel(6, powerBlastPursuit)
            .AddFeaturesAtLevel(10, powerBlastOverload)
            .AddFeaturesAtLevel(14, FeatureBlastReload)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static bool IsEldritchBlast(RulesetEffect rulesetEffect)
    {
        return rulesetEffect is RulesetEffectSpell rulesetEffectSpell &&
               rulesetEffectSpell.SpellDefinition == EldritchBlast;
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
            var pursuitStatus = rulesetCharacter.HasConditionOfType(ConditionBlastPursuit)
                ? 1 + (warlockClassLevel >= 16 ? 1 : 0)
                : 0;
            var overloadStatus = rulesetCharacter.HasConditionOfType(ConditionBlastOverload) ? 1 : 0;

            effectDescription.effectAdvancement.Clear();
            effectDescription.targetParameter = 1 + additionalBeamCount;
            effectDescription.rangeParameter = Math.Max(1, 24 - (6 * (pursuitStatus + overloadStatus)));

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
            var rulesetHero = rulesetCharacter.GetOriginalHero();

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

            return !rulesetCharacter.HasConditionOfType(_conditionDefinition) &&
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

            if (!IsEldritchBlast(activeEffect) ||
                !rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionBlastPursuit.Name, out var rulesetCondition))
            {
                yield break;
            }

            rulesetCondition.EndOccurence = TurnOccurenceType.EndOfTurn;
            rulesetCondition.RemainingRounds += 1; // always use setter to avoid issues with game effects manager

            var rulesetEffectPower =
                attacker.RulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.Name == PowerBlastPursuitName);

            if (rulesetEffectPower != null)
            {
                rulesetEffectPower.RemainingRounds += 1; // always use setter to avoid issues with game effects manager
            }
        }
    }

    private sealed class ExtraActionBlastPursuit : IActionExecutionHandled, ICharacterTurnStartListener
    {
        public void OnActionExecutionHandled(
            GameLocationCharacter gameLocationCharacter,
            CharacterActionParams actionParams,
            ActionScope scope)
        {
            // skip if wrong scope or action type
            if (scope != ActionScope.Battle)
            {
                return;
            }

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.HasConditionOfType(ConditionBlastPursuit))
            {
                return;
            }

            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var eldritchSurgeLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            if (eldritchSurgeLevel >= 16)
            {
                return;
            }

            var actionType = actionParams.ActionDefinition.ActionType;
            var featureToCheck = actionType switch
            {
                ActionType.Main => AdditionalActionBlastPursuit,
                ActionType.Bonus => AdditionalActionBlastPursuitOff,
                _ => null
            };
            var conditionToRemove = actionType switch
            {
                ActionType.Main => ConditionExtraActionBlastPursuitOff.Name,
                ActionType.Bonus => ConditionExtraActionBlastPursuit.Name,
                _ => null
            };

            if (featureToCheck == null)
            {
                return;
            }

            var filters = gameLocationCharacter.ActionPerformancesByType[actionType];
            var k = gameLocationCharacter.CurrentActionRankByType[actionType] - 1;
            var f = k >= 0 && k < filters.Count ? PerformanceFilterExtraData.GetData(filters[k]) : null;
            var feature = f?.Feature;

            if (feature == featureToCheck)
            {
                rulesetCharacter.RemoveAllConditionsOfType(conditionToRemove);
            }
        }

        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var rulesetHero = rulesetCharacter.GetOriginalHero();

            if (rulesetHero == null)
            {
                return;
            }

            var eldritchSurgeLevel = rulesetHero.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name);

            if (eldritchSurgeLevel == 20 && SharedSpellsContext.GetWarlockRemainingSlots(rulesetHero) <= 0)
            {
                InflictCondition(rulesetCharacter, ConditionBlastPursuit);
            }

            if (!rulesetCharacter.HasConditionOfType(ConditionBlastPursuit))
            {
                return;
            }

            InflictCondition(rulesetCharacter, ConditionExtraActionBlastPursuit);
            InflictCondition(rulesetCharacter, ConditionExtraActionBlastPursuitOff);
        }

        private static void InflictCondition(RulesetCharacter rulesetCharacter, ConditionDefinition condition)
        {
            if (rulesetCharacter.HasConditionOfType(condition))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                condition.Name,
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

            if (condition != ConditionBlastPursuit)
            {
                return;
            }

            var title = condition.GuiPresentation.Title;

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

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Warlock, Name) < 14)
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
