using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
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

        // TODO: add restriction to be in dim light or darkness to allow activation (maybe use similar logic as moonlit?)
        // TODO: add advantage on first melee attack
        var powerWayOfShadowShadowStep = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowShadowStep")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MistyStep)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.MistyStep.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.Self)
                .Build())
            .SetShowCasting(true)
            .AddToDB();

        // TODO: add restriction to be in dim light or darkness to allow activation (maybe use similar logic as moonlit?)
        var powerWayOfShadowCloakOfShadows = FeatureDefinitionPowerBuilder
            .Create("PowerWayOfShadowCloakOfShadows")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.Invisibility.EffectDescription)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
            .SetShowCasting(true)
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("WayOfShadow")
            .SetOrUpdateGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.RoguishShadowCaster)
            .AddFeaturesAtLevel(3,
                featureSetWayOfShadowShadowArts,
                FeatureDefinitionCastSpells.CastSpellTraditionLight)
            .AddFeaturesAtLevel(6,
                powerWayOfShadowShadowStep)
            .AddFeaturesAtLevel(11,
                powerWayOfShadowCloakOfShadows)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;
}
