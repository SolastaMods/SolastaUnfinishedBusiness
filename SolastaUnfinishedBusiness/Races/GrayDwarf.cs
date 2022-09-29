using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class GrayDwarfSubraceBuilder
{
    internal static CharacterRaceDefinition SubraceGrayDwarf { get; } = BuildGrayDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildGrayDwarf()
    {
        var grayDwarfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("GrayDwarf", Resources.GrayDwarf, 1024, 512);

        var attributeModifierGrayDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGrayDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var abilityCheckAffinityGrayDwarfLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage, RuleDefinitions.DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityGrayDwarfLightSensitivity.AffinityGroups[0].lightingContext =
            RuleDefinitions.LightingContext.BrightLight;

        var grayDwarfCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight,
                "CombatAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation("LightAffinityGrayDwarfLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(RuleDefinitions.AdvantageType.None)
            .SetMyAttackModifierSign(RuleDefinitions.AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(RuleDefinitions.DieType.D4)
            .AddToDB();

        var conditionGrayDwarfLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionGrayDwarfLightSensitive")
            .SetGuiPresentation(
                "LightAffinityGrayDwarfLightSensitivity", Category.Feature,
                ConditionDefinitions.ConditionLightSensitive.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive(true)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityGrayDwarfLightSensitivity, grayDwarfCombatAffinityLightSensitivity)
            .AddToDB();

        Global.CharacterLabelEnabledConditions.Add(conditionGrayDwarfLightSensitive);

        var grayDwarfLightingEffectAndCondition = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Bright, condition = conditionGrayDwarfLightSensitive
        };

        var lightAffinityGrayDwarfLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(grayDwarfLightingEffectAndCondition)
            .AddToDB();

        var conditionAffinityGrayDwarfCharm = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharm,
                "ConditionAffinityGrayDwarfCharm")
            .SetGuiPresentationNoContent()
            .AddToDB();

        var conditionAffinityGrayDwarfCharmedByHypnoticPattern = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern,
                "ConditionAffinityGrayDwarfCharmedByHypnoticPattern")
            .SetGuiPresentationNoContent()
            .AddToDB();

        var conditionAffinityGrayDwarfParalyzedAdvantage = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityHalflingBrave,
                "ConditionAffinityGrayDwarfParalyzedAdvantage")
            .SetConditionType(ConditionDefinitions.ConditionParalyzed)
            .AddToDB();

        var savingThrowAffinityGrayDwarfIllusion = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityGemIllusion,
                "SavingThrowAffinityGrayDwarfIllusion")
            .AddToDB();

        for (var i = 0; i < 6; i++)
        {
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].affinity =
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
            savingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].savingThrowModifierDiceNumber = 0;
        }

        var featureSetGrayDwarfAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry, "FeatureSetGrayDwarfAncestry")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                conditionAffinityGrayDwarfCharm,
                conditionAffinityGrayDwarfCharmedByHypnoticPattern,
                conditionAffinityGrayDwarfParalyzedAdvantage,
                savingThrowAffinityGrayDwarfIllusion)
            .AddToDB();

        var abilityCheckAffinityGrayDwarfStoneStrength = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                "AbilityCheckAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var savingThrowAffinityGrayDwarfStoneStrength = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging,
                "SavingThrowAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var additionalDamageGrayDwarfStoneStrength = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGrayDwarfStoneStrength")
            .SetGuiPresentationNoContent()
            .SetNotificationTag("StoneStrength")
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetRequiredProperty(RuleDefinitions.RestrictedContextRequiredProperty.MeleeStrengthWeapon)
            .SetDamageDice(RuleDefinitions.DieType.D4, 1)
            .SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die)
            .SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage)
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .AddToDB();

        var conditionGrayDwarfStoneStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBullsStrength, "ConditionGrayDwarfStoneStrength")
            .SetGuiPresentation(
                Category.Condition,
                ConditionDefinitions.ConditionStoneResilience.GuiPresentation.SpriteReference)
            .SetFeatures(
                abilityCheckAffinityGrayDwarfStoneStrength,
                savingThrowAffinityGrayDwarfStoneStrength,
                additionalDamageGrayDwarfStoneStrength)
            .AddToDB();

        Global.CharacterLabelEnabledConditions.Add(conditionGrayDwarfStoneStrength);

        var grayDwarfStoneStrengthEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription)
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .Build();

        grayDwarfStoneStrengthEffect.EffectForms[0].ConditionForm.conditionDefinition =
            conditionGrayDwarfStoneStrength;

        var powerGrayDwarfStoneStrength = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfStoneStrength")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Stoneskin.GuiPresentation.SpriteReference)
            .SetEffectDescription(grayDwarfStoneStrengthEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var grayDwarfInvisibilityEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.Invisibility.EffectDescription)
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .Build();

        var powerGrayDwarfInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(grayDwarfInvisibilityEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var grayDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        grayDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        grayDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;
        grayDwarfRacePresentation.needBeard = false;
        grayDwarfRacePresentation.MaleBeardShapeOptions.SetRange(MorphotypeElementDefinitions.BeardShape_None.Name);
        grayDwarfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        grayDwarfRacePresentation.preferedHairColors = new RangedInt(35, 41);

        var raceGrayDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceGrayDwarf")
            .SetGuiPresentation(Category.Race, grayDwarfSpriteReference)
            .SetRacePresentation(grayDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove5,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages,
                featureSetGrayDwarfAncestry,
                attributeModifierGrayDwarfStrengthAbilityScoreIncrease,
                lightAffinityGrayDwarfLightSensitivity)
            .AddFeaturesAtLevel(3, powerGrayDwarfStoneStrength)
            .AddFeaturesAtLevel(5, powerGrayDwarfInvisibility)
            .AddToDB();

        raceGrayDwarf.subRaces.Clear();
        Dwarf.SubRaces.Add(raceGrayDwarf);

        return raceGrayDwarf;
    }
}
