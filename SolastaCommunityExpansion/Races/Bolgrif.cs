using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using TA;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Races;

internal static class RaceBolgrifBuilder
{
    internal static CharacterRaceDefinition RaceBolgrif { get; } = BuildBolgrif();

    [NotNull]
    private static CharacterRaceDefinition BuildBolgrif()
    {
        var bolgrifSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Bolgrif", Resources.Bolgrif, 1024, 512);

        var bolgrifAbilityScoreModifierWisdom = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifWisdomAbilityScoreIncrease", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Wisdom, 2)
            .AddToDB();

        var bolgrifAbilityScoreModifierStrength = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifStrengthAbilityScoreIncrease", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var bolgrifPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler,
                "EquipmentAffinityBolgrifPowerfulBuild",
                DefinitionBuilder.CENamespaceGuid)
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
            .Create("PowerBolgrifInvisibility", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(bolgrifInvisibilityEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetUsesFixed(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetShowCasting(true)
            .AddToDB();

        var bolgrifDruidicMagicSpellList = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListDruid, "SpellListBolgrifMagic",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells)
            .FinalizeSpells()
            .AddToDB();

        var bolgrifDruidicMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellBolgrifMagic",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(bolgrifDruidicMagicSpellList)
            .AddToDB();

        var bolgrifLanguageProficiency = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBolgrifLanguages", DefinitionBuilder.CENamespaceGuid)
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
            .Create(CharacterRaceDefinitions.Human, "RaceBolgrif", DefinitionBuilder.CENamespaceGuid)
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
