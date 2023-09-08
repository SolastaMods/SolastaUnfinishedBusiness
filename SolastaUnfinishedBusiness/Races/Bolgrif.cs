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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionEquipmentAffinitys;


namespace SolastaUnfinishedBusiness.Races;

internal static class RaceBolgrifBuilder
{
    internal static CharacterRaceDefinition RaceBolgrif { get; } = BuildBolgrif();

    [NotNull]
    private static CharacterRaceDefinition BuildBolgrif()
    {
        var bolgrifSpriteReference = Sprites.GetSprite("Bolgrif", Resources.Bolgrif, 1024, 512);

        var attributeModifierBolgrifWisdomAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifWisdomAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Wisdom, 2)
            .AddToDB();

        var attributeModifierBolgrifStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierBolgrifStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var equipmentAffinityBolgrifPowerfulBuild = FeatureDefinitionEquipmentAffinityBuilder
            .Create(EquipmentAffinityFeatHauler, "EquipmentAffinityBolgrifPowerfulBuild")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .AddToDB();

        var powerBolgrifInvisibility = FeatureDefinitionPowerBuilder
            .Create("PowerBolgrifInvisibility")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Invisibility.EffectDescription)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .ClearEffectAdvancements()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        var spellListBolgrif = SpellListDefinitionBuilder
            .Create("SpellListBolgrif")
            .SetGuiPresentationNoContent(true)
            .FinalizeSpells()
            .AddToDB();

        //explicitly re-use druid spell list, so custom cantrips selected for druid will show here 
        spellListBolgrif.SpellsByLevel[0].Spells = SpellListDefinitions.SpellListDruid.SpellsByLevel[0].Spells;

        var castSpellBolgrifMagic = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellElfHigh, "CastSpellBolgrifMagic")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetSpellCastingAbility(AttributeDefinitions.Wisdom)
            .SetSpellList(spellListBolgrif)
            .AddToDB();

        var proficiencyBolgrifLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyBolgrifLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Giant", "Language_Elvish")
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
            .SetBaseHeight(90)
            .SetBaseWeight(280)
            .SetMinimalAge(30)
            .SetMaximalAge(500)
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

        RacesContext.RaceScaleMap[raceBolgrif] = 8.8f / 6.4f;
        FeatDefinitions.FocusedSleeper.CompatibleRacesPrerequisite.Add(raceBolgrif.name);

        return raceBolgrif;
    }
}
