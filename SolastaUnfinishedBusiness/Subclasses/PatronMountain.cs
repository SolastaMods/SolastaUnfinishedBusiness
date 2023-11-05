using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public class PatronMountain : AbstractSubclass
{
    private const string Name = "PatronMountain";

    public PatronMountain()
    {
        // LEVEL 01

        var spellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, SpellsContext.EarthTremor, Sleep)
            .SetSpellsAtLevel(2, HeatMetal, LesserRestoration)
            .SetSpellsAtLevel(3, ProtectionFromEnergy, SleetStorm)
            .SetSpellsAtLevel(4, FreedomOfMovement, IceStorm)
            .SetSpellsAtLevel(5, ConeOfCold, GreaterRestoration)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellList)
            .AddToDB();

        // Barrier of Stone

        var conditionBarrierOfStone = ConditionDefinitionBuilder
            .Create($"Condition{Name}BarrierOfStone")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var reduceDamageBarrierOfStone = FeatureDefinitionReduceDamageBuilder
            .Create($"ReduceDamage{Name}BarrierOfStone")
            .SetGuiPresentation($"Power{Name}BarrierOfStone", Category.Feature)
            .SetAlwaysActiveReducedDamage((_, defender) =>
            {
                var rulesetCharacter = defender.RulesetCharacter;

                if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
                {
                    return 0;
                }

                var usableCondition =
                    rulesetCharacter.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == conditionBarrierOfStone);

                if (usableCondition == null)
                {
                    return 0;
                }

                var rulesetCaster = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

                if (rulesetCaster == null)
                {
                    return 0;
                }

                var levels = rulesetCaster.GetClassLevel(CharacterClassDefinitions.Warlock);
                var charismaModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                    rulesetCaster.TryGetAttributeValue(AttributeDefinitions.Charisma));

                return (2 * levels) + charismaModifier;
            })
            .AddToDB();

        conditionBarrierOfStone.Features.Add(reduceDamageBarrierOfStone);

        var powerBarrierOfStone = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BarrierOfStone")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerBarrierOfStone.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeBarrierOfStone(powerBarrierOfStone, conditionBarrierOfStone));

        // Knowledge of Aeons

        var proficiencyNatureSurvival = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}NatureSurvival")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Nature, SkillDefinitions.Survival)
            .AddToDB();

        var featureSetKnowledgeOfAeons = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}KnowledgeOfAeons")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                proficiencyNatureSurvival,
                FeatureDefinitionTerrainTypeAffinitys.TerrainTypeAffinityRangerNaturalExplorerMountain)
            .AddToDB();

        // LEVEL 06

        // Clinging Strength

        var conditionClingingStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionLongstrider, $"Condition{Name}ClingingStrength")
            .SetGuiPresentation($"Power{Name}ClingingStrength", Category.Feature, Gui.NoLocalization,
                ConditionDefinitions.ConditionLongstrider.GuiPresentation.SpriteReference)
            .AddFeatures(FeatureDefinitionMovementAffinitys.MovementAffinitySpiderClimb)
            .AddToDB();

        var powerClingingStrength = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ClingingStrength")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionClingingStrength))
                    .Build())
            .AddToDB();

        // Eternal Guardian

        var powerEternalGuardian = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EternalGuardian")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetOverriddenPower(powerBarrierOfStone)
            .AddToDB();

        powerEternalGuardian.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedOnMeBarrierOfStone(powerEternalGuardian, conditionBarrierOfStone));

        // LEVEL 10

        // The Mountain Wakes

        var powerTheMountainWakes = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TheMountainWakes")
            .SetGuiPresentation(Category.Feature, IceStorm)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(IceStorm.EffectDescription)
            .AddToDB();

        // LEVEL 14

        // Icebound Soul

        var conditionIceboundSoul = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"Condition{Name}IceboundSoul")
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetParentCondition(ConditionDefinitions.ConditionBlinded)
            .AddToDB();

        var additionalDamageIceboundSoul = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}IceboundSoul")
            .SetGuiPresentation($"FeatureSet{Name}IceboundSoul", Category.Feature)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackOnly()
            .SetSavingThrowData()
            .SetConditionOperations(new ConditionOperationDescription
            {
                operation = ConditionOperationDescription.ConditionOperation.Add,
                conditionDefinition = conditionIceboundSoul,
                hasSavingThrow = true,
                saveAffinity = EffectSavingThrowType.Negates
            })
            .AddToDB();

        var featureSetIceboundSoul = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}IceboundSoul")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                additionalDamageIceboundSoul,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.MartialMountaineer)
            .AddFeaturesAtLevel(1,
                magicAffinityExpandedSpells,
                featureSetKnowledgeOfAeons,
                powerBarrierOfStone)
            .AddFeaturesAtLevel(6,
                powerClingingStrength,
                powerEternalGuardian)
            .AddFeaturesAtLevel(10,
                powerTheMountainWakes)
            .AddFeaturesAtLevel(14,
                featureSetIceboundSoul)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class AttackBeforeHitConfirmedOnMeBarrierOfStone :
        IAttackBeforeHitConfirmedOnMeOrAlly, IMagicalAttackBeforeHitConfirmedOnMeOrAlly
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public AttackBeforeHitConfirmedOnMeBarrierOfStone(
            FeatureDefinitionPower featureDefinitionPower,
            ConditionDefinition conditionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMeOrAlly(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null)
            {
                yield return HandleReaction(defender, me);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMeOrAlly(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter me,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(defender, me);
        }

        private IEnumerator HandleReaction(GameLocationCharacter defender, GameLocationCharacter me)
        {
            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            if (!me.CanReact() || me == defender)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var gameLocationBattleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationBattleManager is not { IsBattleInProgress: true } || gameLocationActionManager == null)
            {
                yield break;
            }

            if (!gameLocationBattleManager.IsWithinXCells(me, defender, 7))
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (rulesetMe.GetRemainingPowerCharges(_featureDefinitionPower) <= 0)
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_featureDefinitionPower, rulesetMe);
            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "BarrierOfStone", UsablePower = usablePower
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendPower(reactionParams);

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleManager.WaitForReactions(
                me, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetMe.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);
            rulesetDefender.InflictCondition(
                _conditionDefinition.Name,
                _conditionDefinition.DurationType,
                _conditionDefinition.DurationParameter,
                _conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetMe.Guid,
                rulesetMe.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }
}
