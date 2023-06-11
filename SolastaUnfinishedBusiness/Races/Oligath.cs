using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;


namespace SolastaUnfinishedBusiness.Races;

internal static class RaceOligathBuilder
{
    private const string Name = "Oligath";

    internal static CharacterRaceDefinition RaceOligath { get; } = BuildOligath();

    [NotNull]
    private static CharacterRaceDefinition BuildOligath()
    {
        var attributeModifierOligathStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}StrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 2)
            .AddToDB();

        var attributeModifierOligathConstitutionAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ConstitutionAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Constitution, 1)
            .AddToDB();

        var equipmentAffinityOligathPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(EquipmentAffinityFeatHauler, $"EquipmentAffinity{Name}PowerfulBuild")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var proficiencyOligathLanguages = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Languages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Giant")
            .AddToDB();

        var proficiencyOligathNaturalAthlete = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}NaturalAthlete")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Skill, SkillDefinitions.Athletics)
            .AddToDB();

        var damageAffinityOligathHotBlooded = FeatureDefinitionDamageAffinityBuilder
            .Create($"DamageAffinity{Name}HotBlooded")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeCold)
            .AddToDB();

        var powerOligathStoneEndurance = BuildPowerOligathStoneEndurance();

        var raceOligath = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Oligath, 1024, 512))
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
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerOligathStoneEndurance
            .SetCustomSubFeatures(new PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance(powerOligathStoneEndurance));

        return powerOligathStoneEndurance;
    }

    private class PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance :
        IPhysicalAttackBeforeHitConfirmedOnMe, IMagicalAttackInitiated
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public PhysicalAttackBeforeHitConfirmedOnMeStoneEndurance(
            FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
        }

        public IEnumerator OnMagicalAttackInitiated(GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandlePowerStoneEndurance(defender, magicModifier);
        }

        public IEnumerator OnAttackBeforeHitConfirmed(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            yield return HandlePowerStoneEndurance(me, attackModifier);
        }

        private IEnumerator HandlePowerStoneEndurance(GameLocationCharacter me, ActionModifier attackModifier)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            if (rulesetMe is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            if (rulesetMe.GetRemainingPowerCharges(_featureDefinitionPower) <= 0)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(me, (Id)ExtraActionId.DoNothingReaction)
            {
                StringParameter = Gui.Format("Reaction/&CustomReactionStoneEnduranceDescription")
            };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("StoneEndurance", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                me, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var totalReducedDamage = CalculateReducedDamage(rulesetMe);

            attackModifier.DamageRollReduction += totalReducedDamage;

            GameConsoleHelper.LogCharacterUsedPower(rulesetMe, _featureDefinitionPower);
            rulesetMe.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);
            rulesetMe.DamageReduced(rulesetMe, _featureDefinitionPower, totalReducedDamage);
        }

        private static int CalculateReducedDamage(RulesetActor rulesetDefender, int damageAmount = -1)
        {
            var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);

            var result = rulesetDefender.RollDie(
                DieType.D12, RollContext.None, false, AdvantageType.None, out _, out _);

            var totalReducedDamage = result + constitutionModifier;

            if (totalReducedDamage < 0)
            {
                totalReducedDamage = 0;
            }

            if (damageAmount > 0 && damageAmount < totalReducedDamage)
            {
                totalReducedDamage = damageAmount;
            }

            return totalReducedDamage;
        }
    }
}
