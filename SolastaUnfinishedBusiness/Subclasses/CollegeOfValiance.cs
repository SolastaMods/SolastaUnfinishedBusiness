using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSubclassChoices;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class CollegeOfValiance : AbstractSubclass
{
    private const string Name = "CollegeOfValiance";
    private static FeatureDefinition _featureSteadfastInspiration;

    public CollegeOfValiance()
    {
        // Captivating Presence

        var featureCaptivatingPresence = FeatureDefinitionBuilder
            .Create($"Feature{Name}CaptivatingPresence")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ModifyAbilityCheckCaptivatingPresence())
            .AddToDB();

        // Disheartening Performance

        var conditionDishearteningPerformance = ConditionDefinitionBuilder
            .Create($"Condition{Name}DishearteningPerformance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBaned)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            //.SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .AddToDB();

        conditionDishearteningPerformance.AddCustomSubFeatures(
            new RollSavingThrowFinishedDishearteningPerformance(conditionDishearteningPerformance));

        var powerSteadfastDishearteningPerformance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DishearteningPerformance")
            .SetGuiPresentation(Category.Feature, PowerPatronFiendDarkOnesOwnLuck)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.BardicInspiration)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 10, TargetType.Individuals)
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
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.UseHeroicInspiration)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.CollegeOfValiance, 256))
            .AddFeaturesAtLevel(3, featureCaptivatingPresence, powerSteadfastDishearteningPerformance)
            .AddFeaturesAtLevel(6, autoPreparedSpellsRecallLanguage, _featureSteadfastInspiration)
            .AddFeaturesAtLevel(14, actionAffinityHeroicInspiration, powerHeroicInspiration)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Bard;

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
        public void MinRoll(
            RulesetCharacter character,
            int baseBonus,
            string abilityScoreName,
            string proficiencyName,
            List<TrendInfo> advantageTrends,
            List<TrendInfo> modifierTrends,
            ref int rollModifier,
            ref int minRoll)
        {
            if (abilityScoreName == AttributeDefinitions.Charisma &&
                proficiencyName is SkillDefinitions.Deception or SkillDefinitions.Persuasion)
            {
                minRoll = Math.Max(minRoll, 10);
            }
        }
    }

    private sealed class RollSavingThrowFinishedDishearteningPerformance(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDishearteningPerformance) : IRollSavingThrowFinished
    {
        public void OnSavingThrowFinished(
            RulesetCharacter caster,
            RulesetCharacter defender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            // no need to check for source guid here
            if (!defender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDishearteningPerformance.Name, out var activeCondition))
            {
                return;
            }

            var bardCharacter = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            defender.RemoveCondition(activeCondition);

            if (bardCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                return;
            }

            var dieType = bardCharacter.GetBardicInspirationDieValue();
            var inspirationDie = RollDie(dieType, AdvantageType.None, out var r1, out var r2);

            outcomeDelta -= inspirationDie;

            if (outcomeDelta < 0)
            {
                outcome = RollOutcome.Failure;
            }

            var baseLine = outcome == RollOutcome.Failure
                ? "Feedback/&DishearteningPerformanceUsedSuccessLine"
                : "Feedback/&DishearteningPerformanceUsedFailureLine";

            bardCharacter.ShowDieRoll(dieType, r1, r2, advantage: AdvantageType.None,
                title: conditionDishearteningPerformance.GuiPresentation.Title);
            bardCharacter.LogCharacterActivatesAbility(
                Gui.NoLocalization, baseLine, true,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, inspirationDie.ToString())
                ]);
        }
    }
}
