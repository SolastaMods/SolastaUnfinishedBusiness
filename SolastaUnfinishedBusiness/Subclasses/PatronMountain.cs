using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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
internal class PatronMountain : AbstractSubclass
{
    private const string Name = "PatronMountain";

    internal PatronMountain()
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

        var powerBarrierOfStone = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BarrierOfStone")
            .SetGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerBarrierOfStone.SetCustomSubFeatures(new AttackBeforeHitConfirmedOnMeBarrierOfStone(powerBarrierOfStone));

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
            .SetUsesAbilityBonus(ActivationTime.Reaction, RechargeRate.ShortRest, AttributeDefinitions.Charisma)
            .SetReactionContext(ExtraReactionContext.Custom)
            .SetOverriddenPower(powerBarrierOfStone)
            .AddToDB();

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
            .SetGuiPresentationNoContent(true)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetSavingThrowData()
            .SetConditionOperations(new ConditionOperationDescription
            {
                operation = ConditionOperationDescription.ConditionOperation.Add,
                conditionDefinition = conditionIceboundSoul
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

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class AttackBeforeHitConfirmedOnMeBarrierOfStone :
        IAttackBeforeHitConfirmedOnMeOrAlly, IMagicalAttackBeforeHitConfirmedOnMeOrAlly
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public AttackBeforeHitConfirmedOnMeBarrierOfStone(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMeOrAlly(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter ally,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            if (defender == ally)
            {
                yield break;
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMeOrAlly(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter ally,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (defender == ally)
            {
                yield break;
            }
        }

        public IEnumerator HandleReaction(
            GameLocationBattleManager battle,
            GameLocationCharacter me)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == me)
            {
                yield break;
            }

            if (!me.CanReact())
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

            yield return battle.WaitForReactions(me, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetMe.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);

            //
            // implement damage reduction
            //
        }
    }
}
