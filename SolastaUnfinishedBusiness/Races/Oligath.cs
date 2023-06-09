using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;


namespace SolastaUnfinishedBusiness.Races;

internal static class RaceOligathBuilder
{
    internal static CharacterRaceDefinition RaceOligath { get; } = BuildOligath();

    [NotNull]
    private static CharacterRaceDefinition BuildOligath()
    {
        var oligathSpriteReference = Sprites.GetSprite("Bolgrif", Resources.Bolgrif, 1024, 512);

        var attributeModifierOligathStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierOligathStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var attributeModifierOligathConstitutionAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierOligathConstitutionAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 1)
            .AddToDB();

        var equipmentAffinityOligathPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(EquipmentAffinityFeatHauler, "EquipmentAffinityOligathPowerfulBuild")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyOligathLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyOligathLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Giant")
            .AddToDB();

        var proficiencyOligathNaturalAthlete = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyOligathNaturalAthlete")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Athletics)
            .AddToDB();

        var damageAffinityOligathHotBlooded = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityOligathHotBlooded")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeCold)
            .AddToDB();

        var powerOligathStoneEndurance = BuildPowerOligathStoneEndurance();

        var raceOligath = CharacterRaceDefinitionBuilder
            .Create(Human, "RaceOligath")
            .SetGuiPresentation(Category.Race, oligathSpriteReference)
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(Elf.RacePresentation.DeepCopy())
            .SetBaseWeight(35)
            .SetBaseHeight(3)
            .SetMinimalAge(6)
            .SetMaximalAge(120)
            .SetFeaturesAtLevel(1,
                attributeModifierOligathStrengthAbilityScoreIncrease,
                attributeModifierOligathConstitutionAbilityScoreIncrease,
                proficiencyOligathNaturalAthlete,
                powerOligathStoneEndurance,
                equipmentAffinityOligathPowerfulBuild,
                damageAffinityOligathHotBlooded,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionMoveModes.MoveModeMove6,
                proficiencyOligathLanguages)
            .AddToDB();
        RacesContext.RaceScaleMap[raceOligath] = 7.6f / 6.4f;

        return raceOligath;
    }

    private static FeatureDefinition BuildPowerOligathStoneEndurance()
    {
        var powerOligathStoneEndurance = FeatureDefinitionPowerBuilder
            .Create("PowerOligathStoneEndurance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Reaction, RechargeRate.LongRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerOligathStoneEndurance
            .SetCustomSubFeatures(new PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance(powerOligathStoneEndurance));
        return powerOligathStoneEndurance;
    }

    private class PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance : IPhysicalAttackBeforeHitConfirmedOnMe
    {
        private readonly FeatureDefinitionPower featureDefinitionPower;

        public PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance(
            FeatureDefinitionPower featureDefinitionPower)
        {
            this.featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator OnAttackBeforeHitConfirmed(GameLocationBattleManager battle,
            GameLocationCharacter attacker, 
            GameLocationCharacter defender, 
            ActionModifier attackModifier, 
            RulesetAttackMode attackMode,
            bool rangedAttack, 
            AdvantageType advantageType, 
            List<EffectForm> actualEffectForms, 
            RulesetEffect rulesetEffect, 
            bool criticalHit, 
            bool firstTarget)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null || rulesetDefender == null)
            {
                yield break;
            }

            if (rulesetDefender.GetRemainingPowerCharges(featureDefinitionPower) <= 0)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter =
                        Gui.Format("Reaction/&CustomReactionStoneEnduranceDescription")
                };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("StoneEndurance", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var constitution = (rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution) - 10) / 2;
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);

            var result = rulesetDefender.RollDie(DieType.D12, RollContext.None, false,
                AdvantageType.None, out _, out _);

            var damageReduction = result + constitutionModifier;


            if (damageReduction < 0)
            {
                damageReduction = 0;
            }

            attackModifier.damageRollReduction = damageReduction;
            GameConsoleHelper.LogCharacterUsedPower(rulesetDefender, featureDefinitionPower);
            rulesetDefender.UpdateUsageForPower(featureDefinitionPower, featureDefinitionPower.CostPerUse);
        }
    }
}
