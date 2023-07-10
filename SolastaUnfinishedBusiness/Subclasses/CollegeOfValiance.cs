using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class CollegeOfValiance : AbstractSubclass
{
    private const string Name = "CollegeOfValiance";
    private static FeatureDefinition _featureSteadfastInspiration;

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
                    .SetParticleEffectParameters(PowerCollegeLoreCuttingWords)
                    .Build())
            .AddToDB();

        // LEVEL 06

        // Steadfast Inspiration

        _featureSteadfastInspiration = FeatureDefinitionBuilder
            .Create($"Feature{Name}SteadfastInspiration")
            .SetGuiPresentation(Category.Feature)
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
            .Create(PowerBardGiveBardicInspiration, $"Power{Name}HeroicInspiration")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 2)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(PowerBardGiveBardicInspiration)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 10, TargetType.IndividualsUnique, 2)
                    .Build())
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(GrantBardicInspiration, "ActionUseHeroicInspiration")
            .SetOrUpdateGuiPresentation($"Power{Name}HeroicInspiration", Category.Feature)
            .OverrideClassName("UsePower")
            .SetActionId(ExtraActionId.UseHeroicInspiration)
            .SetActivatedPower(powerHeroicInspiration)
            .AddToDB();

        var actionAffinityHeroicInspiration = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}HeroicInspiration")
            .SetGuiPresentationNoContent(true)
            .SetAllowedActionTypes()
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.UseHeroicInspiration)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.TraditionLight)
            .AddFeaturesAtLevel(3, featureCaptivatingPresence, powerSteadfastDishearteningPerformance)
            .AddFeaturesAtLevel(6, autoPreparedSpellsRecallLanguage, _featureSteadfastInspiration)
            .AddFeaturesAtLevel(14, actionAffinityHeroicInspiration, powerHeroicInspiration)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => SubclassChoiceBardColleges;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool IsValianceLevel6(ulong sourceGuid)
    {
        var bardCharacter = EffectHelpers.GetCharacterByGuid(sourceGuid);

        if (bardCharacter == null || bardCharacter.GetSubclassLevel(CharacterClassDefinitions.Bard, Name) <= 5)
        {
            return false;
        }

        bardCharacter.LogCharacterUsedFeature(_featureSteadfastInspiration);

        return true;
    }

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
                return;
            }

            var bardCharacter = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

            if (bardCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            // this is almost the same code as RollBardicInspirationDie but dup here for better combat log messages
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
}
