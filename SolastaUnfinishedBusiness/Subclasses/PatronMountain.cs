using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
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

                if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, $"Condition{Name}BarrierOfStone",
                        out var activeCondition))
                {
                    return 0;
                }

                var rulesetCaster = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

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

        var conditionBarrierOfStone = ConditionDefinitionBuilder
            .Create($"Condition{Name}BarrierOfStone")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(reduceDamageBarrierOfStone)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var powerBarrierOfStone = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BarrierOfStone")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionBarrierOfStone))
                    .SetParticleEffectParameters(Banishment)
                    .Build())
            .AddToDB();

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
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetOverriddenPower(powerBarrierOfStone)
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        powerBarrierOfStone.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitConfirmedOnMeBarrierOfStone(powerBarrierOfStone, powerEternalGuardian));

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

        var additionalDamageIceboundSoul = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}IceboundSoul")
            .SetGuiPresentation($"FeatureSet{Name}IceboundSoul", Category.Feature)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackOnly()
            .SetSavingThrowData()
            .AddConditionOperation(new ConditionOperationDescription
            {
                operation = ConditionOperationDescription.ConditionOperation.Add,
                conditionDefinition = ConditionDefinitions.ConditionBlindedEndOfNextTurn,
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

    private class AttackBeforeHitConfirmedOnMeBarrierOfStone(
        FeatureDefinitionPower powerBarrierOfStone,
        FeatureDefinitionPower powerEternalGuardian)
        :
            IAttackBeforeHitConfirmedOnMeOrAlly, IMagicEffectBeforeHitConfirmedOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMeOrAlly(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
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
                yield return HandleReaction(attacker, defender, helper);
            }
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMeOrAlly(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(attacker, defender, helper);
        }

        private IEnumerator HandleReaction(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter me)
        {
            var rulesetMe = me.RulesetCharacter;
            var levels = rulesetMe.GetClassLevel(CharacterClassDefinitions.Warlock);
            var power = levels < 6 ? powerBarrierOfStone : powerEternalGuardian;

            if (me.IsMyTurn() ||
                !me.CanReact() ||
                me == defender ||
                !me.CanPerceiveTarget(defender) ||
                !me.CanPerceiveTarget(attacker) ||
                !me.IsWithinRange(defender, 7) ||
                rulesetMe.GetRemainingPowerUses(power) == 0)
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

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(power, rulesetMe);
            var actionParams =
                new CharacterActionParams(me, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "BarrierOfStone",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManagerService
                        .MyInstantiateEffectPower(rulesetMe, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };

            var count = gameLocationActionManager.PendingReactionRequestGroups.Count;

            gameLocationActionManager.ReactToUsePower(actionParams, "UsePower", me);

            yield return gameLocationBattleManager.WaitForReactions(attacker, gameLocationActionManager, count);
        }
    }
}
