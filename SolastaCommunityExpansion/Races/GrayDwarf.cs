using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaCommunityExpansion.Races;

internal static class GrayDwarfSubraceBuilder
{
    internal static CharacterRaceDefinition GrayDwarfSubrace { get; } = BuildGrayDwarf();
	
	private static readonly Guid GrayDwarfNamespace = new("6dcf3e31-8c94-44e4-9dda-8eee0edf21d5");

    [NotNull]
    private static CharacterRaceDefinition BuildGrayDwarf()
    {
        var grayDwarfSpriteReference =
            //CustomIcons.CreateAssetReferenceSprite("GrayDwarf", Resources.GrayDwarf, 1024, 512);
			Dwarf.GuiPresentation.SpriteReference;

        var grayDwarfAbilityScoreModifierStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGrayDwarfStrengthAbilityScoreIncrease", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var grayDwarfPerceptionLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityGrayDwarfLightSensitivity", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage, RuleDefinitions.DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        grayDwarfPerceptionLightSensitivity.AffinityGroups[0].lightingContext =
            RuleDefinitions.LightingContext.BrightLight;

        var grayDwarfCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight,
                "CombatAffinityGrayDwarfLightSensitivity", GrayDwarfNamespace)
            .SetGuiPresentation(
                "Feature/&LightAffinityGrayDwarfLightSensitivityTitle",
                "Feature/&LightAffinityGrayDwarfLightSensitivityDescription")
            .SetMyAttackAdvantage(RuleDefinitions.AdvantageType.Disadvantage)
            .SetMyAttackModifierSign(RuleDefinitions.AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(RuleDefinitions.DieType.D4)
            .AddToDB();

        var grayDwarfConditionLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionGrayDwarfLightSensitive", GrayDwarfNamespace)
            .SetGuiPresentation(
                "Feature/&LightAffinityGrayDwarfLightSensitivityTitle",
                "Feature/&LightAffinityGrayDwarfLightSensitivityDescription",
                ConditionDefinitions.ConditionLightSensitive.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive(true)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(grayDwarfPerceptionLightSensitivity, grayDwarfCombatAffinityLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(grayDwarfConditionLightSensitive);

        var grayDwarfLightingEffectAndCondition = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Bright, condition = grayDwarfConditionLightSensitive
        };

        var grayDwarfLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityGrayDwarfLightSensitivity", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(grayDwarfLightingEffectAndCondition)
            .AddToDB();

        if (Main.Settings.ReduceGrayDwarfLightPenalty)
        {
            const string REDUCED_DESCRIPTION = "Feature/&LightAffinityGrayDwarfReducedLightSensitivityDescription";

            grayDwarfCombatAffinityLightSensitivity.myAttackAdvantage = RuleDefinitions.AdvantageType.None;
            grayDwarfCombatAffinityLightSensitivity.myAttackModifierValueDetermination =
                RuleDefinitions.CombatAddinityValueDetermination.Die;
            grayDwarfCombatAffinityLightSensitivity.GuiPresentation.description = REDUCED_DESCRIPTION;
            grayDwarfConditionLightSensitive.GuiPresentation.description = REDUCED_DESCRIPTION;
            grayDwarfLightAffinity.GuiPresentation.description = REDUCED_DESCRIPTION;
        }
        else
        {
            grayDwarfCombatAffinityLightSensitivity.myAttackAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
            grayDwarfCombatAffinityLightSensitivity.myAttackModifierValueDetermination =
                RuleDefinitions.CombatAddinityValueDetermination.None;
            grayDwarfCombatAffinityLightSensitivity.GuiPresentation.description =
                grayDwarfLightAffinity.GuiPresentation.Description;
            grayDwarfConditionLightSensitive.GuiPresentation.description =
                grayDwarfLightAffinity.GuiPresentation.Description;
            grayDwarfLightAffinity.GuiPresentation.description = grayDwarfLightAffinity.GuiPresentation.Description;
        }

        var grayDwarfConditionAffinityGrayDwarfCharm = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharm, "ConditionAffinityGrayDwarfCharm", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var grayDwarfConditionAffinityGrayDwarfCharmedByHypnoticPattern = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern, "ConditionAffinityGrayDwarfCharmedByHypnoticPattern", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var grayDwarfConditionAffinityGrayDwarfParalyzedAdvantage = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityHalflingBrave, "ConditionAffinityGrayDwarfParalyzedAdvantage", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetConditionType(ConditionDefinitions.ConditionParalyzed)            
            .AddToDB();

        var grayDwarfSavingThrowAffinityGrayDwarfIllusion = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityGemIllusion, "SavingThrowAffinityGrayDwarfIllusion", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[0].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[0].savingThrowModifierDiceNumber = 0;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[1].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[1].savingThrowModifierDiceNumber = 0;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[2].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[2].savingThrowModifierDiceNumber = 0;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[3].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[3].savingThrowModifierDiceNumber = 0;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[4].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[4].savingThrowModifierDiceNumber = 0;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[5].affinity = RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
        grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[5].savingThrowModifierDiceNumber = 0;

        var grayDwarfAncestryFeatureSetGrayDwarfAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry, "FeatureSetGrayDwarfAncestry", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                grayDwarfConditionAffinityGrayDwarfCharm,
                grayDwarfConditionAffinityGrayDwarfCharmedByHypnoticPattern,
                grayDwarfConditionAffinityGrayDwarfParalyzedAdvantage,
                grayDwarfSavingThrowAffinityGrayDwarfIllusion)
            .AddToDB();

        var grayDwarfAbilityCheckAffinityConditionStoneStrength = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength, "AbilityCheckAffinityConditionStoneStrength", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var grayDwarfSavingThrowAffinityConditionStoneStrength = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging, "SavingThrowAffinityConditionStoneStrength", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var grayDwarfAdditionalDamageConditionStoneStrength = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageConditionStoneStrength", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("StoneStrength")
            .SetTriggerCondition(RuleDefinitions.AdditionalDamageTriggerCondition.AlwaysActive)
            .SetRequiredProperty(RuleDefinitions.AdditionalDamageRequiredProperty.MeleeStrengthWeapon)
            .SetDamageDice(RuleDefinitions.DieType.D4, 1)
            .SetDamageValueDetermination(RuleDefinitions.AdditionalDamageValueDetermination.Die)
            .SetAdditionalDamageType(RuleDefinitions.AdditionalDamageType.SameAsBaseDamage)
            .SetFrequencyLimit(RuleDefinitions.FeatureLimitedUsage.None)
            .AddToDB();

        var grayDwarfConditionStoneStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBullsStrength, "ConditionStoneStrength", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(
                grayDwarfAbilityCheckAffinityConditionStoneStrength,
                grayDwarfSavingThrowAffinityConditionStoneStrength,
                grayDwarfAdditionalDamageConditionStoneStrength)
            .AddToDB();

        var grayDwarfStoneStrengthPower = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfStoneStrength", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Stoneskin.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        grayDwarfStoneStrengthPower.EffectDescription.EffectForms[0].ConditionForm.conditionDefinition = grayDwarfConditionStoneStrength;

        var grayDwarfInvisibilityPower = FeatureDefinitionPowerBuilder
            .Create("PowerGrayDwarfInvisibility", GrayDwarfNamespace)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Invisibility.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        var grayDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        grayDwarfRacePresentation.hasSurName = false;
        grayDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        grayDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;
		grayDwarfRacePresentation.needBeard = false;
		grayDwarfRacePresentation.MaleBeardShapeOptions.Clear();
        grayDwarfRacePresentation.preferedSkinColors = new RangedInt(49, 52);
        grayDwarfRacePresentation.preferedHairColors = new RangedInt(48, 53);

        var grayDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "GrayDwarfRace", "7f44816c-d076-4513-bf8f-22dc6582f7d5")
            .SetGuiPresentation(Category.Race, grayDwarfSpriteReference)
            .SetRacePresentation(grayDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove5,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                FeatureDefinitionProficiencys.ProficiencyDwarfLanguages,
                grayDwarfAncestryFeatureSetGrayDwarfAncestry,
                grayDwarfAbilityScoreModifierStrength,
                grayDwarfLightAffinity)
            .AddFeaturesAtLevel(3, grayDwarfStoneStrengthPower)
            .AddFeaturesAtLevel(5, grayDwarfInvisibilityPower)
            .AddToDB();

        grayDwarf.subRaces.Clear();
        Dwarf.SubRaces.Add(grayDwarf);

        return grayDwarf;
    }
}
