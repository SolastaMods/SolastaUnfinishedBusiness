using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Feats
{
    internal static class HealingFeats
    {
        public static readonly Guid HealingFeatNamespace = new("501448fd-3c84-4031-befe-84c2ae75123b");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Inspiring Leader- prereq charisma 13, spend 10 minutes inspiring folks to fit temp hp == level + charisma modifier (1/short rest)
            var inspiringLeaderPresentation = GuiPresentationBuilder.Build(
                "InspiringLeader", Category.Feat, PowerOathOfTirmarGoldenSpeech.GuiPresentation.SpriteReference);

            EffectDescription inspiringEffect = BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType.Distance, 10,
                RuleDefinitions.TargetType.Individuals, 6, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, true, 0, RuleDefinitions.DieType.D1, 0, 1);

            FeatureDefinitionPower inspiringPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma, RuleDefinitions.ActivationTime.Minute10, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Charisma, inspiringEffect,
                "PowerInspiringLeaderFeat", inspiringLeaderPresentation);

            feats.Add(FeatDefinitionBuilder
                .Create("FeatInspiringLeader", HealingFeatNamespace)
                .SetFeatures(inspiringPower)
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetGuiPresentation(inspiringLeaderPresentation)
                .AddToDB());

            // Healer- use a healer's kit to stabilize to 1hp, use an action to restore 1d6+4 hp, plus additional hp equal to creature's level (can only heal a given creature once per day)
            // thoughts- grant a stabilize power that sets them to 1hp (unlimited uses), prof per day a heal that does 1d6+4+"caster" level

            var medKitPresentation = GuiPresentationBuilder.Build(
                "HealerUseMedicine", Category.Feat, PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            EffectDescription medKitEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Touch, 1,
                RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.AddBonus, RuleDefinitions.LevelSourceType.CharacterLevel, false, 4, RuleDefinitions.DieType.D6, 1, 1);

            FeatureDefinitionPower medKitPower = BuildPowerFromEffectDescription(0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, medKitEffect,
                "PowerMedKitHealerFeat", medKitPresentation);

            var resuscitatePresentation = GuiPresentationBuilder.Build(
                "HealerResuscitate", Category.Feat, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

            EffectDescription resuscitateEffect = BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType.Touch, 1,
                RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                12 /* seconds since death */);

            FeatureDefinitionPower resuscitatePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Wisdom, resuscitateEffect,
                "PowerResuscitateHealerFeat", resuscitatePresentation);

            var stabilizePresentation = GuiPresentationBuilder.Build(
                "HealerStabilize", Category.Feat, PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

            FeatureDefinitionPower stabilizePower = BuildPowerFromEffectDescription(0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription,
                "PowerStabilizeHealerFeat", stabilizePresentation);

            FeatureDefinition medicineKnowledge = FeatureDefinitionProficiencyBuilder
                .Create("FeatHealerMedicineProficiency", HealingFeatNamespace)
                .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
                .SetGuiPresentation("ProfHealerMedicine", Category.Feat)
                .AddToDB();

            feats.Add(FeatDefinitionBuilder
                .Create("FeatHealer", HealingFeatNamespace)
                .SetFeatures(medicineKnowledge, medKitPower, resuscitatePower, stabilizePower)
                .SetGuiPresentation("Healer", Category.Feat, PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference)
                .AddToDB());

            // Chef: con/wis, short rest ability, everyone regains 1d8 (supposed to be only if they spend hit dice)
            //     once per long rest cook treats that grant temp hp (prof bonus # treats and #thp)

            // define power(s) that treats have
            var treatEatPresentation = GuiPresentationBuilder.Build(
                "ProfChefTreatAction", Category.Feat, PowerFunctionGoodberryHealing.GuiPresentation.SpriteReference);

            EffectDescription treatEffect = BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.No, RuleDefinitions.LevelSourceType.CharacterLevel, false, 5, RuleDefinitions.DieType.D1, 0, 1);
            FeatureDefinitionPower treatPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Wisdom,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.None, false, false, AttributeDefinitions.Wisdom,
                treatEffect, "ChefTreatEatPower", treatEatPresentation);

            // define treats
            var treat = ItemDefinitionBuilder
                .Create(DatabaseHelper.ItemDefinitions.Berry_Ration, "ChefSnackTreat", HealingFeatNamespace)
                .SetOrUpdateGuiPresentation("ProfChefTreat", Category.Feat)
                .SetUsableDeviceDescription(treatPower)
                .AddToDB();

            // make summon effect description
            EffectDescription cookTreatsEffect = BuildEffectDescriptionSummonForm(RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                treat, 5);

            // make power using summon effect to make treats
            var treatCookPresentation = GuiPresentationBuilder.Build(
                "ProfChefTreatCook", Category.Feat, PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            FeatureDefinitionPower cookTreatsPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Hours1, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Wisdom, cookTreatsEffect,
                "FeatChefCookTreats", treatCookPresentation);

            // short rest activated ability to heal 1d8 (limit number of times this can be done)
            var shortRestFeastPresentation = GuiPresentationBuilder.Build(
                "ChefShortRestFeast", Category.Feat, PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            EffectDescription shortRestFeastEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Distance, 10,
                RuleDefinitions.TargetType.Individuals, 4, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.No, RuleDefinitions.LevelSourceType.CharacterLevel, false, 0, RuleDefinitions.DieType.D8, 1, 1);
            FeatureDefinitionPower shortRestFeast = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Hours1, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, shortRestFeastEffect,
                "FeatChefShortRestFeast", shortRestFeastPresentation);

            FeatureDefinition conIncrement = BuildAdditiveAttributeModifier("FeatChefConIncrement", AttributeDefinitions.Constitution, 1);
            FeatureDefinition wisIncrement = BuildAdditiveAttributeModifier("FeatChefWisIncrement", AttributeDefinitions.Wisdom, 1);

            feats.Add(FeatDefinitionBuilder
                .Create("FeatChefCon", HealingFeatNamespace)
                .SetFeatures(conIncrement, shortRestFeast, cookTreatsPower)
                .SetGuiPresentation("ChefCon", Category.Feat)
                .AddToDB());

            feats.Add(FeatDefinitionBuilder
                .Create("FeatChefWis", HealingFeatNamespace)
                .SetFeatures(wisIncrement, shortRestFeast, cookTreatsPower)
                .SetGuiPresentation("ChefWis", Category.Feat)
                .AddToDB());

            static FeatureDefinitionAttributeModifier BuildAdditiveAttributeModifier(string name, string attribute, int amount)
            {
                return FeatureDefinitionAttributeModifierBuilder
                    .Create(name, HealingFeatNamespace)
                    .SetGuiPresentation(Category.Feat)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive, attribute, amount)
                    .AddToDB();
            }
        }

        private static FeatureDefinitionPower BuildPowerFromEffectDescription(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
          string usesAbilityScoreName,
          RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
          bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
          EffectDescription effectDescription, string name, GuiPresentation guiPresentation)
        {
            return FeatureDefinitionPowerBuilder
                .Create(name, HealingFeatNamespace)
                .SetGuiPresentation(guiPresentation)
                .Configure(
                    usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge, proficiencyBonusToAttack,
                    abilityScoreBonusToAttack, abilityScore, effectDescription, false /* unique instance */)
                .AddToDB();
        }

        private static EffectDescription BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
            RuleDefinitions.TargetType targetType, int targetParameter,
            RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect,
            EffectForm.LevelApplianceType applyLevel, RuleDefinitions.LevelSourceType levelType, bool applyAbilityBonus,
            int bonusHitPoints, RuleDefinitions.DieType dieType, int diceNumber, int levelMultiplier)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetDurationData(durationType, durationParameter, endOfEffect);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetTempHPForm(bonusHitPoints, dieType, diceNumber);
            effectFormBuilder.SetLevelAdvancement(applyLevel, levelType, levelMultiplier);
            if (applyAbilityBonus)
            {
                effectFormBuilder.SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);
            }
            effectFormBuilder.CreatedByCharacter();

            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return effectDescriptionBuilder.Build();
        }

        private static EffectDescription BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
            RuleDefinitions.TargetType targetType, int targetParameter,
            RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect,
            EffectForm.LevelApplianceType applyLevel, RuleDefinitions.LevelSourceType levelType, bool applyAbilityBonus,
            int bonusHitPoints, RuleDefinitions.DieType dieType, int diceNumber, int levelMultiplier)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetDurationData(durationType, durationParameter, endOfEffect);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetHealingForm(RuleDefinitions.HealingComputation.Dice, bonusHitPoints, dieType, diceNumber, false, RuleDefinitions.HealingCap.MaximumHitPoints);
            effectFormBuilder.SetLevelAdvancement(applyLevel, levelType, levelMultiplier);
            if (applyAbilityBonus)
            {
                effectFormBuilder.SetBonusMode(RuleDefinitions.AddBonusMode.AbilityBonus);
            }
            effectFormBuilder.CreatedByCharacter();

            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return effectDescriptionBuilder.Build();
        }

        private static EffectDescription BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
            RuleDefinitions.TargetType targetType, int targetParameter,
            RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect,
            int secondsSinceDeath)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetTargetFiltering(RuleDefinitions.TargetFilteringMethod.CharacterOnly, RuleDefinitions.TargetFilteringTag.No, 5, RuleDefinitions.DieType.D8);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetDurationData(durationType, durationParameter, endOfEffect);
            effectDescriptionBuilder.SetRequiredCondition(DatabaseHelper.ConditionDefinitions.ConditionDead);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetReviveForm(secondsSinceDeath, RuleDefinitions.ReviveHitPoints.One, new List<ConditionDefinition>());

            effectFormBuilder.CreatedByCharacter();

            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return effectDescriptionBuilder.Build();
        }

        private static EffectDescription BuildEffectDescriptionSummonForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
            RuleDefinitions.TargetType targetType, int targetParameter,
            RuleDefinitions.DurationType durationType, int durationParameter, RuleDefinitions.TurnOccurenceType endOfEffect,
            ItemDefinition item, int number)
        {
            EffectDescriptionBuilder effectDescriptionBuilder = new EffectDescriptionBuilder();
            effectDescriptionBuilder.SetTargetingData(RuleDefinitions.Side.Ally, rangeType, rangeParameter, targetType, targetParameter, 0, ActionDefinitions.ItemSelectionType.None);
            effectDescriptionBuilder.SetCreatedByCharacter();
            effectDescriptionBuilder.SetDurationData(durationType, durationParameter, endOfEffect);

            EffectFormBuilder effectFormBuilder = new EffectFormBuilder();
            effectFormBuilder.SetSummonForm(SummonForm.Type.InventoryItem, item, number, "", null, true, null, ScriptableObject.CreateInstance<EffectProxyDefinition>());

            effectFormBuilder.CreatedByCharacter();

            effectDescriptionBuilder.AddEffectForm(effectFormBuilder.Build());
            effectDescriptionBuilder.SetEffectAdvancement(RuleDefinitions.EffectIncrementMethod.None, 1,
                0, 0, 0, 0, 0, 0, 0, 0, RuleDefinitions.AdvancementDuration.None);

            EffectParticleParameters particleParams = new EffectParticleParameters();
            particleParams.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription.EffectParticleParameters);
            effectDescriptionBuilder.SetParticleEffectParameters(particleParams);

            return effectDescriptionBuilder.Build();
        }
    }
}
