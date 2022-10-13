using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ElAntoniousFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var conditionDualFlurryApply = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryApply")
            .SetGuiPresentation(Category.Condition)
            //TODO: Double check duration equals 1 won't break things
            // .SetDuration(DurationType.Round, 0, false)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .AddToDB();

        var conditionDualFlurryGrant = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryGrant")
            .SetGuiPresentation(Category.Condition)
            //TODO: Double check duration equals 1 won't break things
            // .SetDuration(DurationType.Round, 0, false)
            .SetDuration(DurationType.Round, 1)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create(AdditionalActionSurgedMain, "AdditionalActionDualFlurry")
                    .SetGuiPresentation(Category.Feature, AdditionalActionSurgedMain.GuiPresentation.SpriteReference)
                    .SetActionType(ActionDefinitions.ActionType.Bonus)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                    .AddToDB())
            .AddToDB();

        void AfterOnAttackDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            [CanBeNull] RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            if (rangedAttack || attackMode == null)
            {
                return;
            }

            var condition =
                attacker.RulesetCharacter.HasConditionOfType(conditionDualFlurryApply.Name)
                    ? conditionDualFlurryGrant
                    : conditionDualFlurryApply;

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                condition,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }

        var featDualFlurry = FeatDefinitionBuilder
            .Create("FeatDualFlurry")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionOnAttackDamageEffectBuilder
                    .Create("OnAttackDamageEffectFeatDualFlurry")
                    .SetGuiPresentation("FeatDualFlurry", Category.Feat)
                    .SetOnAttackDamageDelegates(null, AfterOnAttackDamage)
                    .AddToDB())
            .AddToDB();

        var featTorchbearer = FeatDefinitionBuilder
            .Create("FeatTorchbearer")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionPowerBuilder
                .Create("PowerTorchbearer")
                .SetGuiPresentation(Category.Feature)
                .SetUsesFixed(ActivationTime.BonusAction)
                .SetEffectDescription(
                    EffectDescriptionBuilder
                        .Create(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription)
                        .SetCanBePlacedOnCharacter(false)
                        .SetCreatedByCharacter()
                        .SetDurationData(DurationType.Round, 3)
                        .SetSpeed(SpeedType.Instant, 11f)
                        .SetTargetingData(
                            Side.Enemy,
                            RangeType.Touch,
                            30,
                            TargetType.Individuals,
                            1,
                            2,
                            ActionDefinitions.ItemSelectionType.Equiped)
                        .SetEffectForms(
                            EffectFormBuilder
                                .Create()
                                .SetConditionForm(
                                    DatabaseHelper.ConditionDefinitions.ConditionOnFire1D4,
                                    ConditionForm.ConditionOperation.Add)
                                .Build())
                        .SetSavingThrowData(
                            false,
                            AttributeDefinitions.Dexterity,
                            false,
                            EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                            AttributeDefinitions.Dexterity,
                            15)
                        .Build())
                .SetShowCasting(false)
                .SetCustomSubFeatures(new ValidatorPowerUse(ValidatorsCharacter.OffHandHasLightSource))
                .AddToDB())
            .AddToDB();

        feats.AddRange(featTorchbearer, featDualFlurry);
    }
}
