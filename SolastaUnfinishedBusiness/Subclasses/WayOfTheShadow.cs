using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfShadow : AbstractSubclass
{
    private const string Name = "WayOfTheShadow";

    internal const string ConditionCloakOfShadowsName = $"Condition{Name}CloakOfShadows";

    public WayOfShadow()
    {
        // LEVEL 03

        // Shadow Arts

        var powerDarkness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkness")
            .SetGuiPresentation(Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 3)
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeDarkness())
            .AddToDB();

        var featureSetShadowArts = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ShadowArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(SenseDarkvision, powerDarkness)
            .AddToDB();

        // LEVEL 06

        // Shadow Step

        var conditionShadowsStep = ConditionDefinitionBuilder
            .Create($"Condition{Name}ShadowsStep")
            .SetGuiPresentation($"Power{Name}ShadowsStep", Category.Feature,
                ConditionHeraldOfBattle)
            .SetPossessive()
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}ShadowsStep")
                    .SetGuiPresentation($"Condition{Name}ShadowsStep", Category.Condition, Gui.NoLocalization)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .AddToDB();

        conditionShadowsStep.GuiPresentation.description = Gui.EmptyContent;

        var powerShadowsStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowsStep")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerSilhouetteStep, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.TeleportToDestination),
                        EffectFormBuilder.AddConditionForm(conditionShadowsStep, true, true))
                    .SetParticleEffectParameters(PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight),
                new FilterTargetingPositionShadowsStep())
            .AddToDB();

        // LEVEL 11

        // Improved Shadow Step

        var conditionImprovedShadowsStep = ConditionDefinitionBuilder
            .Create(conditionShadowsStep, $"Condition{Name}ImprovedShadowsStep")
            .SetParentCondition(conditionShadowsStep)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create($"AdditionalAction{Name}SwiftBlade")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionType.Bonus)
                    .SetRestrictedActions(Id.AttackOff)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB();

        var powerImprovedShadowsStep = FeatureDefinitionPowerBuilder
            .Create(powerShadowsStep, $"Power{Name}ImprovedShadowsStep")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.TeleportToDestination),
                        EffectFormBuilder.AddConditionForm(conditionImprovedShadowsStep, true, true))
                    .SetParticleEffectParameters(PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddToDB();

        // LEVEL 17

        // Cloak of Shadows

        var conditionCloakOfShadows = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, ConditionCloakOfShadowsName)
            .SetParentCondition(ConditionInvisibleBase)
            .SetPossessive()
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
            .SetSpecialInterruptions(ConditionInterruption.Attacks, ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .AddToDB();

        var powerCloakOfShadows = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CloakOfShadows")
            .SetGuiPresentation(Category.Feature, PowerPatronTimekeeperTimeShift)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCloakOfShadows))
                    .Build())
            .AddToDB();

        powerCloakOfShadows.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight));

        PowerMonkFlurryOfBlows.AddCustomSubFeatures(
            PowerOrSpellFinishedByMeFlurryOfBlows.Marker);
        PowerTraditionFreedomFlurryOfBlowsSwiftStepsImprovement.AddCustomSubFeatures(
            PowerOrSpellFinishedByMeFlurryOfBlows.Marker);
        PowerTraditionFreedomFlurryOfBlowsUnendingStrikesImprovement.AddCustomSubFeatures(
            PowerOrSpellFinishedByMeFlurryOfBlows.Marker);

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3, featureSetShadowArts)
            .AddFeaturesAtLevel(6, powerShadowsStep)
            .AddFeaturesAtLevel(11, powerImprovedShadowsStep)
            .AddFeaturesAtLevel(17, powerCloakOfShadows)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Shadow Arts
    //

    private sealed class PowerOrSpellFinishedByMeDarkness : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.MyExecuteActionCastNoCost(Darkness, 0, action.ActionParams);

            yield break;
        }
    }

    //
    // Shadow Step
    //

    private class FilterTargetingPositionShadowsStep : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            yield return cursorLocationSelectPosition.MyComputeValidPositions(LocationDefinitions.LightingState.Bright);
        }
    }

    //
    // Cloak of Shadows
    //

    private sealed class PowerOrSpellFinishedByMeFlurryOfBlows : IPowerOrSpellFinishedByMe
    {
        internal static readonly PowerOrSpellFinishedByMeFlurryOfBlows Marker = new();

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (!rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionCloakOfShadowsName))
            {
                yield break;
            }

            rulesetCharacter.UsedKiPoints--;
            rulesetCharacter.KiPointsAltered?.Invoke(rulesetCharacter, rulesetCharacter.RemainingKiPoints);
        }
    }
}
