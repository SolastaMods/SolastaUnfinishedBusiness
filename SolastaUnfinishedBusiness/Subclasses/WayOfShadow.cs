using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfShadow : AbstractSubclass
{
    internal WayOfShadow()
    {
        var powerWayOfShadowDarkness = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowDarkness")
            .SetGuiPresentation(SpellDefinitions.Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Darkness.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfShadowDarkvision = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowDarkvision")
            .SetGuiPresentation(SpellDefinitions.Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Darkvision.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfShadowPassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowPassWithoutTrace")
            .SetGuiPresentation(SpellDefinitions.PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.PassWithoutTrace.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfShadowSilence = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowSilence")
            .SetGuiPresentation(SpellDefinitions.Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Silence.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var featureSetWayOfShadowShadowArts = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWayOfShadowShadowArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerWayOfShadowDarkness,
                powerWayOfShadowDarkvision,
                powerWayOfShadowPassWithoutTrace,
                powerWayOfShadowSilence)
            .AddToDB();

        //
        // keep a tab if it can use certain powers
        //

        var conditionWayOfShadowTrackHiddenRevealed = ConditionDefinitionBuilder
            .Create("ConditionWayOfShadowTrackHiddenRevealed")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(WayOfShadowVisibilityTracker.Build())
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionWayOfShadowTrackHiddenRevealed);

        var lightAffinityWayOfShadow = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfShadow")
            .SetGuiPresentationNoContent(true)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = conditionWayOfShadowTrackHiddenRevealed
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim,
                condition = conditionWayOfShadowTrackHiddenRevealed
            })
            .AddToDB();

        // TODO: add advantage on first melee attack
        var powerWayOfShadowShadowStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowShadowStep")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MistyStep)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.MistyStep.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Self)
                .Build())
            .SetCustomSubFeatures(
                ValidatorsCharacter.HasAnyOfConditions(WayOfShadowVisibilityTracker.ConditionWayOfShadowHidden))
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfShadowCloakOfShadows = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowCloakOfShadows")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.Invisibility.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
            .SetCustomSubFeatures(
                ValidatorsCharacter.HasAnyOfConditions(WayOfShadowVisibilityTracker.ConditionWayOfShadowHidden))
            .SetShowCasting(true)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfShadow")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3,
                featureSetWayOfShadowShadowArts,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                lightAffinityWayOfShadow,
                powerWayOfShadowShadowStep)
            .AddFeaturesAtLevel(11,
                powerWayOfShadowCloakOfShadows)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;
}

internal sealed class WayOfShadowVisibilityTracker : ICustomOnActionFeature, ICustomConditionFeature
{
    private const string CategoryRevealed = "WayOfShadowRevealed";
    private const string CategoryHidden = "WayOfShadowHidden";
    private static ConditionDefinition ConditionWayOfShadowRevealed { get; set; }
    internal static ConditionDefinition ConditionWayOfShadowHidden { get; set; }

    public void ApplyFeature(RulesetCharacter hero)
    {
        if (!hero.HasConditionOfType(ConditionWayOfShadowRevealed))
        {
            hero.AddConditionOfCategory(CategoryHidden,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    ConditionWayOfShadowHidden,
                    DurationType.Permanent,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name),
                false);
        }
    }

    public void RemoveFeature(RulesetCharacter hero)
    {
        hero.RemoveAllConditionsOfCategory(CategoryHidden, false);
    }

    public void OnAfterAction(CharacterAction characterAction)
    {
        var hero = characterAction.ActingCharacter.RulesetCharacter;
        var action = characterAction.ActionDefinition;

        if (!action.Name.StartsWith("Attack")
            && !action.Name.StartsWith("Cast")
            && !action.Name.StartsWith("Power"))
        {
            return;
        }

        var ruleEffect = characterAction.ActionParams.RulesetEffect;

        if (ruleEffect == null || !IsAllowedEffect(ruleEffect.EffectDescription))
        {
            hero.AddConditionOfCategory(CategoryRevealed,
                RulesetCondition.CreateActiveCondition(
                    hero.Guid,
                    ConditionWayOfShadowRevealed,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    hero.Guid,
                    hero.CurrentFaction.Name
                ));
        }
    }

    internal static FeatureDefinition Build()
    {
        ConditionWayOfShadowRevealed = ConditionDefinitionBuilder
            .Create("ConditionWayOfShadowRevealed")
            .SetGuiPresentationNoContent()
            .SetDuration(DurationType.Round, 1)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionWayOfShadowHidden = ConditionDefinitionBuilder
            .Create("ConditionWayOfShadowHidden")
            .SetGuiPresentationNoContent()
            .SetCancellingConditions(ConditionWayOfShadowRevealed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create("FeatureWayOfShadowTrackHiddenRevealed")
            .SetGuiPresentationNoContent()
            .SetCustomSubFeatures(new WayOfShadowVisibilityTracker())
            .AddToDB();
    }

    // returns true if effect is self teleport or any self targeting spell that is self-buff
    private static bool IsAllowedEffect(EffectDescription effect)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (effect.TargetType)
        {
            case TargetType.Position:
            {
                foreach (var form in effect.EffectForms)
                {
                    if (form.FormType != EffectForm.EffectFormType.Motion) { return false; }

                    if (form.MotionForm.Type != MotionForm.MotionType.TeleportToDestination) { return false; }
                }

                break;
            }
            case TargetType.Self:
            {
                foreach (var form in effect.EffectForms)
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (form.FormType)
                    {
                        case EffectForm.EffectFormType.Damage:
                        case EffectForm.EffectFormType.Healing:
                        case EffectForm.EffectFormType.ShapeChange:
                        case EffectForm.EffectFormType.Summon:
                        case EffectForm.EffectFormType.Counter:
                        case EffectForm.EffectFormType.Motion:
                            return false;
                    }
                }

                break;
            }
            default:
                return false;
        }

        return true;
    }
}
