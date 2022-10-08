using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using UnityEngine.AddressableAssets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class HealingFeats
{
    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        var inspiringEffect = BuildEffectDescriptionTempHpForm(RuleDefinitions.RangeType.Distance, 10,
            RuleDefinitions.TargetType.Individuals, 6, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, 0,
            RuleDefinitions.DieType.D1, 0, 1);

        var powerFeatInspiringLeader = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Charisma, RuleDefinitions.ActivationTime.Minute10, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Charisma, inspiringEffect,
            "PowerFeatInspiringLeader", PowerOathOfTirmarGoldenSpeech.GuiPresentation.SpriteReference);

        var medKitEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, 4,
            RuleDefinitions.DieType.D6, 1, 1);

        var powerFeatHealerMedKit = BuildPowerFromEffectDescription(0,
            RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Wisdom, medKitEffect,
            "PowerFeatHealerMedKit", PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

        var resuscitateEffect = BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            12 /* seconds since death */);

        var powerFeatHealerResuscitate = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Wisdom, resuscitateEffect,
            "PowerFeatHealerResuscitate", PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

        var powerFeatHealerStabilize = BuildPowerFromEffectDescription(0,
            RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Wisdom,
            DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription,
            "PowerFeatHealerStabilize", PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

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

    private static FeatureDefinitionPower BuildPowerFromEffectDescription(
        int usesPerRecharge,
        RuleDefinitions.UsesDetermination usesDetermination,
        string usesAbilityScoreName,
        RuleDefinitions.ActivationTime activationTime,
        int costPerUse,
        RuleDefinitions.RechargeRate recharge,
        bool proficiencyBonusToAttack,
        bool abilityScoreBonusToAttack, string abilityScore,
        EffectDescription effectDescription,
        string name,
        AssetReferenceSprite assetReferenceSprite)
    {
        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation(name, Category.Feature, assetReferenceSprite)
            .Configure(
                usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge,
                proficiencyBonusToAttack,
                abilityScoreBonusToAttack, abilityScore, effectDescription)
            .AddToDB();
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
        return new EffectDescriptionBuilder()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .AddEffectForm(
                new EffectFormBuilder()
                    .SetTempHpForm(bonusHitPoints, dieType, diceNumber)
                    .SetLevelAdvancement(applyLevel, levelType, levelMultiplier)
                    .CreatedByCharacter()
                    .SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus)
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription
                .EffectParticleParameters)
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
        return new EffectDescriptionBuilder()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .AddEffectForm(
                new EffectFormBuilder()
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
            .SetParticleEffectParameters(
                DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters)
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
        return new EffectDescriptionBuilder()
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
            .AddEffectForm(
                new EffectFormBuilder()
                    .SetReviveForm(secondsSinceDeath, RuleDefinitions.ReviveHitPoints.One)
                    .CreatedByCharacter()
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon
                .EffectDescription.EffectParticleParameters)
            .Build();
    }
}
