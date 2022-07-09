using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaCommunityExpansion.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaCommunityExpansion.Feats;

public static class CraftyFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var craftyInt = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierCreed_Of_Pakri, "AttributeModifierFeatCraftyInt",
                "b23c3b73-7690-42ba-aa49-7ca3451daa05")
            .SetGuiPresentation("AttributeInt", Category.Feat,
                AttributeModifierCreed_Of_Pakri.GuiPresentation.SpriteReference)
            .AddToDB();

        var craftyWis = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierCreed_Of_Maraike, "AttributeModifierFeatCraftyWis",
                "23f944c7-2359-43cc-8bdc-71833bf35302")
            .SetGuiPresentation("AttributeWis", Category.Feat,
                AttributeModifierCreed_Of_Maraike.GuiPresentation.SpriteReference)
            .AddToDB();

        var craftyDex = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierCreed_Of_Misaye, "AttributeModifierFeatCraftyDex",
                "4db12466-67da-47a4-8d96-a9bf9cf3a251")
            .SetGuiPresentation("AttributeDex", Category.Feat,
                AttributeModifierCreed_Of_Misaye.GuiPresentation.SpriteReference)
            .AddToDB();

        var craftyArcana = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyArcana", "44a54666-80ba-475c-90b1-774e86f1a69a")
            .SetGuiPresentation("Feat/&CraftySkillsTitle", "Feat/&CraftyArcanaDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Arcana.Name)
            .AddToDB();

        var craftyMedicine = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyMedicine", "1ac54869-a8ce-4a51-a858-1f7e34680b96")
            .SetGuiPresentation("Feat/&CraftySkillsTitle", "Feat/&CraftyMedicineDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Medecine.Name)
            .AddToDB();

        var craftyNature = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyNature", "7399b06a-bfda-4e60-8366-17e0d6cec0d0")
            .SetGuiPresentation("Feat/&CraftySkillsTitle", "Feat/&CraftyNatureDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.SkillOrExpertise,
                DatabaseHelper.SkillDefinitions.Nature.Name)
            .AddToDB();

        var craftyHerbalismKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyHerbalismKit", "9345e1fd-ec4c-4509-acb5-3f3257b25ec4")
            .SetGuiPresentation("Feat/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.HerbalismKitType.Name)
            .AddToDB();

        var craftyManacalonRosary = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyManacalonRosary", "0685a944-76cd-423a-81a1-9ceec507d69a")
            .SetGuiPresentation("Feat/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.EnchantingToolType.Name)
            .AddToDB();

        var craftyPoisonersKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyPoisonersKit", "32ddae84-66e7-4b56-b5ec-0ec91a713e2e")
            .SetGuiPresentation("Feat/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.PoisonersKitType.Name)
            .AddToDB();

        var craftyScrollKit = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyScrollKit", "5309bd7f-b533-40ff-ae95-d977e02d61fe")
            .SetGuiPresentation("Feat/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ScrollKitType.Name)
            .AddToDB();

        var craftySmithsTools = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftySmithsTools", "48905450-4b35-480f-9868-f340c7902920")
            .SetGuiPresentation("Feat/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .AddToDB();

        var craftyBows = FeatureDefinitionProficiencyBuilder
            .Create(ProficiencyAllLanguages, "CraftyBows", "62a71277-b62d-41e6-9546-19f6faa2b5a7")
            .SetGuiPresentation("Feat/&CraftyBowsTitle", "Feat/&CraftyBowsDescription")
            .SetProficiencies(RuleDefinitions.ProficiencyType.Weapon, ShortbowType.Name, LongbowType.Name,
                LightCrossbowType.Name,
                HeavyCrossbowType.Name)
            .AddToDB();

        var apothecaryIntFeat = FeatDefinitionBuilder
            .Create(ArmorMaster, "ApothecaryInt", "ef387249-45e0-4899-aadd-44810f8aeb6d")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyInt, craftyHerbalismKit, craftyArcana)
            .AddToDB();

        var apothecaryWisFeat = FeatDefinitionBuilder
            .Create(ArmorMaster, "ApothecaryWis", "4fd80bf9-7749-4c01-9d95-6eb56c644fe2")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyWis, craftyHerbalismKit, craftyMedicine)
            .AddToDB();

        var manacalonCrafter = FeatDefinitionBuilder
            .Create(MasterEnchanter, "ManacalonCrafter", "290f73c8-201c-489e-bdcb-7a39ab40915c")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyInt, craftyManacalonRosary, craftyArcana)
            .AddToDB();

        var toxicologistInt = FeatDefinitionBuilder
            .Create(ArmorMaster, "ToxicologistInt", "702d1b4d-953c-406d-a900-d5d376ed29d3")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyInt, craftyPoisonersKit, craftyNature)
            .AddToDB();

        var toxicologistWis = FeatDefinitionBuilder
            .Create(ArmorMaster, "ToxicologistWis", "1bb4acbd-1890-48ae-9f86-46c2cb95cb79")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyWis, craftyPoisonersKit, craftyMedicine)
            .AddToDB();

        var craftyScribe = FeatDefinitionBuilder
            .Create(MasterEnchanter, "CraftyScribe", "bd83e063-2751-4898-8070-f74ca925f8b5")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyInt, craftyScrollKit, craftyArcana)
            .AddToDB();

        var craftyFletcher = FeatDefinitionBuilder
            .Create(ArmorMaster, "CraftyFletcher", "67c5f2d2-a98c-49a1-a1ab-16cc8f4b4ba4")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(craftyDex, craftySmithsTools, craftyBows)
            .AddToDB();

        feats.AddRange(apothecaryIntFeat, apothecaryWisFeat, manacalonCrafter, toxicologistInt, toxicologistWis,
            craftyScribe, craftyFletcher);
    }
}
