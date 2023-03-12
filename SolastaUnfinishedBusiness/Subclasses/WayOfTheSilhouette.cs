using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class WayOfTheSilhouette : AbstractSubclass
{
    internal WayOfTheSilhouette()
    {
        var powerWayOfSilhouetteDarkness = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkness")
            .SetGuiPresentation(Darkness.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(Darkness.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteDarkvision = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteDarkvision")
            .SetGuiPresentation(Darkvision.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(Darkvision.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouettePassWithoutTrace = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouettePassWithoutTrace")
            .SetGuiPresentation(PassWithoutTrace.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(PassWithoutTrace.EffectDescription)
            .AddToDB();

        var powerWayOfSilhouetteSilence = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteSilence")
            .SetGuiPresentation(Silence.GuiPresentation)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 2)
            .SetEffectDescription(Silence.EffectDescription)
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
            .SetGuiPresentation(Category.Feature, MistyStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetEffectDescription(MistyStep.EffectDescription)
            .SetShowCasting(true)
            .AddToDB();

        var lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesWeak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        var lightAffinityWayOfSilhouetteStrong = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityWayOfSilhouetteCloakOfSilhouettesStrong")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Dim,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Darkness,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        var powerWayOfSilhouetteImprovedSilhouetteStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfSilhouetteImprovedSilhouetteStep")
            .SetGuiPresentation(Category.Feature, DimensionDoor)
            .SetOverriddenPower(powerWayOfSilhouetteSilhouetteStep)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(DimensionDoor.EffectDescription)
            .SetUniqueInstance()
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfSilhouette")
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("WayOfTheSilhouette", Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3,
                featureSetWayOfSilhouetteSilhouetteArts,
                lightAffinityWayOfSilhouetteCloakOfSilhouettesWeak,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                lightAffinityWayOfSilhouetteStrong,
                powerWayOfSilhouetteSilhouetteStep)
            .AddFeaturesAtLevel(11,
                powerWayOfSilhouetteImprovedSilhouetteStep)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
