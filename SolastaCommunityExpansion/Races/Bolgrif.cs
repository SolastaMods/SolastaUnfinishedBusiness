using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Races;

internal static class BolgrifRaceBuilder
{
    internal static CharacterRaceDefinition BolgrifRace { get; } = BuildBolgrif();

    private static CharacterRaceDefinition BuildBolgrif()
    {
        var bolgrifSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Bolgrif", Resources.Bolgrif, 1024, 512);

        var bolgrifAbilityScoreModifierWisdom = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifWisdomAbilityScoreIncrease", "4099c645-fc05-4ba1-833f-eabb94b865d0")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Wisdom, 2)
            .AddToDB();

        var bolgrifAbilityScoreModifierStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifStrengthAbilityScoreIncrease", "7b8d459b-c1f2-4373-bc4d-5e29ea4851f3")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var bolgrifPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler, "BolgrifPowerfulBuild",
                "3f635935-28a3-4bfd-8f51-77417ad7eb8a")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var bolgrifInvisibilityEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.Invisibility.EffectDescription)
            .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .Build();

        bolgrifInvisibilityEffect.EffectAdvancement.Clear();

        var bolgrifInvisibilityPower = FeatureDefinitionPowerBuilder
            .Create("BolgrifInvisibilityPower", "36dcb055-372c-4abf-83b7-4405475d9295")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(bolgrifInvisibilityEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetUsesFixed(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetShowCasting(true)
            .AddToDB();

        var bolgrifDruidicMagicSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListDruid, "BolgrifDruidicMagicSpellList",
                "3ac97eec-8d09-4ce3-8d29-40ea8b423798")
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells)
            .FinalizeSpells()
            .AddToDB();

        var bolgrifDruidicMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "BolgrifDruidicMagic",
                "ea2a9c8e-6ca9-490b-a056-d768182b5cd2")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(bolgrifDruidicMagicSpellList)
            .AddToDB();

        var bolgrifLanguageProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBolgrifLanguages", "dc03c8d7-5098-4dec-9f60-46e3af0d63c9")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", "Language_Giant",
                "Language_Elvish")
            .AddToDB();

        var bolgrifRacePresentation = CharacterRaceDefinitions.Dwarf.RacePresentation.DeepCopy();

        bolgrifRacePresentation.preferedSkinColors = new RangedInt(45, 47);
        bolgrifRacePresentation.preferedHairColors = new RangedInt(16, 32);
        bolgrifRacePresentation.needBeard = false;
        bolgrifRacePresentation.MaleBeardShapeOptions.Add(MorphotypeElementDefinitions.BeardShape_None.Name);

        var bolgrif = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Human, "BolgrifRace", "346b7f90-973f-425f-8342-d534759e65aa")
            .SetGuiPresentation(Category.Race, bolgrifSpriteReference)
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(bolgrifRacePresentation)
            .SetMinimalAge(30)
            .SetMaximalAge(500)
            .SetBaseHeight(84)
            .SetBaseWeight(170)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                bolgrifAbilityScoreModifierWisdom,
                bolgrifAbilityScoreModifierStrength,
                FeatureDefinitionSenses.SenseNormalVision,
                bolgrifPowerfulBuild,
                bolgrifInvisibilityPower,
                bolgrifDruidicMagic,
                bolgrifLanguageProficiency)
            .AddToDB();

        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(bolgrif.name);

        return bolgrif;
    }
}
