using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Monk.Subclasses;

public static class WayOfTheOpenHand
{
    public static CharacterSubclassDefinition Build()
    {
        return CharacterSubclassDefinitionBuilder
            .Create("ClassMonkTraditionWayOfTheOpenHand", DefinitionBuilder.CENamespaceGuid)
            .SetOrUpdateGuiPresentation(Category.Subclass,
                CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(BuildOpenHandTechnique(), 3)
            .AddFeatureAtLevel(BuildWholenessOfBody(), 6)
            .AddFeatureAtLevel(BuildTanquility(), 11)
            .AddFeatureAtLevel(BuildQuiveringPalm(), 17)
            .AddToDB();
    }

    private static FeatureDefinition BuildOpenHandTechnique()
    {
        var technique = FeatureDefinitionPowerBuilder
            .Create("ClassMonkOpenHandTechnique", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetRechargeRate(RechargeRate.AtWill)
            .SetUsesFixed(1)
            // need to check for mode == null otherwise it breaks cantrips like Hurl Flame
            .SetCustomSubFeatures(new ReactionAttackModeRestriction(
                (mode, _, _, _) => mode != null && mode.AttackTags.Contains(Monk.FlurryTag)
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
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
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
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
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
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.No, LevelSourceType.ClassLevel)
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ClassMonkOpenHandDistractCondition", Monk.GUID)
                        .SetGuiPresentation(Category.Condition,
                            ConditionDefinitions.ConditionDazzled.GuiPresentation.SpriteReference)
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
    }

    private static FeatureDefinition BuildWholenessOfBody()
    {
        return FeatureDefinitionPowerBuilder
            .Create("ClassMonkWholenessOfBody", Monk.GUID)
            .SetGuiPresentation(Category.Power,
                FeatureDefinitionPowers.PowerPaladinLayOnHands.GuiPresentation.SpriteReference)
            .SetRechargeRate(RechargeRate.LongRest)
            .SetUsesFixed(1)
            .SetCostPerUse(1)
            .SetActivationTime(ActivationTime.Action)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(new EffectFormBuilder()
                    //TODO: for some reason TA haven't implemented `MultiplyDice` advancement type for healing
                    //TODO: power tooltip doesn't show actual value or even advancement type, can we fix this?
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.MultiplyBonus, LevelSourceType.ClassLevel)
                    .SetHealingForm(HealingComputation.Dice, 3, DieType.D1, 0, false, HealingCap.MaximumHitPoints)
                    .Build())
                .Build())
            .AddToDB();
    }

    private static FeatureDefinition BuildTanquility()
    {
        var tranquility = FeatureDefinitionPowerBuilder
            .Create("ClassMonkTanquility", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetActivationTime(ActivationTime.NoCost)
            .SetCostPerUse(1)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                .SetDurationData(DurationType.UntilAnyRest)
                .SetEffectForms(new EffectFormBuilder()
                    .SetConditionForm(ConditionDefinitionBuilder
                        .Create("ClassMonkTanquilityCondition", Monk.GUID)
                        .SetGuiPresentation(Category.Condition,
                            ConditionDefinitions.ConditionBlurred.GuiPresentation.SpriteReference)
                        .SetDuration(DurationType.UntilAnyRest)
                        .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.CastSpell)
                        .SetFeatures(
                            FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityMagebaneRejectMagic,
                            FeatureDefinitionCombatAffinitys.CombatAffinityDodging)
                        .AddToDB(), ConditionForm.ConditionOperation.Add, true, false)
                    .Build())
                .Build())
            .AddToDB();

        return tranquility;
    }

    private static FeatureDefinition BuildQuiveringPalm()
    {
        return FeatureDefinitionPowerSharedPoolBuilder
            .Create("ClassMonkQuiveringPalm", Monk.GUID)
            .SetGuiPresentation(Category.Power)
            .SetActivationTime(ActivationTime.OnAttackHit)
            .SetSharedPool(Monk.KiPool)
            .SetCostPerUse(3)
            .SetFixedUsesPerRecharge(3)
            .SetRechargeRate(RechargeRate.ShortRest)
            .SetCustomSubFeatures(
                new ReactionAttackModeRestriction((mode, _, _, _) =>
                    mode != null && WeaponValidators.IsUnarmedWeapon(mode)))
            .SetEffectDescription(new EffectDescriptionBuilder()
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Instantaneous)
                .SetSavingThrowData(true,
                    true,
                    AttributeDefinitions.Constitution,
                    true, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Wisdom
                )
                .SetEffectForms(
                    new EffectFormBuilder()
                        .SetKillForm(KillCondition.Always)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build(),
                    new EffectFormBuilder()
                        .SetDamageForm(diceNumber: 10, dieType: DieType.D10, damageType: DamageTypeNecrotic)
                        .HasSavingThrow(EffectSavingThrowType.None)
                        .Build())
                .Build())
            .AddToDB();
    }
}
