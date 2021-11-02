using SolastaCommunityExpansion.Features;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SolastaCommunityExpansion.Feats
{
    class HealingFeats
    {
        public static Guid HealingFeatNamespace = new Guid("501448fd-3c84-4031-befe-84c2ae75123b");

        public static void CreateFeats(List<FeatDefinition> feats)
        {
            // Inspiring Leader- prereq charisma 13, spend 10 minutes inspiring folks to fit temp hp == level + charisma modifier (1/short rest)
            GuiPresentationBuilder inspringLeaderPresentation = new GuiPresentationBuilder(
                "Feat/&InspiringLeaderDescription",
                "Feat/&InspiringLeaderTitle");
            inspringLeaderPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarGoldenSpeech.GuiPresentation.SpriteReference);

            EffectDescription inspriringEffect = BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType.Distance, 10,
                RuleDefinitions.TargetType.Individuals, 6, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.Add, RuleDefinitions.LevelSourceType.CharacterLevel, true, 0, RuleDefinitions.DieType.D1, 0, 1);

            FeatureDefinitionPower inspiringPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Charisma, RuleDefinitions.ActivationTime.Minute10, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Charisma, inspriringEffect,
                "PowerInspiringLeaderFeat", inspringLeaderPresentation.Build());

            FeatDefinitionBuilder inspiringLeader = new FeatDefinitionBuilder("FeatInspiringLeader", GuidHelper.Create(HealingFeatNamespace, "FeatInspiringLeader").ToString(),
                new List<FeatureDefinition>()
            {
                inspiringPower,
            }, inspringLeaderPresentation.Build());
            inspiringLeader.SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13);
            feats.Add(inspiringLeader.AddToDB());

            // Healer- use a healer's kit to stabilize to 1hp, use an action to restore 1d6+4 hp, plus additional hp equal to creature's level (can only heal a given creature once per day)
            // thoughts- grant a stabilize power that sets them to 1hp (unlimited uses), prof per day a heal that does 1d6+4+"caster" level
            GuiPresentationBuilder healerPresentation = new GuiPresentationBuilder(
                "Feat/&HealerDescription",
                "Feat/&HealerTitle");
            healerPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            GuiPresentationBuilder medKitPresentation = new GuiPresentationBuilder(
                "Feat/&HealerUseMedicineDescription",
                "Feat/&HealerUseMedicineTitle");
            medKitPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            EffectDescription medKitEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Touch, 1,
                RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.Add, RuleDefinitions.LevelSourceType.CharacterLevel, false, 4, RuleDefinitions.DieType.D6, 1, 1);

            FeatureDefinitionPower medKitPower = BuildPowerFromEffectDescription(0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, medKitEffect,
                "PowerMedKitHealerFeat", medKitPresentation.Build());


            GuiPresentationBuilder resuscitatePresentation = new GuiPresentationBuilder(
                "Feat/&HealerResuscitateDescription",
                "Feat/&HealerResuscitateTitle");
            resuscitatePresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

            EffectDescription resuscitateEffect = BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType.Touch, 1,
                RuleDefinitions.TargetType.Individuals, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                12 /* seconds since death */);

            FeatureDefinitionPower resuscitatePower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Wisdom, resuscitateEffect,
                "PowerResuscitateHealerFeat", resuscitatePresentation.Build());

            GuiPresentationBuilder stabalizePresentation = new GuiPresentationBuilder(
                "Feat/&HealerStabalizeDescription",
                "Feat/&HealerStabalizeTitle");
            stabalizePresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainLifePreserveLife.GuiPresentation.SpriteReference);

            FeatureDefinitionPower stabalizePower = BuildPowerFromEffectDescription(0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Action, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, DatabaseHelper.SpellDefinitions.SpareTheDying.EffectDescription,
                "PowerStabalizeHealerFeat", stabalizePresentation.Build());

            GuiPresentationBuilder medicineExpertisePresentation = new GuiPresentationBuilder(
                "Feat/&ProfHealerMedicineDescription",
                "Feat/&ProfHealerMedicineTitle");
            FeatureDefinition medicineKnowledge = BuildProficiency(RuleDefinitions.ProficiencyType.SkillOrExpertise, new List<string>()
            {
               SkillDefinitions.Medecine,
            }, "FeatHealerMedicineProficiency", medicineExpertisePresentation.Build()
            );



            FeatDefinitionBuilder healer = new FeatDefinitionBuilder("FeatHealer", GuidHelper.Create(HealingFeatNamespace, "FeatHealer").ToString(),
                new List<FeatureDefinition>()
            {
                medicineKnowledge,
                medKitPower,
                resuscitatePower,
                stabalizePower,
            }, healerPresentation.Build());
            feats.Add(healer.AddToDB());


            // Chef: con/wis, short rest ability, everyone regains 1d8 (supposed to be only if they spend hit dice)
            //     once per long rest cook treats that grant temp hp (prof bonus # treats and #thp)

            // define power(s) that treats have
            GuiPresentationBuilder treatEatPresentation = new GuiPresentationBuilder(
                "Feat/&ProfChefTreatActionDescription",
                "Feat/&ProfChefTreatActionTitle");
            treatEatPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionGoodberryHealing.GuiPresentation.SpriteReference);
            EffectDescription treatEffect = BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.No, RuleDefinitions.LevelSourceType.CharacterLevel, false, 5, RuleDefinitions.DieType.D1, 0, 1);
            FeatureDefinitionPower treatPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Wisdom,
                RuleDefinitions.ActivationTime.BonusAction, 1, RuleDefinitions.RechargeRate.None, false, false, AttributeDefinitions.Wisdom,
                treatEffect, "ChefTreatEatPower", treatEatPresentation.Build());

            // define treats
            GuiPresentationBuilder treatPresentation = new GuiPresentationBuilder(
                "Feat/&ProfChefTreatDescription",
                "Feat/&ProfChefTreatTitle");
            treatPresentation.SetSpriteReference(DatabaseHelper.ItemDefinitions.Berry_Ration.GuiPresentation.SpriteReference);
            ItemDefinition treat = ItemBuilder.CopyFromItemSetFunctions(HealingFeatNamespace, new List<FeatureDefinitionPower>() { treatPower }, DatabaseHelper.ItemDefinitions.Berry_Ration,
                "ChefSnackTreat", treatPresentation.Build());

            // make summon effect description
            EffectDescription cookTreatsEffect = BuildEffectDescriptionSummonForm(RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self, 1, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                treat, 5);

            // make power using summon effect to make treats
            GuiPresentationBuilder treatCookPresentation = new GuiPresentationBuilder(
                "Feat/&ProfChefTreatCookDescription",
                "Feat/&ProfChefTreatCookTitle");
            treatEatPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);
            FeatureDefinitionPower cookTreatsPower = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Hours1, 1, RuleDefinitions.RechargeRate.LongRest,
                false, false, AttributeDefinitions.Wisdom, cookTreatsEffect,
                "FeatChefCookTreats", treatCookPresentation.Build());

            // short rest activated ability to heal 1d8 (limit number of times this can be done)
            GuiPresentationBuilder shortRestFeastPresentation = new GuiPresentationBuilder(
                "Feat/&ChefShortRestFeastDescription",
                "Feat/&ChefShortRestFeastTitle");
            inspringLeaderPresentation.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionGoodberryHealingOther.GuiPresentation.SpriteReference);

            EffectDescription shortRestFeastEffect = BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType.Distance, 10,
                RuleDefinitions.TargetType.Individuals, 4, RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.EndOfTurn,
                EffectForm.LevelApplianceType.No, RuleDefinitions.LevelSourceType.CharacterLevel, false, 0, RuleDefinitions.DieType.D8, 1, 1);
            FeatureDefinitionPower shortRestFeast = BuildPowerFromEffectDescription(1, RuleDefinitions.UsesDetermination.Fixed,
                AttributeDefinitions.Wisdom, RuleDefinitions.ActivationTime.Hours1, 1, RuleDefinitions.RechargeRate.ShortRest,
                false, false, AttributeDefinitions.Wisdom, shortRestFeastEffect,
                "FeatChefShortRestFeast", shortRestFeastPresentation.Build());

            //RestActivityBuilder.BuildRestActivity(RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.ShortRest,
            //    RestActivityDefinition.ActivityCondition.CanUsePower, "UsePower", shortRestFeast.Name, "ChefCookTreats", shortRestFeastPresentation.Build());

            GuiPresentationBuilder conPresentation = new GuiPresentationBuilder(
                "Feat/&FeatChefConIncrementDescription",
                "Feat/&FeatChefConIncrementTitle");
            FeatureDefinition conIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Constitution, 1, "FeatChefConIncrement", conPresentation.Build());

            GuiPresentationBuilder wisPresentation = new GuiPresentationBuilder(
                "Feat/&FeatChefWisIncrementDescription",
                "Feat/&FeatChefWisIncrementTitle");
            FeatureDefinition wisIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Wisdom, 1, "FeatChefWisIncrement", wisPresentation.Build());

            GuiPresentationBuilder chefConPresentation = new GuiPresentationBuilder(
                "Feat/&ChefConDescription",
                "Feat/&ChefConTitle");
            FeatDefinitionBuilder chefCon = new FeatDefinitionBuilder("FeatChefCon", GuidHelper.Create(HealingFeatNamespace, "FeatChefCon").ToString(),
                new List<FeatureDefinition>()
            {
                conIncrement,
                shortRestFeast,
                cookTreatsPower,
            }, chefConPresentation.Build());
            feats.Add(chefCon.AddToDB());

            GuiPresentationBuilder chefWisPresentation = new GuiPresentationBuilder(
                "Feat/&ChefWisDescription",
                "Feat/&ChefWisTitle");
            FeatDefinitionBuilder chefWis = new FeatDefinitionBuilder("FeatChefWis", GuidHelper.Create(HealingFeatNamespace, "FeatChefWis").ToString(),
                new List<FeatureDefinition>()
            {
                wisIncrement,
                shortRestFeast,
                cookTreatsPower,
            }, chefWisPresentation.Build());
            feats.Add(chefWis.AddToDB());
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
            string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttributeModifierBuilder builder = new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(HealingFeatNamespace, name).ToString(),
                modifierType, attribute, amount, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionProficiencyBuilder builder = new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(HealingFeatNamespace, name).ToString(), type, proficiencies, guiPresentation);
            return builder.AddToDB();
        }


        public static FeatureDefinitionPower BuildPowerFromEffectDescription(int usesPerRecharge, RuleDefinitions.UsesDetermination usesDetermination,
          string usesAbilityScoreName,
          RuleDefinitions.ActivationTime activationTime, int costPerUse, RuleDefinitions.RechargeRate recharge,
          bool proficiencyBonusToAttack, bool abilityScoreBonusToAttack, string abilityScore,
          EffectDescription effectDescription, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionPowerBuilder builder = new FeatureDefinitionPowerBuilder(name, GuidHelper.Create(HealingFeatNamespace, name).ToString(),
                usesPerRecharge, usesDetermination, usesAbilityScoreName, activationTime, costPerUse, recharge, proficiencyBonusToAttack,
                abilityScoreBonusToAttack, abilityScore,
                effectDescription, guiPresentation, false /* unique instance */);
            return builder.AddToDB();
        }

        public static EffectDescription BuildEffectDescriptionTempHPForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
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

        public static EffectDescription BuildEffectDescriptionHealingForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
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

        public static EffectDescription BuildEffectDescriptionReviveForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
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

        public static EffectDescription BuildEffectDescriptionSummonForm(RuleDefinitions.RangeType rangeType, int rangeParameter,
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
