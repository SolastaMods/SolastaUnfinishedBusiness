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

        // only reports condition on char panel
        Global.CharacterLabelEnabledConditions.Add(CustomConditionsContext.InvisibilityEveryRound);

        var lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit, condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();
        
        var lightAffinityWayOfSilhouetteStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim, condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness, condition = CustomConditionsContext.InvisibilityEveryRound
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
