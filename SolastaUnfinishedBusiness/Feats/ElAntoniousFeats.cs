using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ElAntoniousFeats
{
    private static ConditionDefinition _conditionDualFlurryApply;
    private static ConditionDefinition _conditionDualFlurryGrant;

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildFeatDualFlurry());
        feats.Add(BuildFeatTorchbearer());
    }

    private static FeatDefinition BuildFeatDualFlurry()
    {
        _conditionDualFlurryApply = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryApply")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .AddToDB();

        _conditionDualFlurryGrant = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryGrant")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive(true)
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

        return FeatDefinitionBuilder
            .Create("FeatDualFlurry")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionOnAttackDamageEffectBuilder
                    .Create("OnAttackDamageEffectFeatDualFlurry")
                    .SetGuiPresentation("FeatDualFlurry", Category.Feat)
                    .SetOnAttackDamageDelegates(null, AfterOnAttackDamage)
                    .AddToDB())
            .AddToDB();
    }

    private static void AfterOnAttackDamage(
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
            attacker.RulesetCharacter.HasConditionOfType(_conditionDualFlurryApply.Name)
                ? _conditionDualFlurryGrant
                : _conditionDualFlurryApply;

        var rulesetCondition = RulesetCondition.CreateActiveCondition(
            attacker.RulesetCharacter.Guid,
            condition, DurationType.Round, 0,
            TurnOccurenceType.EndOfTurn,
            attacker.RulesetCharacter.Guid,
            attacker.RulesetCharacter.CurrentFaction.Name);

        attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
    }

    private static FeatDefinition BuildFeatTorchbearer()
    {
        var burnEffect = new EffectForm
        {
            formType = EffectForm.EffectFormType.Condition,
            ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add,
                ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionOnFire1D4
            }
        };

        var burnDescription = new EffectDescription();

        burnDescription.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
        burnDescription.SetCreatedByCharacter(true);
        burnDescription.SetTargetSide(Side.Enemy);
        burnDescription.SetTargetType(TargetType.Individuals);
        burnDescription.SetTargetParameter(1);
        burnDescription.SetRangeType(RangeType.Touch);
        burnDescription.SetDurationType(DurationType.Round);
        burnDescription.SetDurationParameter(3);
        burnDescription.SetCanBePlacedOnCharacter(false);
        burnDescription.SetHasSavingThrow(true);
        burnDescription.SetSavingThrowAbility(AttributeDefinitions.Dexterity);
        burnDescription.SetSavingThrowDifficultyAbility(AttributeDefinitions.Dexterity);
        burnDescription.SetDifficultyClassComputation(EffectDifficultyClassComputation.AbilityScoreAndProficiency);
        burnDescription.SetSpeedType(SpeedType.Instant);
        burnDescription.EffectForms.Clear();
        burnDescription.EffectForms.Add(burnEffect);

        var powerTorchbearer = FeatureDefinitionPowerBuilder
            .Create("PowerTorchbearer")
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(ActivationTime.BonusAction)
            .SetEffectDescription(burnDescription)
            .SetUsesFixed(1)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetShowCasting(false)
            .SetCustomSubFeatures(new ValidatorPowerUse(ValidatorsCharacter.OffHandHasLightSource))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatTorchbearer")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerTorchbearer)
            .AddToDB();
    }
}
