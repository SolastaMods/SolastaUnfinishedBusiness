using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class HealingFeats
{
    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        var inspiringEffect = BuildEffectDescriptionTempHpForm(
            RuleDefinitions.RangeType.Distance,
            10,
            RuleDefinitions.TargetType.Individuals,
            6,
            RuleDefinitions.DurationType.Permanent,
            0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus,
            RuleDefinitions.LevelSourceType.CharacterLevel,
            0,
            RuleDefinitions.DieType.D1,
            0,
            1);

        var powerFeatInspiringLeader = FeatureDefinitionPowerBuilder
            .Create("PowerFeatInspiringLeader")
            .SetGuiPresentation(Category.Feature, PowerOathOfTirmarGoldenSpeech.GuiPresentation.SpriteReference)
            .SetUsesFixed(
                RuleDefinitions.ActivationTime.Minute10,
                RuleDefinitions.RechargeRate.ShortRest,
                inspiringEffect)
            .AddToDB();

        var medKitEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, 4,
            RuleDefinitions.DieType.D6, 1, 1);

        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(
                RuleDefinitions.ActivationTime.Action,
                RuleDefinitions.RechargeRate.ShortRest,
                AttributeDefinitions.Wisdom,
                medKitEffect)
            .AddToDB();

        var resuscitateEffect = BuildEffectDescriptionReviveForm(
            RuleDefinitions.RangeType.Touch,
            1,
            RuleDefinitions.TargetType.Individuals,
            1,
            RuleDefinitions.DurationType.Permanent,
            0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            12);

        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
            .SetUsesFixed(
                RuleDefinitions.ActivationTime.Action,
                RuleDefinitions.RechargeRate.LongRest,
                resuscitateEffect)
            .AddToDB();

        var powerFeatHealerStabilize = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerStabilize")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference)
            .SetUsesAbilityBonus(
                RuleDefinitions.ActivationTime.Action,
                RuleDefinitions.RechargeRate.ShortRest,
                AttributeDefinitions.Wisdom,
                DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription)
            .AddToDB();

        var proficiencyFeatHealerMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatHealerMedicine")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
            .AddToDB();

        feats.AddRange(
            FeatDefinitionBuilder
                .Create("FeatInspiringLeader")
                .SetGuiPresentation("PowerFeatInspiringLeader", Category.Feature)
                .SetFeatures(powerFeatInspiringLeader)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .AddToDB(),
            FeatDefinitionBuilder
                .Create("FeatHealer")
                .SetGuiPresentation(Category.Feat,
                    PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference)
                .SetFeatures(powerFeatHealerMedKit,
                    powerFeatHealerResuscitate, powerFeatHealerStabilize, proficiencyFeatHealerMedicine)
                .AddToDB());
    }

    private static EffectDescription BuildEffectDescriptionTempHpForm(
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        int targetParameter,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        EffectForm.LevelApplianceType applyLevel,
        RuleDefinitions.LevelSourceType levelType,
        int bonusHitPoints,
        RuleDefinitions.DieType dieType,
        int diceNumber,
        int levelMultiplier)
    {
        return EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetTempHpForm(bonusHitPoints, dieType, diceNumber)
                    .SetLevelAdvancement(applyLevel, levelType, levelMultiplier)
                    .CreatedByCharacter()
                    .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
            .Build();
    }

    private static EffectDescription BuildEffectDescriptionHealingForm(
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        int targetParameter,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        EffectForm.LevelApplianceType applyLevel,
        RuleDefinitions.LevelSourceType levelType,
        int bonusHitPoints,
        RuleDefinitions.DieType dieType,
        int diceNumber,
        int levelMultiplier)
    {
        return EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetHealingForm(
                        RuleDefinitions.HealingComputation.Dice,
                        bonusHitPoints,
                        dieType,
                        diceNumber,
                        false,
                        RuleDefinitions.HealingCap.MaximumHitPoints)
                    .SetLevelAdvancement(applyLevel, levelType, levelMultiplier)
                    .CreatedByCharacter()
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
            .Build();
    }

    private static EffectDescription BuildEffectDescriptionReviveForm(
        RuleDefinitions.RangeType rangeType,
        int rangeParameter,
        RuleDefinitions.TargetType targetType,
        int targetParameter,
        RuleDefinitions.DurationType durationType,
        int durationParameter,
        RuleDefinitions.TurnOccurenceType endOfEffect,
        int secondsSinceDeath)
    {
        return EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                rangeType,
                rangeParameter,
                targetType,
                targetParameter,
                0)
            .SetTargetFiltering(
                RuleDefinitions.TargetFilteringMethod.CharacterOnly,
                RuleDefinitions.TargetFilteringTag.No,
                5,
                RuleDefinitions.DieType.D8)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .SetRequiredCondition(DatabaseHelper.ConditionDefinitions.ConditionDead)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetReviveForm(secondsSinceDeath, RuleDefinitions.ReviveHitPoints.One)
                    .CreatedByCharacter()
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
            .Build();
    }
}
