using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
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
                var rulesetDefender = defender.RulesetActor;

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
            .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
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
                    .SetCasterEffectParameters(FeatureDefinitionPowers.PowerTraditionOpenHandWholenessOfBody)
                    .Build())
            .AddToDB();

        powerOligathStoneEndurance.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new CustomBehaviorStoneEndurance(powerOligathStoneEndurance));

        return powerOligathStoneEndurance;
    }

    private class CustomBehaviorStoneEndurance(FeatureDefinitionPower powerStoneEndurance)
        : ITryAlterOutcomeAttack
    {
        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerStoneEndurance, rulesetDefender);

            // don't use CanReact() to allow stone endurance when prone
            if (helper != defender ||
                !defender.IsReactionAvailable() ||
                rulesetDefender is not { IsDeadOrUnconscious: false } ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionIncapacitated) ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionStunned) ||
                rulesetDefender.HasConditionOfTypeOrSubType(ConditionParalyzed) ||
                rulesetDefender.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return defender.MyReactToUsePower(
                Id.PowerReaction,
                usablePower,
                [defender],
                attacker,
                "StoneEndurance",
                battleManager: battleManager);
        }
    }
}
