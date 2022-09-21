using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

public static class CraftyFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var craftyArcana = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyArcana")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle", "Feature/&ProficiencyCraftyArcanaDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Arcana.Name)
            .AddToDB();

        var craftyMedicine = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyMedicine")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle",
                "Feature/&ProficiencyCraftyMedicineDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Medecine.Name)
            .AddToDB();

        var craftyNature = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyNature")
            .SetGuiPresentation("Feature/&ProficiencyCraftySkillsTitle", "Feature/&ProficiencyCraftyNatureDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Nature.Name)
            .AddToDB();

        var craftyHerbalismKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyHerbalismKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var craftyManacalonRosary = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyManacalonRosary")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.EnchantingToolType.Name)
            .AddToDB();

        var craftyPoisonersKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyPoisonersKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var craftyScrollKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyScrollKit")
            .SetGuiPresentation("Feature/&ProficiencyCraftyScribeTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        var craftySmithsTools = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftySmithsTools")
            .SetGuiPresentation("Feature/&ProficiencyCraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .AddToDB();

        var craftyBows = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "ProficiencyCraftyBows")
            .SetGuiPresentation("Feature/&ProficiencyCraftyBowsTitle", "Feature/&ProficiencyCraftyBowsDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon, ShortbowType.Name, LongbowType.Name,
                LightCrossbowType.Name,
                HeavyCrossbowType.Name)
            .AddToDB();

        var apothecaryIntFeat = FeatDefinitionBuilder
            .Create(ArmorMaster, "FeatApothecaryInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, craftyHerbalismKit, craftyArcana)
            .AddToDB();

        var apothecaryWisFeat = FeatDefinitionBuilder
            .Create(ArmorMaster, "FeatApothecaryWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, craftyHerbalismKit, craftyMedicine)
            .AddToDB();

        GroupFeats.MakeGroup("FeatGroupApothecary", "Apothecary",
            apothecaryIntFeat,
            apothecaryWisFeat);

        var manacalonCrafter = FeatDefinitionBuilder
            .Create(MasterEnchanter, "FeatManacalonCrafter")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, craftyManacalonRosary, craftyArcana)
            .AddToDB();

        var toxicologistInt = FeatDefinitionBuilder
            .Create(ArmorMaster, "FeatToxicologistInt")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, craftyPoisonersKit, craftyNature)
            .AddToDB();

        var toxicologistWis = FeatDefinitionBuilder
            .Create(ArmorMaster, "FeatToxicologistWis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, craftyPoisonersKit, craftyMedicine)
            .AddToDB();

        GroupFeats.MakeGroup("FeatGroupToxicologist", "Toxicologist",
            toxicologistInt,
            toxicologistWis);

        var craftyScribe = FeatDefinitionBuilder
            .Create(MasterEnchanter, "FeatCraftyScribe")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Pakri, craftyScrollKit, craftyArcana)
            .AddToDB();

        var craftyFletcher = FeatDefinitionBuilder
            .Create(ArmorMaster, "FeatCraftyFletcher")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, craftySmithsTools, craftyBows)
            .AddToDB();

        feats.AddRange(apothecaryIntFeat, apothecaryWisFeat, manacalonCrafter, toxicologistInt, toxicologistWis,
            craftyScribe, craftyFletcher);
    }
}
