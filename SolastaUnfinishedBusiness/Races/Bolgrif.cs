using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Utils;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceBolgrifBuilder
{
    internal static CharacterRaceDefinition RaceBolgrif { get; } = BuildBolgrif();

    [NotNull]
    private static CharacterRaceDefinition BuildBolgrif()
    {
        var bolgrifSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Bolgrif", Resources.Bolgrif, 1024, 512);

        var attributeModifierBolgrifWisdomAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifWisdomAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Wisdom, 2)
            .AddToDB();

        var attributeModifierBolgrifStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Strength, 1)
            .AddToDB();

        var equipmentAffinityBolgrifPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler,
                "EquipmentAffinityBolgrifPowerfulBuild")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var bolgrifInvisibilityEffect = EffectDescriptionBuilder
            .Create(SpellDefinitions.Invisibility.EffectDescription)
            .SetDurationData(RuleDefinitions.DurationType.Round, 1, RuleDefinitions.TurnOccurenceType.StartOfTurn)
            .ClearEffectAdvancements()
            .SetTargetingData(
                RuleDefinitions.Side.Ally,
                RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self)
            .Build();

        var powerBolgrifInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerBolgrifInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility.GuiPresentation.SpriteReference)
            .SetEffectDescription(bolgrifInvisibilityEffect)
            .SetActivationTime(RuleDefinitions.ActivationTime.BonusAction)
            .SetUsesFixed(1)
            .SetRechargeRate(RuleDefinitions.RechargeRate.ShortRest)
            .SetShowCasting(true)
            .AddToDB();

        var spellListBolgrifMagic = SpellListDefinitionBuilder
            .Create(SpellListDefinitions.SpellListDruid, "SpellListBolgrifMagic")
            .SetGuiPresentationNoContent()
            .ClearSpells()
            .SetSpellsAtLevel(0, SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells.ToArray())
            .FinalizeSpells()
            .AddToDB();

        var castSpellBolgrifMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellBolgrifMagic")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListBolgrifMagic)
            .AddToDB();

        var proficiencyBolgrifLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBolgrifLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(RuleDefinitions.ProficiencyType.Language, "Language_Common", "Language_Giant",
                "Language_Elvish")
            .AddToDB();

        var bolgrifRacePresentation = Dwarf.RacePresentation.DeepCopy();

        bolgrifRacePresentation.needBeard = false;
        bolgrifRacePresentation.MaleBeardShapeOptions.Add("BeardShape_None");
        bolgrifRacePresentation.preferedSkinColors = new RangedInt(45, 47);
        bolgrifRacePresentation.preferedHairColors = new RangedInt(16, 32);

        var raceBolgrif = CharacterRaceDefinitionBuilder
            .Create(Human, "RaceBolgrif")
            .SetGuiPresentation(Category.Race, bolgrifSpriteReference)
            .SetSizeDefinition(CharacterSizeDefinitions.Medium)
            .SetRacePresentation(bolgrifRacePresentation)
            .SetMinimalAge(30)
            .SetMaximalAge(500)
            .SetBaseHeight(84)
            .SetBaseWeight(170)
            .SetFeaturesAtLevel(1,
                attributeModifierBolgrifStrengthAbilityScoreIncrease,
                attributeModifierBolgrifWisdomAbilityScoreIncrease,
                equipmentAffinityBolgrifPowerfulBuild,
                powerBolgrifInvisibility,
                FeatureDefinitionSenses.SenseNormalVision,
                castSpellBolgrifMagic,
                FeatureDefinitionMoveModes.MoveModeMove6,
                proficiencyBolgrifLanguages)
            .AddToDB();

        raceBolgrif.GuiPresentation.sortOrder = Dwarf.GuiPresentation.sortOrder - 1;

        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceBolgrif.name);

        return raceBolgrif;
    }
}
