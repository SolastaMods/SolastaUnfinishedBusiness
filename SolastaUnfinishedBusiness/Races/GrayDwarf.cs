using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;

namespace SolastaUnfinishedBusiness.Races;

internal static class GrayDwarfSubraceBuilder
{
    internal static CharacterRaceDefinition SubraceGrayDwarf { get; } = BuildGrayDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildGrayDwarf()
    {
        var grayDwarfSpriteReference = Sprites.GetSprite("GrayDwarf", Resources.GrayDwarf, 1024, 512);

        var attributeModifierGrayDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGrayDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var lightAffinityGrayDwarfLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(CustomConditionsContext.LightSensitivity.Name, Category.Condition)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = CustomConditionsContext.LightSensitivity
                })
            .AddToDB();

        var conditionAffinityGrayDwarfCharm = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityElfFeyAncestryCharm,
                "ConditionAffinityGrayDwarfCharm")
            .AddToDB();

        var conditionAffinityGrayDwarfCharmedByHypnoticPattern = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern,
                "ConditionAffinityGrayDwarfCharmedByHypnoticPattern")
            .AddToDB();

        var conditionAffinityGrayDwarfParalyzedAdvantage = FeatureDefinitionConditionAffinityBuilder
            .Create(ConditionAffinityHalflingBrave,
                "ConditionAffinityGrayDwarfParalyzedAdvantage")
            .SetConditionType(ConditionDefinitions.ConditionParalyzed)
            .AddToDB();

        var savingThrowAffinityGrayDwarfIllusion = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityGemIllusion,
                "SavingThrowAffinityGrayDwarfIllusion")
            .AddToDB();

        for (var i = 0; i < 6; i++)
        {
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].affinity = CharacterSavingThrowAffinity.Advantage;
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].savingThrowModifierDiceNumber = 0;
        }

        var featureSetGrayDwarfAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry, "FeatureSetGrayDwarfAncestry")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .ClearFeatureSet()
            .AddFeatureSet(
                conditionAffinityGrayDwarfCharm,
                conditionAffinityGrayDwarfCharmedByHypnoticPattern,
                conditionAffinityGrayDwarfParalyzedAdvantage,
                savingThrowAffinityGrayDwarfIllusion)
            .AddToDB();

        var abilityCheckAffinityGrayDwarfStoneStrength = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(AbilityCheckAffinityConditionBullsStrength, "AbilityCheckAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var savingThrowAffinityGrayDwarfStoneStrength = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(SavingThrowAffinityConditionRaging, "SavingThrowAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var additionalDamageGrayDwarfStoneStrength = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGrayDwarfStoneStrength")
            .SetGuiPresentationNoContent()
            .SetNotificationTag("StoneStrength")
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AlwaysActive)
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeStrengthWeapon)
            .SetDamageDice(DieType.D4, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddToDB();

        var conditionGrayDwarfStoneStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBullsStrength, "ConditionGrayDwarfStoneStrength")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionStoneResilience)
            .SetFeatures(
                abilityCheckAffinityGrayDwarfStoneStrength,
                savingThrowAffinityGrayDwarfStoneStrength,
                additionalDamageGrayDwarfStoneStrength)
            .AddToDB();

        var grayDwarfStoneStrengthEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription)
            .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
            .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
            .Build();

        grayDwarfStoneStrengthEffect.EffectForms[0].ConditionForm.conditionDefinition = conditionGrayDwarfStoneStrength;

        var powerGrayDwarfStoneStrength = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfStoneStrength")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Stoneskin)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(grayDwarfStoneStrengthEffect)
            .SetShowCasting(true)
            .AddToDB();

        var powerGrayDwarfInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.Invisibility.EffectDescription)
                .SetDurationData(DurationType.Minute, 1, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
            .SetShowCasting(true)
            .AddToDB();

        var grayDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        grayDwarfRacePresentation.needBeard = false;
        grayDwarfRacePresentation.MaleBeardShapeOptions.Add("BeardShape_None");
        grayDwarfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        grayDwarfRacePresentation.preferedHairColors = new RangedInt(35, 41);

        grayDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        grayDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;

        var raceGrayDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceGrayDwarf")
            .SetGuiPresentation(Category.Race, grayDwarfSpriteReference)
            .SetRacePresentation(grayDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierGrayDwarfStrengthAbilityScoreIncrease,
                featureSetGrayDwarfAncestry,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                lightAffinityGrayDwarfLightSensitivity,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages)
            .AddFeaturesAtLevel(3,
                powerGrayDwarfStoneStrength)
            .AddFeaturesAtLevel(5,
                powerGrayDwarfInvisibility)
            .AddToDB();

        raceGrayDwarf.subRaces.Clear();
        Dwarf.SubRaces.Add(raceGrayDwarf);

        return raceGrayDwarf;
    }
}
