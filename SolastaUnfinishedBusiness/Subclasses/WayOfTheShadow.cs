using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfShadow : AbstractSubclass
{
    private const string Name = "WayOfTheShadow";

    internal const string ConditionCloakOfShadowsName = $"Condition{Name}CloakOfShadows";

    private const string ConditionDarknessMoveProhibit = $"Condition{Name}DarknessMoveProhibit";

    internal static readonly EffectProxyDefinition EffectProxyDarkness = EffectProxyDefinitionBuilder
        .Create(EffectProxyDefinitions.ProxyDarkness, $"Proxy{Name}Darkness")
        .SetCanMove()
        .SetActionId(ExtraActionId.ProxyDarkness)
        .SetCanMove()
        .SetAdditionalFeatures(FeatureDefinitionMoveModes.MoveModeMove12, FeatureDefinitionMoveModes.MoveModeFly12)
        .AddToDB();

    internal static readonly SpellDefinition SpellDarkness = SpellDefinitionBuilder
        .Create(Darkness, $"Spell{Name}Darkness")
        .SetMaterialComponent(MaterialComponentType.None)
        .AddToDB();

    public WayOfShadow()
    {
        var validateIsNotInBrightLight = new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight);

        // LEVEL 03 - Shadow Arts

        var conditionDarknessMoveTracker = ConditionDefinitionBuilder
            .Create($"Condition{Name}DarknessMoveTracker")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddCustomSubFeatures(new ActionFinishedByMeProxyDarkness())
            .AddToDB();

        var conditionDarknessMoveProhibit = ConditionDefinitionBuilder
            .Create(ConditionDarknessMoveProhibit)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration()
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}DarknessMoveProhibit")
                    .SetGuiPresentationNoContent(true)
                    .SetForbiddenActions((Id)ExtraActionId.ProxyDarkness)
                    .AddToDB())
            .AddToDB();

        var spellDarknessForms = SpellDarkness.EffectDescription.EffectForms;

        spellDarknessForms[0].SummonForm.effectProxyDefinitionName = EffectProxyDarkness.Name;

        spellDarknessForms.AddRange(
            EffectFormBuilder.ConditionForm(conditionDarknessMoveTracker, ConditionForm.ConditionOperation.Add, true,
                true),
            EffectFormBuilder.ConditionForm(conditionDarknessMoveProhibit, ConditionForm.ConditionOperation.Add, true,
                true));

        var powerDarkness = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Darkness")
            .SetGuiPresentation(SpellDarkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Sphere, 3)
                    .SetEffectForms()
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeDarkness(SpellDarkness))
            .AddToDB();

        var featureSetShadowArts = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ShadowArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(SenseDarkvision, powerDarkness)
            .AddToDB();

        // LEVEL 06 - Shadow Step

        var conditionShadowStep = ConditionDefinitionBuilder
            .Create($"Condition{Name}ShadowVeil")
            .SetGuiPresentation(Category.Condition, ConditionHeraldOfBattle)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}ShadowStep")
                    .SetGuiPresentationNoContent(true)
                    .SetMyAttackAdvantage(AdvantageType.Advantage)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var powerShadowStep = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowStep")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerSilhouetteStep, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.TeleportToDestination),
                        EffectFormBuilder.AddConditionForm(conditionShadowStep, true, true))
                    .SetParticleEffectParameters(PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddCustomSubFeatures(validateIsNotInBrightLight, new FilterTargetingPositionShadowsStep())
            .AddToDB();

        // LEVEL 11 - Improved Shadow Step

        var conditionImprovedShadowStep = ConditionDefinitionBuilder
            .Create($"Condition{Name}ImprovedShadowVeil")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create($"AdditionalAction{Name}ImprovedShadowStep")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionType.Bonus)
                    .SetRestrictedActions(Id.AttackOff)
                    .SetForbiddenActions(Id.FlurryOfBlows, Id.FlurryOfBlowsSwiftSteps, Id.FlurryOfBlowsUnendingStrikes)
                    .SetMaxAttacksNumber(1)
                    .AddToDB())
            .AddToDB();

        var powerImprovedShadowStep = FeatureDefinitionPowerBuilder
            .Create(powerShadowStep, $"Power{Name}ImprovedShadowStep")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder.MotionForm(MotionForm.MotionType.TeleportToDestination),
                        EffectFormBuilder.AddConditionForm(conditionShadowStep, true, true),
                        EffectFormBuilder.AddConditionForm(conditionImprovedShadowStep, true, true))
                    .SetParticleEffectParameters(PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddToDB();

        // LEVEL 17 - Cloak of Shadows

        var conditionCloakOfShadows = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, ConditionCloakOfShadowsName)
            .SetParentCondition(ConditionInvisibleBase)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionMoveThroughEnemyModifierBuilder
                    .Create($"MoveThroughEnemyModifier{Name}CloakOfShadows")
                    .SetGuiPresentationNoContent(true)
                    .SetMinSizeDifference(0)
                    .AddToDB())
            .SetCancellingConditions(ConditionDefinitions.ConditionIncapacitated)
            .AddCustomSubFeatures(new CustomBehaviorCloakOfShadows())
            .AddToDB();

        conditionCloakOfShadows.GuiPresentation.spriteReference =
            ConditionDefinitions.ConditionStealthy.GuiPresentation.SpriteReference;
        conditionCloakOfShadows.SpecialInterruptions.Clear();

        var powerCloakOfShadows = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CloakOfShadows")
            .SetGuiPresentation(Category.Feature, PowerPatronTimekeeperTimeShift)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.KiPoints, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Invisibility)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionCloakOfShadows))
                    .Build())
            .AddCustomSubFeatures(validateIsNotInBrightLight)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3, featureSetShadowArts)
            .AddFeaturesAtLevel(6, powerShadowStep)
            .AddFeaturesAtLevel(11, powerImprovedShadowStep)
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

    private sealed class PowerOrSpellFinishedByMeDarkness(SpellDefinition spellDarkness) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActingCharacter.MyExecuteActionCastNoCost(spellDarkness, 0, action.ActionParams);

            yield break;
        }
    }

    private sealed class ActionFinishedByMeProxyDarkness : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action.ActionId != (Id)ExtraActionId.ProxyDarkness)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                ConditionDarknessMoveProhibit,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDarknessMoveProhibit,
                0,
                0,
                0);
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

    private sealed class CustomBehaviorCloakOfShadows
        : IMagicEffectFinishedByMe, ICharacterBeforeTurnEndListener, IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            if (Gui.Battle != null ||
                action is not CharacterActionMove ||
                actingCharacter.MovingToDestination ||
                ValidatorsCharacter.IsNotInBrightLight(rulesetCharacter) ||
                !rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionCloakOfShadowsName, out var actionCondition))
            {
                yield break;
            }

            rulesetCharacter.RemoveCondition(actionCondition);
        }

        public void OnCharacterBeforeTurnEnded(GameLocationCharacter locationCharacter)
        {
            var rulesetCharacter = locationCharacter.RulesetCharacter;

            if (!ValidatorsCharacter.IsNotInBrightLight(rulesetCharacter) &&
                rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionCloakOfShadowsName, out var actionCondition))
            {
                rulesetCharacter.RemoveCondition(actionCondition);
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action, GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var sourceDefinition = action.ActionParams.RulesetEffect.SourceDefinition.Name;

            if (!sourceDefinition.StartsWith("PowerMonkFlurryOfBlows") &&
                !sourceDefinition.StartsWith("PowerTraditionFreedomFlurryOfBlows"))
            {
                yield break;
            }

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
