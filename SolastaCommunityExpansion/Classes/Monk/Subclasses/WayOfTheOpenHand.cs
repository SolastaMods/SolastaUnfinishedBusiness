using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Features;
using SolastaCommunityExpansion.Models;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses;

public static class WayOfTheOpenHand
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("ClassMonkTraditionWayOfTheOpenHand", DefinitionBuilder.CENamespaceGuid)
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(BuildOpenHandTechnique(), 3)
            .AddToDB();
    }

    private static FeatureDefinition BuildOpenHandTechnique()
    {
        var hasFlurry = CharacterValidators.HasAnyOfConditions("ClassMonkFlurryOfBlowsCondition");
        
        var technique = FeatureDefinitionPowerBuilder
            .Create("ClassMonkOpenHandTechnique", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetCostPerUse(0)
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                ReactionAttackModeRestriction.MeleeOnly,
                (_, character, _) => hasFlurry(character)
            ))
            .AddToDB();
        
        var prone = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMonkOpenHandProne", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetSharedPool(technique)
            .SetActivationTime(ActivationTime.NoCost)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Dexterity,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Wisdom
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel, 1)
                    .SetMotionForm(MotionForm.MotionType.FallProne, 0)
                    .Build())
                .Build())
            .AddToDB();
        
        var push = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMonkOpenHandPush", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetSharedPool(technique)
            .SetActivationTime(ActivationTime.NoCost)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetSavingThrowData(
                    true,
                    true,
                    AttributeDefinitions.Strength,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Wisdom
                )
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.Negates)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel, 1)
                    .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                    .Build())
                .Build())
            .AddToDB();
        
        var distract = FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMonkOpenHandDistract", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetSharedPool(technique)
            .SetActivationTime(ActivationTime.NoCost)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetDurationData(DurationType.Round, 1)
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                .SetEffectForms(new EffectFormBuilder()
                    .HasSavingThrow(EffectSavingThrowType.None)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel, 1)
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ClassMonkOpenHandDistractCondition", Monk.GUID)
                        .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
                        .SetDuration(DurationType.Round, 1)
                        .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
                        .SetConditionType(ConditionType.Detrimental)
                        .SetFeatures(FeatureDefinitionActionAffinitys.ActionAffinityConditionShocked)
                        .AddToDB(), ConditionForm.ConditionOperation.Add)
                    .Build())
                .Build())
            .AddToDB();

        PowerBundleContext.RegisterPowerBundle(technique, true, prone, push, distract);

        return technique;
        // return FeatureDefinitionFeatureSetBuilder
        //     .Create("ClassMonkOpenHandTechniqueBUNDLE", Monk.GUID)
        //     .SetGuiPresentation(Category.Feature)
        //     .SetFeatureSet(prone, push, distract)
        //     .AddToDB();
    }
}