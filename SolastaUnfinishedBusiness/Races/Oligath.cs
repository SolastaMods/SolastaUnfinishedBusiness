using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
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

        var damageAffinityOligathHotBlooded = FeatureDefinitionFeatureSetBuilder
            .Create($"DamageAffinity{Name}HotBlooded")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionConditionAffinitys.ConditionAffinityWeatherFrozenImmunity,
                FeatureDefinitionConditionAffinitys.ConditionAffinityWeatherChilledImmunity)
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
        var powerOligathStoneEndurance = FeatureDefinitionPowerBuilder
            .Create("PowerOligathStoneEndurance")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        var conditionOligathStoneEndurance = ConditionDefinitionBuilder
            .Create($"Condition{Name}StoneEndurance")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.Attacked)
            .AddToDB();

        var reduceDamageOligathStoneEndurance = FeatureDefinitionReduceDamageBuilder
            .Create($"ReduceDamage{Name}StoneEndurance")
            .SetGuiPresentation($"Power{Name}StoneEndurance", Category.Feature)
            .SetAlwaysActiveReducedDamage((attacker, defender) =>
            {
                var rulesetDefender = defender.RulesetCharacter;

                if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
                {
                    return 0;
                }

                var usableCondition =
                    rulesetDefender.AllConditions.FirstOrDefault(x =>
                        x.ConditionDefinition == conditionOligathStoneEndurance);

                if (usableCondition == null)
                {
                    return 0;
                }

                var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
                var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);

                var result = rulesetDefender.RollDie(
                    DieType.D12, RollContext.None, false, AdvantageType.None, out _, out _);

                var totalReducedDamage = result + constitutionModifier;

                if (totalReducedDamage < 0)
                {
                    totalReducedDamage = 0;
                }

                return totalReducedDamage;
            })
            .AddToDB();

        conditionOligathStoneEndurance.Features.Add(reduceDamageOligathStoneEndurance);

        powerOligathStoneEndurance
            .AddCustomSubFeatures(
                new AttackBeforeHitConfirmedOnMeStoneEndurance(
                    powerOligathStoneEndurance,
                    conditionOligathStoneEndurance));

        return powerOligathStoneEndurance;
    }

    private class AttackBeforeHitConfirmedOnMeStoneEndurance(
        FeatureDefinitionPower featureDefinitionPower,
        ConditionDefinition conditionDefinition)
        :
            IAttackBeforeHitConfirmedOnMe, IMagicalAttackBeforeHitConfirmedOnMe
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnMe(GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
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
                yield return HandlePowerStoneEndurance(me);
            }
        }

        public IEnumerator OnMagicalAttackBeforeHitConfirmedOnMe(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier magicModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandlePowerStoneEndurance(defender);
        }

        private IEnumerator HandlePowerStoneEndurance(GameLocationCharacter me)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (!me.IsReactionAvailable())
            {
                yield break;
            }

            var rulesetMe = me.RulesetCharacter;

            // allow stone endurance when prone
            if (rulesetMe is not { IsDeadOrUnconscious: false }
                || rulesetMe.HasConditionOfTypeOrSubType(ConditionIncapacitated)
                || rulesetMe.HasConditionOfTypeOrSubType(ConditionStunned)
                || rulesetMe.HasConditionOfTypeOrSubType(ConditionParalyzed))
            {
                yield break;
            }

            if (!rulesetMe.CanUsePower(featureDefinitionPower))
            {
                yield break;
            }


            var usablePower = UsablePowersProvider.Get(featureDefinitionPower, rulesetMe);
            var reactionParams = new CharacterActionParams(me, (Id)ExtraActionId.DoNothingReaction)
            {
                UsablePower = usablePower
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

            rulesetMe.UsePower(usablePower);
            rulesetMe.InflictCondition(
                conditionDefinition.Name,
                conditionDefinition.DurationType,
                conditionDefinition.DurationParameter,
                conditionDefinition.TurnOccurence,
                AttributeDefinitions.TagEffect,
                rulesetMe.Guid,
                rulesetMe.CurrentFaction.Name,
                1,
                conditionDefinition.Name,
                0,
                0,
                0);
        }
    }
}
