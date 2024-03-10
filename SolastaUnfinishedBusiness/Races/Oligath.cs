using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
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

        // chilled and frozen immunities are handled by srd house rules now
        var damageAffinityOligathHotBlooded = FeatureDefinitionFeatureSetBuilder
            .Create($"DamageAffinity{Name}HotBlooded")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance)
            .AddToDB();

        var powerOligathStoneEndurance = BuildPowerOligathStoneEndurance();

        var raceOligath = CharacterRaceDefinitionBuilder
            .Create(Human, $"Race{Name}")
            .SetGuiPresentation(Category.Race, Sprites.GetSprite(Name, Resources.Oligath, 1024, 512))
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(Elf.RacePresentation.DeepCopy())
            .SetBaseHeight(90)
            .SetBaseWeight(310)
            .SetMinimalAge(18)
            .SetMaximalAge(80)
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

    private static FeatureDefinitionPower BuildPowerOligathStoneEndurance()
    {
        var reduceDamageOligathStoneEndurance = FeatureDefinitionReduceDamageBuilder
            .Create($"ReduceDamage{Name}StoneEndurance")
            .SetGuiPresentation($"Power{Name}StoneEndurance", Category.Feature)
            .SetAlwaysActiveReducedDamage((attacker, defender) =>
            {
                var rulesetDefender = defender.RulesetCharacter;

                if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                    !rulesetDefender.HasConditionOfType($"Condition{Name}StoneEndurance"))
                {
                    return 0;
                }

                var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
                var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
                var dieRoll = rulesetDefender.RollDie(
                    DieType.D12, RollContext.None, false, AdvantageType.None, out _, out _);

                var totalReducedDamage = dieRoll + constitutionModifier;

                if (totalReducedDamage < 0)
                {
                    totalReducedDamage = 0;
                }

                return totalReducedDamage;
            })
            .AddToDB();

        var conditionOligathStoneEndurance = ConditionDefinitionBuilder
            .Create($"Condition{Name}StoneEndurance")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(reduceDamageOligathStoneEndurance)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var powerOligathStoneEndurance = FeatureDefinitionPowerBuilder
            .Create("PowerOligathStoneEndurance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionOligathStoneEndurance))
                    .Build())
            .AddToDB();

        powerOligathStoneEndurance.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new AttackBeforeHitConfirmedOnMeStoneEndurance(powerOligathStoneEndurance));

        return powerOligathStoneEndurance;
    }

    private class AttackBeforeHitConfirmedOnMeStoneEndurance(FeatureDefinitionPower powerStoneEndurance)
        : IAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rulesetEffect == null)
            {
                yield return HandlePowerStoneEndurance(attacker, defender);
            }
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandlePowerStoneEndurance(attacker, defender);
        }

        private IEnumerator HandlePowerStoneEndurance(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            // don't use CanReact() to allow stone endurance when prone
            if (!defender.IsReactionAvailable() ||
                rulesetDefender is not { IsDeadOrUnconscious: false } ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionIncapacitated) ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionStunned) ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionParalyzed) ||
                rulesetDefender.GetRemainingPowerUses(powerStoneEndurance) == 0)
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerStoneEndurance, rulesetDefender);
            var actionParams = new CharacterActionParams(defender, Id.PowerReaction)
            {
                StringParameter = "StoneEndurance",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };

            var count = gameLocationActionService.PendingReactionRequestGroups.Count;

            gameLocationActionService.ReactToUsePower(actionParams, "UsePower", defender);

            yield return gameLocationBattleService.WaitForReactions(attacker, gameLocationActionService, count);
        }
    }
}
