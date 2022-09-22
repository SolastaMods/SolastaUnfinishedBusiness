using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
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

internal static class DarkelfSubraceBuilder
{
    internal static CharacterRaceDefinition DarkelfSubrace { get; } = BuildDarkelf();

    [NotNull]
    private static CharacterRaceDefinition BuildDarkelf()
    {
        var darkelfSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Darkelf", Resources.Darkelf, 1024, 512);

        var darkelfAbilityScoreModifierCharisma = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkelfCharismaAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Charisma, 1)
            .AddToDB();

        var darkElfPerceptionLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarkelfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage, RuleDefinitions.DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        darkElfPerceptionLightSensitivity.AffinityGroups[0].lightingContext =
            RuleDefinitions.LightingContext.BrightLight;

        var darkelfCombatAffinityLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight,
                "CombatAffinityDarkelfLightSensitivity")
            .SetGuiPresentation("LightAffinityDarkelfLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(RuleDefinitions.AdvantageType.Disadvantage)
            .SetMyAttackModifierSign(RuleDefinitions.AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(RuleDefinitions.DieType.D4)
            .AddToDB();

        var darkelfConditionLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionDarkelfLightSensitive")
            .SetGuiPresentation(
                "LightAffinityDarkelfLightSensitivity",
                Category.Feature,
                ConditionDefinitions.ConditionLightSensitive.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive(true)
            .SetConditionType(RuleDefinitions.ConditionType.Detrimental)
            .SetFeatures(darkElfPerceptionLightSensitivity, darkelfCombatAffinityLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(darkelfConditionLightSensitive);

        var darkelfLightingEffectAndCondition = new FeatureDefinitionLightAffinity.LightingEffectAndCondition
        {
            lightingState = LocationDefinitions.LightingState.Bright, condition = darkelfConditionLightSensitive
        };

        var darkelfLightAffinity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkelfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(darkelfLightingEffectAndCondition)
            .AddToDB();

        if (Main.Settings.ReduceDarkElfLightPenalty)
        {
            const string REDUCED_DESCRIPTION = "Feature/&LightAffinityDarkelfReducedLightSensitivityDescription";

            darkelfCombatAffinityLightSensitivity.myAttackAdvantage = RuleDefinitions.AdvantageType.None;
            darkelfCombatAffinityLightSensitivity.myAttackModifierValueDetermination =
                RuleDefinitions.CombatAffinityValueDetermination.Die;
            darkelfCombatAffinityLightSensitivity.GuiPresentation.description = REDUCED_DESCRIPTION;
            darkelfConditionLightSensitive.GuiPresentation.description = REDUCED_DESCRIPTION;
            darkelfLightAffinity.GuiPresentation.description = REDUCED_DESCRIPTION;
        }
        else
        {
            darkelfCombatAffinityLightSensitivity.myAttackAdvantage = RuleDefinitions.AdvantageType.Disadvantage;
            darkelfCombatAffinityLightSensitivity.myAttackModifierValueDetermination =
                RuleDefinitions.CombatAffinityValueDetermination.None;
            darkelfCombatAffinityLightSensitivity.GuiPresentation.description =
                darkelfLightAffinity.GuiPresentation.Description;
            darkelfConditionLightSensitive.GuiPresentation.description =
                darkelfLightAffinity.GuiPresentation.Description;
            darkelfLightAffinity.GuiPresentation.description = darkelfLightAffinity.GuiPresentation.Description;
        }

        var darkelfMagicSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListDarkelfMagic")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
            .FinalizeSpells()
            .AddToDB();

        var darkelfDarkMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellDarkelfMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellList(darkelfMagicSpellList)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .AddToDB();

        var darkelfFaerieFirePower = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfFaerieFire")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.FaerieFire.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.FaerieFire.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        darkelfFaerieFirePower.EffectDescription.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        darkelfFaerieFirePower.EffectDescription.fixedSavingThrowDifficultyClass = 8;
        darkelfFaerieFirePower.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        var darkelfDarknessPower = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfDarkness")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkness.GuiPresentation.SpriteReference)
            .SetEffectDescription(SpellDefinitions.Darkness.EffectDescription.Copy())
            .SetActivationTime(RuleDefinitions.ActivationTime.Action)
            .SetFixedUsesPerRecharge(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.LongRest)
            .SetCostPerUse(1)
            .SetShowCasting(true)
            .AddToDB();

        darkelfDarknessPower.EffectDescription.difficultyClassComputation =
            RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
        darkelfDarknessPower.EffectDescription.fixedSavingThrowDifficultyClass = 8;
        darkelfDarknessPower.EffectDescription.savingThrowDifficultyAbility = AttributeDefinitions.Charisma;

        var darkelfWeaponTraining = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyDarkelfWeaponTraining")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                WeaponTypeDefinitions.RapierType.Name,
                WeaponTypeDefinitions.ShortswordType.Name)
            .AddToDB();

        var darkelfRacePresentation = Elf.RacePresentation.DeepCopy();

        darkelfRacePresentation.surNameOptions = new List<string>
        {
            "Race/&DarkelfSurName1Title",
            "Race/&DarkelfSurName2Title",
            "Race/&DarkelfSurName3Title",
            "Race/&DarkelfSurName4Title",
            "Race/&DarkelfSurName5Title"
        };

        darkelfRacePresentation.femaleNameOptions = ElfHigh.RacePresentation.FemaleNameOptions;
        darkelfRacePresentation.maleNameOptions = ElfHigh.RacePresentation.MaleNameOptions;
        darkelfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        darkelfRacePresentation.preferedHairColors = new RangedInt(48, 53);

        var darkelf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceDarkelf")
            .SetGuiPresentation(Category.Race, darkelfSpriteReference)
            .SetRacePresentation(darkelfRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                FeatureDefinitionFeatureSets.FeatureSetElfHighLanguages,
                darkelfAbilityScoreModifierCharisma,
                darkelfWeaponTraining,
                darkelfDarkMagic,
                darkelfLightAffinity)
            .AddFeaturesAtLevel(3, darkelfFaerieFirePower)
            .AddFeaturesAtLevel(5, darkelfDarknessPower)
            .AddToDB();

        darkelf.subRaces.Clear();
        Elf.SubRaces.Add(darkelf);

        return darkelf;
    }
}
