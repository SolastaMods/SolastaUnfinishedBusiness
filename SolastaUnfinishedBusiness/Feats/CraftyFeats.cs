using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class CraftyFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyCraftyArcana = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyArcana")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle", "Feature/&ProficiencyCraftyArcanaDescription")
            .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Arcana.Name)
            .AddToDB();

        var proficiencyCraftyMedicine = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyMedicine")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle",
                "Feature/&ProficiencyCraftyMedicineDescription")
            .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Medecine.Name)
            .AddToDB();

        var proficiencyCraftyNature = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyNature")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle", "Feature/&ProficiencyCraftyNatureDescription")
            .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Nature.Name)
            .AddToDB();

        var proficiencyCraftyHerbalismKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyHerbalismKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var proficiencyCraftyManacalonRosary = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyManacalonRosary")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.EnchantingToolType.Name)
            .AddToDB();

        var proficiencyCraftyPoisonersKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyPoisonersKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var proficiencyCraftyScrollKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyScrollKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyScribeTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        var proficiencyCraftySmithsTools = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftySmithsTools")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .AddToDB();

        var proficiencyCraftyBows = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyBows")
            .SetGuiPresentation("Feature/&ProficiencyCraftyBowsTitle", "Feature/&ProficiencyCraftyBowsDescription")
            .SetProficiencies(ProficiencyType.Weapon,
                ShortbowType.Name,
                LongbowType.Name,
                CustomWeaponsContext.HandXbowWeaponType.Name,
                LightCrossbowType.Name,
                HeavyCrossbowType.Name)
            .AddToDB();

        var featApothecaryInt = FeatDefinitionBuilder
            .Create("FeatApothecaryInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyHerbalismKit, proficiencyCraftyArcana)
            .AddToDB();

        var featApothecaryWis = FeatDefinitionBuilder
            .Create("FeatApothecaryWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, proficiencyCraftyHerbalismKit, proficiencyCraftyMedicine)
            .AddToDB();

        GroupFeats.MakeGroup("FeatGroupApothecary", "Apothecary",
            featApothecaryInt,
            featApothecaryWis);

        var featManacalonCrafter = FeatDefinitionBuilder
            .Create("FeatManacalonCrafter")
            .SetGuiPresentation(Category.Feat)
            .SetMustCastSpellsPrerequisite()
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyManacalonRosary, proficiencyCraftyArcana)
            .AddToDB();

        var featToxicologistInt = FeatDefinitionBuilder
            .Create("FeatToxicologistInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyPoisonersKit, proficiencyCraftyNature)
            .AddToDB();

        var featToxicologistWis = FeatDefinitionBuilder
            .Create("FeatToxicologistWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, proficiencyCraftyPoisonersKit, proficiencyCraftyMedicine)
            .AddToDB();

        GroupFeats.MakeGroup("FeatGroupToxicologist", "Toxicologist",
            featToxicologistInt,
            featToxicologistWis);

        var featCraftyScribe = FeatDefinitionBuilder
            .Create("FeatCraftyScribe")
            .SetGuiPresentation(Category.Feat)
            .SetMustCastSpellsPrerequisite()
            .SetFeatures(AttributeModifierCreed_Of_Pakri, proficiencyCraftyScrollKit, proficiencyCraftyArcana)
            .AddToDB();

        var featCraftyFletcher = FeatDefinitionBuilder
            .Create("FeatCraftyFletcher")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, proficiencyCraftySmithsTools, proficiencyCraftyBows)
            .AddToDB();

        feats.AddRange(
            featApothecaryInt,
            featApothecaryWis,
            featManacalonCrafter,
            featToxicologistInt,
            featToxicologistWis,
            featCraftyScribe,
            featCraftyFletcher);
    }
}
