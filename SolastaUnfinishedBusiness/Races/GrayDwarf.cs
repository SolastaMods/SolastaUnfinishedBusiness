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
    internal static CharacterRaceDefinition GrayDwarfSubrace { get; } = BuildGrayDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildGrayDwarf()
    {
        var grayDwarfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("GrayDwarf", Resources.GrayDwarf, 1024, 512);

        var grayDwarfAbilityScoreModifierStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierGrayDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var grayDwarfPerceptionLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage, RuleDefinitions.DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        grayDwarfPerceptionLightSensitivity.AffinityGroups[0].lightingContext =
            RuleDefinitions.LightingContext.BrightLight;

        var grayDwarfCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight,
                "CombatAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation("LightAffinityGrayDwarfLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(RuleDefinitions.AdvantageType.None)
            .SetMyAttackModifierSign(RuleDefinitions.AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(RuleDefinitions.DieType.D4)
            .AddToDB();

        var grayDwarfConditionLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionGrayDwarfLightSensitive")
            .SetGuiPresentation(
                "LightAffinityGrayDwarfLightSensitivity", Category.Feature,
                ConditionDefinitions.ConditionLightSensitive.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive(true)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(grayDwarfPerceptionLightSensitivity, grayDwarfCombatAffinityLightSensitivity)
            .AddToDB();

        Global.CharacterLabelEnabledConditions.Add(grayDwarfConditionLightSensitive);

        var grayDwarfLightingEffectAndCondition = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Bright, condition = grayDwarfConditionLightSensitive
        };

        var grayDwarfLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityGrayDwarfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(grayDwarfLightingEffectAndCondition)
            .AddToDB();

        var grayDwarfConditionAffinityGrayDwarfCharm = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharm,
                "ConditionAffinityGrayDwarfCharm")
            .SetGuiPresentationNoContent()
            .AddToDB();

        var grayDwarfConditionAffinityGrayDwarfCharmedByHypnoticPattern = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityElfFeyAncestryCharmedByHypnoticPattern,
                "ConditionAffinityGrayDwarfCharmedByHypnoticPattern")
            .SetGuiPresentationNoContent()
            .AddToDB();

        var grayDwarfConditionAffinityGrayDwarfParalyzedAdvantage = FeatureDefinitionConditionAffinityBuilder
            .Create(FeatureDefinitionConditionAffinitys.ConditionAffinityHalflingBrave,
                "ConditionAffinityGrayDwarfParalyzedAdvantage")
            .SetConditionType(ConditionDefinitions.ConditionParalyzed)
            .AddToDB();

        var grayDwarfSavingThrowAffinityGrayDwarfIllusion = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityGemIllusion,
                "SavingThrowAffinityGrayDwarfIllusion")
            .AddToDB();

        for (var i = 0; i < 6; i++)
        {
            grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].affinity =
                RuleDefinitions.CharacterSavingThrowAffinity.Advantage;
            grayDwarfSavingThrowAffinityGrayDwarfIllusion.AffinityGroups[i].savingThrowModifierDiceNumber = 0;
        }

        var grayDwarfAncestryFeatureSetGrayDwarfAncestry = FeatureDefinitionFeatureSetBuilder
            .Create(FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry, "FeatureSetGrayDwarfAncestry")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                grayDwarfConditionAffinityGrayDwarfCharm,
                grayDwarfConditionAffinityGrayDwarfCharmedByHypnoticPattern,
                grayDwarfConditionAffinityGrayDwarfParalyzedAdvantage,
                grayDwarfSavingThrowAffinityGrayDwarfIllusion)
            .AddToDB();

        var grayDwarfAbilityCheckAffinityGrayDwarfStoneStrength = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionBullsStrength,
                "AbilityCheckAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var grayDwarfSavingThrowAffinityGrayDwarfStoneStrength = FeatureDefinitionSavingThrowAffinityBuilder
            .Create(FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityConditionRaging,
                "SavingThrowAffinityGrayDwarfStoneStrength")
            .AddToDB();

        var grayDwarfAdditionalDamageGrayDwarfStoneStrength = FeatureDefinitionAdditionalDamageBuilder
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

        var grayDwarfConditionGrayDwarfStoneStrength = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBullsStrength, "ConditionGrayDwarfStoneStrength")
            .SetGuiPresentation(
                Category.Condition,
                ConditionDefinitions.ConditionStoneResilience.GuiPresentation.SpriteReference)
            .SetFeatures(
                grayDwarfAbilityCheckAffinityGrayDwarfStoneStrength,
                grayDwarfSavingThrowAffinityGrayDwarfStoneStrength,
                grayDwarfAdditionalDamageGrayDwarfStoneStrength)
            .AddToDB();

        Global.CharacterLabelEnabledConditions.Add(grayDwarfConditionGrayDwarfStoneStrength);

        var grayDwarfStoneStrengthEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription)
            .SetDurationData(RuleDefinitions.DurationType.Minute, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .Build();

        grayDwarfStoneStrengthEffect.EffectForms[0].ConditionForm.conditionDefinition =
            grayDwarfConditionGrayDwarfStoneStrength;

        var grayDwarfStoneStrengthPower = FeatureDefinitionPowerBuilder
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

        var grayDwarfInvisibilityPower = FeatureDefinitionPowerBuilder
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

        var grayDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceGrayDwarf")
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
