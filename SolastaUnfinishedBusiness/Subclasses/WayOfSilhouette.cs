using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfSilhouette : AbstractSubclass
{
    internal WayOfSilhouette()
    {
        var powerWayOfSilhouetteDarkness = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkness")
            .SetGuiPresentation(SpellDefinitions.Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Darkness.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfSilhouetteDarkvision = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkvision")
            .SetGuiPresentation(SpellDefinitions.Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Darkvision.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfSilhouettePassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouettePassWithoutTrace")
            .SetGuiPresentation(SpellDefinitions.PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.PassWithoutTrace.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var powerWayOfSilhouetteSilence = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteSilence")
            .SetGuiPresentation(SpellDefinitions.Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(SpellDefinitions.Silence.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var featureSetWayOfSilhouetteSilhouetteArts = FeatureDefinitionFeatureSetBuilder
            .Create("FeatureSetWayOfSilhouetteSilhouetteArts")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerWayOfSilhouetteDarkness,
                powerWayOfSilhouetteDarkvision,
                powerWayOfSilhouettePassWithoutTrace,
                powerWayOfSilhouetteSilence)
            .AddToDB();

        var powerWayOfSilhouetteSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteSilhouetteStep")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MistyStep)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(SpellDefinitions.MistyStep.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var conditionWayOfSilhouetteInvisibility = ConditionDefinitionBuilder
            .Create("ConditionWayOfSilhouetteInvisibility")
            .SetGuiPresentationNoContent()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(WayOfSilhouetteInvisibility.Build())
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(conditionWayOfSilhouetteInvisibility);

        var lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit, condition = conditionWayOfSilhouetteInvisibility
            })
            .AddToDB();
        
        var lightAffinityWayOfSilhouetteStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim, condition = conditionWayOfSilhouetteInvisibility
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness, condition = conditionWayOfSilhouetteInvisibility
            })
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfSilhouette")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3,
                featureSetWayOfSilhouetteSilhouetteArts,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak,
                powerWayOfSilhouetteSilhouetteStep)
            .AddFeaturesAtLevel(11,
                lightAffinityWayOfSilhouetteStrong)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;
}

internal sealed class WayOfSilhouetteInvisibility : ICustomOnActionFeature, ICustomConditionFeature
{
    private const string CategoryRevealed = "WayOfSilhouetteRevealed";
    private const string CategoryHidden = "WayOfSilhouetteHidden";
    private static ConditionDefinition ConditionWayOfSilhouetteRevealed { get; set; }
    private static ConditionDefinition ConditionWayOfSilhouetteInvisibility { get; set; }

    public void ApplyFeature(RulesetCharacter hero)
    {
        if (!hero.HasConditionOfType(ConditionWayOfSilhouetteRevealed))
        {
            BecomeInvisible(hero);
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
            BecomeRevealed(hero);
        }
    }

    internal static FeatureDefinition Build()
    {
        ConditionWayOfSilhouetteRevealed = ConditionDefinitionBuilder
            .Create("ConditionWayOfSilhouetteRevealed")
            .SetGuiPresentationNoContent()
            .SetDuration(DurationType.Round, 1)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        ConditionWayOfSilhouetteInvisibility = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionInvisible, "ConditionWayOfSilhouetteInvisible")
            .SetCancellingConditions(ConditionWayOfSilhouetteRevealed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpell,
                ConditionInterruption.UsePower)
            .SetTurnOccurence(TurnOccurenceType.StartOfTurn)
            .AddToDB();

        return FeatureDefinitionBuilder
            .Create("FeatureWayOfSilhouetteInvisibility")
            .SetGuiPresentationNoContent()
            .SetCustomSubFeatures(new WayOfSilhouetteInvisibility())
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

    private static void BecomeRevealed(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CategoryRevealed,
            RulesetCondition.CreateActiveCondition(
                hero.Guid,
                ConditionWayOfSilhouetteRevealed,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                hero.Guid,
                hero.CurrentFaction.Name
            ));
    }

    private static void BecomeInvisible(RulesetCharacter hero)
    {
        hero.AddConditionOfCategory(CategoryHidden,
            RulesetCondition.CreateActiveCondition(
                hero.Guid,
                ConditionWayOfSilhouetteInvisibility,
                DurationType.Permanent,
                0,
                TurnOccurenceType.EndOfTurn,
                hero.Guid,
                hero.CurrentFaction.Name),
            false);
    }
}

