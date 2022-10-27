using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class HealingFeats
{
    internal static void CreateFeats(List<FeatDefinition> feats)
    {
        var powerFeatInspiringLeader = FeatureDefinitionPowerBuilder
            .Create("PowerFeatInspiringLeader")
            .SetGuiPresentation(Category.Feature, PowerOathOfTirmarGoldenSpeech)
            .SetUsesFixed(ActivationTime.Minute10, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Distance, 10, TargetType.Individuals, 6)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetTempHpForm()
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                        .CreatedByCharacter()
                        .SetBonusMode(AddBonusMode.AbilityBonus)
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealingOther)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetHealingForm(
                            HealingComputation.Dice,
                            4,
                            DieType.D6,
                            1,
                            false,
                            HealingCap.MaximumHitPoints)
                        .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                        .CreatedByCharacter()
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(
                    TargetFilteringMethod.CharacterOnly,
                    TargetFilteringTag.No,
                    5,
                    DieType.D8)
                .SetCreatedByCharacter()
                .SetDurationData(DurationType.Permanent)
                .SetRequiredCondition(DatabaseHelper.ConditionDefinitions.ConditionDead)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetReviveForm(12, ReviveHitPoints.One)
                        .CreatedByCharacter()
                        .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var powerFeatHealerStabilize = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerStabilize")
            .SetGuiPresentation(Category.Feature, PowerDomainLifePreserveLife)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription)
            .AddToDB();

        var proficiencyFeatHealerMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatHealerMedicine")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
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
                .SetGuiPresentation(Category.Feat, PowerFunctionGoodberryHealingOther)
                .SetFeatures(
                    powerFeatHealerMedKit,
                    powerFeatHealerResuscitate,
                    powerFeatHealerStabilize,
                    proficiencyFeatHealerMedicine)
                .AddToDB());
    }
}
