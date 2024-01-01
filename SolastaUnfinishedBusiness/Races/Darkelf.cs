using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class SubraceDarkelfBuilder
{
    internal static CharacterRaceDefinition SubraceDarkelf { get; } = BuildDarkelf();

    internal static FeatureDefinitionCastSpell CastSpellDarkelfMagic { get; private set; }

    [NotNull]
    private static CharacterRaceDefinition BuildDarkelf()
    {
        CastSpellDarkelfMagic = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellDarkelfMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellKnowledge(SpellKnowledge.FixedList)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetSlotsPerLevel(SharedSpellsContext.RaceCastingSlots)
            .SetReplacedSpells(1, 0)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create("SpellListDarkelf")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.DancingLights)
                    .SetSpellsAtLevel(1, SpellDefinitions.FaerieFire)
                    .SetSpellsAtLevel(2, SpellDefinitions.Darkness)
                    .FinalizeSpells()
                    .AddToDB())
            .AddToDB();

        var darkelfSpriteReference = Sprites.GetSprite("Darkelf", Resources.Darkelf, 1024, 512);

        var attributeModifierDarkelfCharismaAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierDarkelfCharismaAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Charisma, 1)
            .AddToDB();

        var lightAffinityDarkelfLightSensitivity = FeatureDefinitionLightAffinityBuilder
            .Create("LightAffinityDarkelfLightSensitivity")
            .SetGuiPresentation(CustomConditionsContext.LightSensitivity.Name, Category.Condition)
            .AddLightingEffectAndCondition(
                new FeatureDefinitionLightAffinity.LightingEffectAndCondition
                {
                    lightingState = LocationDefinitions.LightingState.Bright,
                    condition = CustomConditionsContext.LightSensitivity
                })
            .AddToDB();

        var proficiencyDarkelfWeaponTraining = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyDarkelfWeaponTraining")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(
                ProficiencyType.Weapon,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                WeaponTypeDefinitions.RapierType.Name,
                WeaponTypeDefinitions.ShortswordType.Name)
            .AddToDB();

        var darkelfRacePresentation = Elf.RacePresentation.DeepCopy();

        darkelfRacePresentation.femaleNameOptions = ElfHigh.RacePresentation.FemaleNameOptions;
        darkelfRacePresentation.maleNameOptions = ElfHigh.RacePresentation.MaleNameOptions;
        darkelfRacePresentation.surNameOptions = []; // names are added from names.txt resources
        darkelfRacePresentation.preferedSkinColors = new RangedInt(48, 53);
        darkelfRacePresentation.preferedHairColors = new RangedInt(48, 53);

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
            .AddToDB();

        raceDarkelf.subRaces.Clear();
        Elf.SubRaces.Add(raceDarkelf);

        return raceDarkelf;
    }
}
