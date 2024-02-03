using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMoveModes;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class PatronMoonlit : AbstractSubclass
{
    private const string Name = "Moonlit";

    public PatronMoonlit()
    {
        var spellListMoonlit = SpellListDefinitionBuilder
            .Create(SpellListWizard, $"SpellList{Name}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(1, FaerieFire, Sleep)
            .SetSpellsAtLevel(2, MoonBeam, SeeInvisibility)
            .SetSpellsAtLevel(3, Daylight, Slow)
            .SetSpellsAtLevel(4, GreaterInvisibility, GuardianOfFaith)
            .SetSpellsAtLevel(5, DominatePerson, MindTwist)
            .FinalizeSpells(true, 9)
            .AddToDB();

        var magicAffinityMoonlitExpandedSpells = FeatureDefinitionMagicAffinityBuilder
            .Create($"MagicAffinity{Name}ExpandedSpells")
            .SetGuiPresentation("MagicAffinityPatronExpandedSpells", Category.Feature)
            .SetExtendedSpellList(spellListMoonlit)
            .AddToDB();

        var lightAffinityMoonlitWeak = FeatureDefinitionLightAffinityBuilder
            .Create($"LightAffinity{Name}Weak")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(new FeatureDefinitionLightAffinity.LightingEffectAndCondition
            {
                lightingState = LocationDefinitions.LightingState.Unlit,
                condition = CustomConditionsContext.InvisibilityEveryRound
            })
            .AddToDB();

        var lightAffinityMoonlitStrong = FeatureDefinitionLightAffinityBuilder
            .Create($"LightAffinity{Name}Strong")
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

        var featureSetMoonlitSuperiorSight = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureSetInvocationDevilsSight, $"FeatureSet{Name}SuperiorSight")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(SenseSuperiorDarkvision)
            .AddToDB();

        var powerMoonlitDarkMoon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DarkMoon")
            .SetGuiPresentation(Category.Feature, Darkness)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Darkness.EffectDescription)
                    .SetDurationData(DurationType.Minute, 1)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerMoonlitFullMoon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}FullMoon")
            .SetGuiPresentation(Category.Feature, Daylight)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Daylight.EffectDescription)
                    .SetDurationData(DurationType.Minute, 1)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var powerMoonlitDanceOfTheNightSky = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}DanceOfTheNightSky")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Fly.EffectDescription)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique, 4)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var conditionMoonTouched = ConditionDefinitionBuilder
            .Create(DatabaseHelper.ConditionDefinitions.ConditionLevitate,
                $"Condition{Name}MoonTouched")
            .SetGuiPresentation(Category.Condition)
            .SetParentCondition(DatabaseHelper.ConditionDefinitions.ConditionFlying)
            .SetConditionType(ConditionType.Neutral)
            .SetFeatures(MoveModeFly2)
            .AddToDB();

        var powerMoonlitMoonTouched = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MoonTouched")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 12, TargetType.Cylinder, 10, 10)
                    .SetSavingThrowData(
                        true,
                        AttributeDefinitions.Dexterity,
                        true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Dexterity,
                        20)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                conditionMoonTouched,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.Levitate, 10)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .SetRecurrentEffect(Entangle.EffectDescription.RecurrentEffect)
                    .Build())
            .SetUniqueInstance()
            .AddToDB();

        var bonusCantripsMoonlit = FeatureDefinitionBonusCantripsBuilder
            .Create($"BonusCantrips{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetBonusCantrips(
                SpellDefinitionBuilder
                    .Create(MoonBeam, "AtWillMoonbeam")
                    .SetSpellLevel(0)
                    .AddToDB(),
                SpellDefinitionBuilder
                    .Create(FaerieFire, "AtWillFaerieFire")
                    .SetSpellLevel(0)
                    .AddToDB())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Patron{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PatronMoonlit, 256))
            .AddFeaturesAtLevel(1,
                lightAffinityMoonlitWeak,
                magicAffinityMoonlitExpandedSpells,
                featureSetMoonlitSuperiorSight)
            .AddFeaturesAtLevel(6,
                lightAffinityMoonlitStrong,
                powerMoonlitDarkMoon,
                powerMoonlitFullMoon)
            .AddFeaturesAtLevel(10,
                powerMoonlitDanceOfTheNightSky,
                powerMoonlitMoonTouched)
            .AddFeaturesAtLevel(14,
                bonusCantripsMoonlit)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => DatabaseHelper.CharacterClassDefinitions.Warlock;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice => DatabaseHelper.FeatureDefinitionSubclassChoices
        .SubclassChoiceWarlockOtherworldlyPatrons;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
