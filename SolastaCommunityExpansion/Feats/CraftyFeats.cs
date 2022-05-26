using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.FeatDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;
using static SolastaModApi.DatabaseHelper.WeaponTypeDefinitions;
using ProficiencyType = RuleDefinitions.ProficiencyType;

namespace SolastaCommunityExpansion.Feats
{
    public static class CraftyFeats
    {
        internal static void CreateFeats(List<FeatDefinition> feats)
        {
            var crafty_int = FeatureDefinitionAttributeModifierBuilder
                .Create(AttributeModifierCreed_Of_Pakri, "AttributeModifierFeatCraftyInt", "b23c3b73-7690-42ba-aa49-7ca3451daa05")
                .SetGuiPresentation("AttributeInt", Category.CraftyFeats, AttributeModifierCreed_Of_Pakri.GuiPresentation.SpriteReference)
                .AddToDB();

            var crafty_wis = FeatureDefinitionAttributeModifierBuilder
                .Create(AttributeModifierCreed_Of_Maraike, "AttributeModifierFeatCraftyWis", "23f944c7-2359-43cc-8bdc-71833bf35302")
                .SetGuiPresentation("AttributeWis", Category.CraftyFeats, AttributeModifierCreed_Of_Maraike.GuiPresentation.SpriteReference)
                .AddToDB();

            var crafty_dex = FeatureDefinitionAttributeModifierBuilder
                .Create(AttributeModifierCreed_Of_Maraike, "AttributeModifierFeatCraftyDex", "4db12466-67da-47a4-8d96-a9bf9cf3a251")
                .SetGuiPresentation("AttributeDex", Category.CraftyFeats, AttributeModifierCreed_Of_Misaye.GuiPresentation.SpriteReference)
                .AddToDB();

            var crafty_arcana = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyArcana", "44a54666-80ba-475c-90b1-774e86f1a69a")
                .SetGuiPresentation("CraftyFeats/&CraftySkillsTitle", "CraftyFeats/&CraftyArcanaDescription")
                .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Arcana.Name)
                .AddToDB();

            var crafty_medicine = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyMedicine", "1ac54869-a8ce-4a51-a858-1f7e34680b96")
                .SetGuiPresentation("CraftyFeats/&CraftySkillsTitle", "CraftyFeats/&CraftyMedicineDescription")
                .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Medecine.Name)
                .AddToDB();

            var crafty_nature = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyNature", "7399b06a-bfda-4e60-8366-17e0d6cec0d0")
                .SetGuiPresentation("CraftyFeats/&CraftySkillsTitle", "CraftyFeats/&CraftyNatureDescription")
                .SetProficiencies(ProficiencyType.SkillOrExpertise, DatabaseHelper.SkillDefinitions.Nature.Name)
                .AddToDB();

            var crafty_herbalism_kit = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyHerbalismKit", "9345e1fd-ec4c-4509-acb5-3f3257b25ec4")
                .SetGuiPresentation("CraftyFeats/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
                .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.HerbalismKitType.Name)
                .AddToDB();

            var crafty_manacalon_rosary = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyManacalonRosary", "0685a944-76cd-423a-81a1-9ceec507d69a")
                .SetGuiPresentation("CraftyFeats/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
                .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.EnchantingToolType.Name)
                .AddToDB();

            var crafty_poisoners_kit = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyPoisonersKit", "32ddae84-66e7-4b56-b5ec-0ec91a713e2e")
                .SetGuiPresentation("CraftyFeats/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
                .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.PoisonersKitType.Name)
                .AddToDB();

            var crafty_scroll_kit = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyScrollKit", "5309bd7f-b533-40ff-ae95-d977e02d61fe")
                .SetGuiPresentation("CraftyFeats/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
                .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.ScrollKitType.Name)
                .AddToDB();

            var crafty_smiths_tools = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftySmithsTools", "48905450-4b35-480f-9868-f340c7902920")
                .SetGuiPresentation("CraftyFeats/&CraftyToolsTitle", "Feature/&ToolProficiencyPluralShortDescription")
                .SetProficiencies(ProficiencyType.ToolOrExpertise, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
                .AddToDB();

            var crafty_bows = FeatureDefinitionProficiencyBuilder
                .Create(ProficiencyAllLanguages, "CraftyBows", "62a71277-b62d-41e6-9546-19f6faa2b5a7")
                .SetGuiPresentation("CraftyFeats/&CraftyBowsTitle", "CraftyFeats/&CraftyBowsDescription")
                .SetProficiencies(ProficiencyType.Weapon, ShortbowType.Name, LongbowType.Name, LightCrossbowType.Name, HeavyCrossbowType.Name)
                .AddToDB();

            var apothecaryIntFeat = FeatDefinitionBuilder
                .Create(ArmorMaster, "ApothecaryInt", "ef387249-45e0-4899-aadd-44810f8aeb6d")
                .SetGuiPresentation("ApothecaryIntFeat", Category.CraftyFeats)
                .SetFeatures(crafty_int, crafty_herbalism_kit, crafty_herbalism_kit)
                .AddToDB();

            var apothecaryWisFeat = FeatDefinitionBuilder
                .Create(ArmorMaster, "ApothecaryWis", "4fd80bf9-7749-4c01-9d95-6eb56c644fe2")
                .SetGuiPresentation("ApothecaryWisFeat", Category.CraftyFeats)
                .SetFeatures(crafty_wis, crafty_herbalism_kit, crafty_medicine)
                .AddToDB();

            var manacalonCrafter = FeatDefinitionBuilder
                .Create(MasterEnchanter, "ManacalonCrafter", "290f73c8-201c-489e-bdcb-7a39ab40915c")
                .SetGuiPresentation("CraftyFeats/&ManacalonCrafterFeatTitle", "CraftyFeats/&ManacalonCrafterFeatDescription")
                .SetFeatures(crafty_int, crafty_manacalon_rosary, crafty_arcana)
                .AddToDB();

            var toxicologistInt = FeatDefinitionBuilder
                .Create(ArmorMaster, "ToxicologistInt", "702d1b4d-953c-406d-a900-d5d376ed29d3")
                .SetGuiPresentation("CraftyFeats/&ToxicologistIntFeatTitle", "CraftyFeats/&ToxicologistIntFeatDescription")
                .SetFeatures(crafty_int, crafty_poisoners_kit, crafty_nature)
                .AddToDB();

            var toxicologistWis = FeatDefinitionBuilder
                .Create(ArmorMaster, "ToxicologistWis", "1bb4acbd-1890-48ae-9f86-46c2cb95cb79")
                .SetGuiPresentation("CraftyFeats/&ToxicologistWisFeatTitle", "CraftyFeats/&ToxicologistWisFeatDescription")
                .SetFeatures(crafty_wis, crafty_poisoners_kit, crafty_medicine)
                .AddToDB();

            var craftyScribe = FeatDefinitionBuilder
                .Create(MasterEnchanter, "CraftyScribe", "bd83e063-2751-4898-8070-f74ca925f8b5")
                .SetGuiPresentation("CraftyFeats/&CraftyScribeFeatTitle", "CraftyFeats/&CraftyScribeFeatDescription")
                .SetFeatures(crafty_int, crafty_scroll_kit, crafty_arcana)
                .AddToDB();

            var craftyFletcher = FeatDefinitionBuilder
                .Create(ArmorMaster, "CraftyFletcher", "67c5f2d2-a98c-49a1-a1ab-16cc8f4b4ba4")
                .SetGuiPresentation("CraftyFeats/&CraftyFletcherFeatTitle", "CraftyFeats/&CraftyFletcherFeatDescription")
                .SetFeatures(crafty_dex, crafty_smiths_tools, crafty_bows)
                .AddToDB();

            feats.AddRange(apothecaryIntFeat, apothecaryWisFeat, manacalonCrafter, toxicologistInt, toxicologistWis, craftyScribe, craftyFletcher);
        }
    }
}
