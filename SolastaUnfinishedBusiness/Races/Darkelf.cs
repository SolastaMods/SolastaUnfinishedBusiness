using System.Collections.Generic;
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
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class DarkelfSubraceBuilder
{
    internal static CharacterRaceDefinition SubraceDarkelf { get; } = BuildDarkelf();

    internal static FeatureDefinitionCastSpell CastSpellDarkelfMagic { get; private set; }

    internal static FeatureDefinitionPower PowerDarkelfFaerieFire { get; private set; }

    internal static FeatureDefinitionPower PowerDarkelfDarkness { get; private set; }

    [NotNull]
    private static CharacterRaceDefinition BuildDarkelf()
    {
        var darkelfSpriteReference =
            CustomIcons.GetSprite("Darkelf", Resources.Darkelf, 1024, 512);

        var attributeModifierDarkelfCharismaAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkelfCharismaAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Charisma, 1)
            .AddToDB();

        var abilityCheckAffinityDarkelfLightSensitivity = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create("AbilityCheckAffinityDarkelfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.Disadvantage, DieType.D1, 0,
                (AttributeDefinitions.Wisdom, SkillDefinitions.Perception))
            .AddToDB();

        abilityCheckAffinityDarkelfLightSensitivity.AffinityGroups[0].lightingContext =
            LightingContext.BrightLight;

        var combatAffinityDarkelfLightSensitivity = FeatureDefinitionCombatAffinityBuilder
            .Create(FeatureDefinitionCombatAffinitys.CombatAffinitySensitiveToLight,
                "CombatAffinityDarkelfLightSensitivity")
            .SetGuiPresentation("LightAffinityDarkelfLightSensitivity", Category.Feature)
            .SetMyAttackAdvantage(AdvantageType.None)
            .SetMyAttackModifierSign(AttackModifierSign.Substract)
            .SetMyAttackModifierDieType(DieType.D4)
            .AddToDB();

        var conditionDarkelfLightSensitive = ConditionDefinitionBuilder
            .Create("ConditionDarkelfLightSensitive")
            .SetGuiPresentation(
                "LightAffinityDarkelfLightSensitivity",
                Category.Feature,
                ConditionDefinitions.ConditionLightSensitive.GuiPresentation.SpriteReference)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetPossessive()
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(abilityCheckAffinityDarkelfLightSensitivity, combatAffinityDarkelfLightSensitivity)
            .AddToDB();

        // this allows the condition to still display as a label on character panel
        Global.CharacterLabelEnabledConditions.Add(conditionDarkelfLightSensitive);

        var lightAffinityDarkelfLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkelfLightSensitivity")
            .SetGuiPresentation(Category.Feature)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = conditionDarkelfLightSensitive
                })
            .AddToDB();

        var spellListDarkelfMagic = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListWizard, "SpellListDarkelfMagic")
            .SetGuiPresentationNoContent()
            .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
            .FinalizeSpells()
            .AddToDB();

        CastSpellDarkelfMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellDarkelfMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellList(spellListDarkelfMagic)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .AddToDB();

        PowerDarkelfFaerieFire = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfFaerieFire")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.FaerieFire.GuiPresentation.SpriteReference)
            .SetUsesFixed(
                ActivationTime.Action,
                RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.FaerieFire.EffectDescription)
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Charisma,
                        8)
                    .Build())
            .SetShowCasting(true)
            .AddToDB();

        PowerDarkelfDarkness = FeatureDefinitionPowerBuilder
            .Create("PowerDarkelfDarkness")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Darkness.GuiPresentation.SpriteReference)
            .SetUsesFixed(
                ActivationTime.Action,
                RechargeRate.LongRest)
            .SetEffectDescription(SpellDefinitions.Darkness.EffectDescription, true)
            .SetShowCasting(true)
            .AddToDB();

        var proficiencyDarkelfWeaponTraining = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyDarkelfWeaponTraining")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                WeaponTypeDefinitions.RapierType.Name,
                WeaponTypeDefinitions.ShortswordType.Name)
            .AddToDB();

        var darkelfRacePresentation = Elf.RacePresentation.DeepCopy();

        darkelfRacePresentation.femaleNameOptions = ElfHigh.RacePresentation.FemaleNameOptions;
        darkelfRacePresentation.maleNameOptions = ElfHigh.RacePresentation.MaleNameOptions;
        darkelfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        darkelfRacePresentation.preferedHairColors = new RangedInt(48, 53);

        darkelfRacePresentation.surNameOptions = new List<string>();

        for (var i = 1; i <= 5; i++)
        {
            darkelfRacePresentation.surNameOptions.Add($"Race/&DarkelfSurName{i}Title");
        }

        var raceDarkelf = CharacterRaceDefinitionBuilder
            .Create(ElfHigh, "RaceDarkelf")
            .SetGuiPresentation(Category.Race, darkelfSpriteReference)
            .SetRacePresentation(darkelfRacePresentation)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseSuperiorDarkvision,
                FeatureDefinitionFeatureSets.FeatureSetElfHighLanguages,
                attributeModifierDarkelfCharismaAbilityScoreIncrease,
                proficiencyDarkelfWeaponTraining,
                CastSpellDarkelfMagic,
                lightAffinityDarkelfLightSensitivity)
            .AddFeaturesAtLevel(3,
                PowerDarkelfFaerieFire)
            .AddFeaturesAtLevel(5,
                PowerDarkelfDarkness)
            .AddToDB();

        raceDarkelf.GuiPresentation.sortOrder = ElfSylvan.GuiPresentation.sortOrder + 1;
        raceDarkelf.subRaces.Clear();
        Elf.SubRaces.Add(raceDarkelf);

        return raceDarkelf;
    }
}
