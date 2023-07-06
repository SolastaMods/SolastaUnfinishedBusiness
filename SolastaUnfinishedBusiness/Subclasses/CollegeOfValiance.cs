using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfValiance : AbstractSubclass
{
    private const string Name = "CollegeOfValiance";

    internal CollegeOfValiance()
    {
        // Captivating Presence

        var featureCaptivatingPresence = FeatureDefinitionBuilder
            .Create($"Feature{Name}CaptivatingPresence")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new ModifyAbilityCheckCaptivatingPresence())
            .AddToDB();

        // Disheartening Performance

        var conditionDishearteningPerformance = ConditionDefinitionBuilder
            .Create($"Condition{Name}DishearteningPerformance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var featureDishearteningPerformance = FeatureDefinitionBuilder
            .Create($"Feature{Name}DishearteningPerformance")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new OnSavingThrowAfterRollDishearteningPerformance(conditionDishearteningPerformance))
            .AddToDB();

        conditionDishearteningPerformance.Features.Add(featureDishearteningPerformance);

        var powerSteadfastDishearteningPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DishearteningPerformance")
            .SetGuiPresentation(Category.Feature, PowerPatronFiendDarkOnesOwnLuck)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 10, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDishearteningPerformance))
                    .Build())
            .AddToDB();

        // LEVEL 06

        // Steadfast Inspiration

        var conditionSteadfastInspiration = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBardicInspiration, $"Condition{Name}SteadfastInspiration")
            .AddToDB();

        var featureSteadfastInspiration = FeatureDefinitionBuilder
            .Create($"Feature{Name}SteadfastInspiration")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures(new UsePowerFinishedByMeSteadfastInspiration(conditionSteadfastInspiration))
            .AddToDB();

        conditionSteadfastInspiration.Features.Add(featureSteadfastInspiration);

        var powerSteadfastInspiration = FeatureDefinitionPowerBuilder
            .Create(PowerBardGiveBardicInspiration, $"Power{Name}SteadfastInspiration")
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerBardGiveBardicInspiration)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionSteadfastInspiration))
                    .Build())
            .SetOverriddenPower(PowerBardGiveBardicInspiration)
            .AddToDB();

        var featureSetSteadfastInspiration = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}SteadfastInspiration")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerSteadfastInspiration)
            .AddToDB();

        // Recall Language

        var autoPreparedSpellsRecallLanguage = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}RecallLanguage")
            .SetGuiPresentation(Category.Feature)
            .SetAutoTag("College")
            .SetSpellcastingClass(CharacterClassDefinitions.Bard)
            .SetPreparedSpellGroups(BuildSpellGroup(6, Tongues))
            .AddToDB();

        // LEVEL 14

        // Heroic Inspiration

        var powerHeroicInspiration = FeatureDefinitionPowerBuilder
            .Create(powerSteadfastInspiration, $"Power{Name}HeroicInspiration")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Charisma)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerBardGiveBardicInspiration)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 10, TargetType.IndividualsUnique, 2)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.TraditionLight)
            .AddFeaturesAtLevel(3, featureCaptivatingPresence, powerSteadfastDishearteningPerformance)
            .AddFeaturesAtLevel(6, featureSetSteadfastInspiration, autoPreparedSpellsRecallLanguage)
            .AddFeaturesAtLevel(14, powerHeroicInspiration)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyAbilityCheckCaptivatingPresence : IModifyAbilityCheck
    {
        public int MinRoll(
            [CanBeNull] RulesetCharacter character,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends)
        {
            if (character == null ||
                abilityScoreName != AttributeDefinitions.Charisma ||
                (proficiencyName != SkillDefinitions.Performance && proficiencyName != SkillDefinitions.Persuasion))
            {
                return 1;
            }

            return 10;
        }
    }

    private sealed class OnSavingThrowAfterRollDishearteningPerformance : IOnSavingThrowAfterRoll
    {
        private readonly ConditionDefinition _conditionDishearteningPerformance;

        public OnSavingThrowAfterRollDishearteningPerformance(ConditionDefinition conditionDishearteningPerformance)
        {
            _conditionDishearteningPerformance = conditionDishearteningPerformance;
        }

        public void OnSavingThrowAfterRoll(
            RulesetCharacter caster,
            Side sourceSide,
            RulesetActor target,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasSavingThrow,
            string savingThrowAbility,
            int saveDC,
            bool disableSavingThrowOnAllies,
            bool advantageForEnemies,
            bool ignoreCover,
            FeatureSourceType featureSourceType,
            List<EffectForm> effectForms,
            List<SaveAffinityBySenseDescription> savingThrowAffinitiesBySense,
            List<SaveAffinityByFamilyDescription> savingThrowAffinitiesByFamily,
            string sourceName,
            BaseDefinition sourceDefinition,
            string schoolOfMagic,
            MetamagicOptionDefinition metamagicOption,
            ref RollOutcome saveOutcome,
            ref int saveOutcomeDelta)
        {
            var usableCondition =
                target.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDishearteningPerformance);

            if (usableCondition == null)
            {
                return;
            }

            if (saveOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                target.RemoveCondition(usableCondition);

                return;
            }

            var bardCharacter = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

            if (bardCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                target.RemoveCondition(usableCondition);

                return;
            }

            var dieType = bardCharacter.GetBardicInspirationDieValue();
            var inspirationDie = RollDie(dieType, AdvantageType.None, out _, out _);
            var baseLine = inspirationDie > saveOutcomeDelta
                ? "Feedback/&DishearteningPerformanceUsedSuccessLine"
                : "Feedback/&DishearteningPerformanceUsedFailureLine";
            var console = Gui.Game.GameConsole;
            var entry = new GameConsoleEntry(baseLine, console.consoleTableDefinition) { Indent = true };

            console.AddCharacterEntry(bardCharacter, entry);
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, Gui.FormatDieTitle(dieType));
            entry.AddParameter(ConsoleStyleDuplet.ParameterType.Positive, inspirationDie.ToString());
            console.AddEntry(entry);

            saveOutcomeDelta -= inspirationDie;

            if (saveOutcomeDelta < 0)
            {
                saveOutcome = RollOutcome.Failure;
            }

            target.RemoveCondition(usableCondition);
        }
    }

    private sealed class UsePowerFinishedByMeSteadfastInspiration : IActionFinishedByMe
    {
        private readonly ConditionDefinition _conditionSteadfastInspiration;

        public UsePowerFinishedByMeSteadfastInspiration(ConditionDefinition conditionSteadfastInspiration)
        {
            _conditionSteadfastInspiration = conditionSteadfastInspiration;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            if (characterAction is not CharacterActionUseBardicInspiration)
            {
                yield break;
            }

            var failedAttackRoll =
                characterAction.AttackRoll > 0 &&
                characterAction.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure;

            var failedAbilityCheck =
                characterAction.AbilityCheckRoll > 0 &&
                characterAction.AbilityCheckRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure;

            var failedSavingThrow =
                characterAction.RolledSaveThrow &&
                characterAction.SaveOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure;

            if (!failedAttackRoll && !failedAbilityCheck && !failedSavingThrow)
            {
                yield break;
            }

            var rulesetCharacter = characterAction.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _conditionSteadfastInspiration.Name,
                _conditionSteadfastInspiration.DurationType,
                _conditionSteadfastInspiration.DurationParameter,
                _conditionSteadfastInspiration.TurnOccurence,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }
}
