using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class HealingFeats
{
    public static void CreateFeats(List<FeatDefinition> feats)
    {
        var inspiringLeaderPresentation = GuiPresentationBuilder.Build(
            "FeatInspiringLeader", Category.Feat, PowerOathOfTirmarGoldenSpeech.GuiPresentation.SpriteReference);

        var inspiringEffect = BuildEffectDescriptionTempHpForm(RuleDefinitions.RangeType.Distance, 10,
            RuleDefinitions.TargetType.Individuals, 6, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, true, 0,
            RuleDefinitions.DieType.D1, 0, 1);

        var powerFeatInspiringLeader = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Charisma, RuleDefinitions.ActivationTime.Minute10, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Charisma, inspiringEffect,
            "PowerFeatInspiringLeader", inspiringLeaderPresentation);

        feats.Add(FeatDefinitionBuilder
            .Create("FeatInspiringLeader")
            .SetFeatures(powerFeatInspiringLeader)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .SetGuiPresentation(inspiringLeaderPresentation)
            .AddToDB());

        var medKitPresentation = GuiPresentationBuilder.Build(
            "PowerFeatHealerMedKit", Category.Feature,
            PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

        var medKitEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, false, 4,
            RuleDefinitions.DieType.D6, 1, 1);

        var powerFeatHealerMedKit = BuildPowerFromEffectDescription(0,
            RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Wisdom, medKitEffect,
            "PowerFeatHealerMedKit", medKitPresentation);

        var resuscitatePresentation = GuiPresentationBuilder.Build(
            "PowerFeatHealerResuscitate", Category.Feature,
            PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

        var resuscitateEffect = BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType.Touch, 1,
            RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.EndOfTurn,
            12 /* seconds since death */);

        var powerFeatHealerResuscitate = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.LongRest,
            false, false, AttributeDefinitions.Wisdom, resuscitateEffect,
            "PowerFeatHealerResuscitate", resuscitatePresentation);

        var stabilizePresentation = GuiPresentationBuilder.Build(
            "PowerFeatHealerStabilize", Category.Feature, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

        var powerFeatHealerStabilize = BuildPowerFromEffectDescription(0,
            RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
            AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1,
            RuleDefinitions.RechargeRate.ShortRest,
            false, false, AttributeDefinitions.Wisdom,
            DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription,
            "PowerFeatHealerStabilize", stabilizePresentation);

        FeatureDefinition proficiencyFeatHealerMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatHealerMedicine")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        feats.Add(FeatDefinitionBuilder
            .Create("FeatHealer")
            .SetFeatures(proficiencyFeatHealerMedicine, powerFeatHealerMedKit, powerFeatHealerResuscitate,
                powerFeatHealerStabilize)
            .SetGuiPresentation(Category.Feat,
                PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference)
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
        GuiPresentation guiPresentation)
    {
        return FeatureDefinitionPowerBuilder
            .Create(name)
            .SetGuiPresentation(guiPresentation)
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
        bool applyAbilityBonus,
        int bonusHitPoints,
        RuleDefinitions.DieType dieType,
        int diceNumber,
        int levelMultiplier)
    {
        var effectFormBuilder = new EffectFormBuilder()
            .SetTempHPForm(bonusHitPoints, dieType, diceNumber)
            .SetLevelAdvancement(applyLevel, levelType, levelMultiplier)
            .CreatedByCharacter();

        if (applyAbilityBonus)
        {
            effectFormBuilder.SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);
        }

        return new EffectDescriptionBuilder()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .AddEffectForm(effectFormBuilder.Build())
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
        bool applyAbilityBonus,
        int bonusHitPoints,
        RuleDefinitions.DieType dieType,
        int diceNumber,
        int levelMultiplier)
    {
        var effectFormBuilder = new EffectFormBuilder()
            .SetHealingForm(
                RuleDefinitions.HealingComputation.Dice,
                bonusHitPoints,
                dieType,
                diceNumber,
                false,
                RuleDefinitions.HealingCap.MaximumHitPoints)
            .SetLevelAdvancement(applyLevel, levelType, levelMultiplier)
            .CreatedByCharacter();

        if (applyAbilityBonus)
        {
            effectFormBuilder.SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);
        }

        return new EffectDescriptionBuilder()
            .SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0)
            .SetCreatedByCharacter()
            .SetDurationData(durationType, durationParameter, endOfEffect)
            .AddEffectForm(effectFormBuilder.Build())
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
                    .SetReviveForm(
                        secondsSinceDeath,
                        RuleDefinitions.ReviveHitPoints.One,
                        new List<ConditionDefinition>())
                    .CreatedByCharacter()
                    .Build())
            .SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None)
            .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon
                .EffectDescription.EffectParticleParameters)
            .Build();
    }
}
