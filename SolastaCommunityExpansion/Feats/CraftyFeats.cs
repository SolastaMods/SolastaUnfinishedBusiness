using System.Collections.Generic;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Feats
{
    public static class CraftyFeats
    {
        public class CraftyFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
        {
            protected CraftyFeatBuilder(string name, string guid, string title, string description, FeatDefinition base_Feat) : base(base_Feat, name, guid)
            {
                if (title != "")
                {
                    Definition.GuiPresentation.Title = title;
                }
                if (description != "")
                {
                    Definition.GuiPresentation.Description = description;
                }
            }
            public static FeatDefinition CreateCopyFrom(string name, string guid, string title, string description, FeatDefinition base_Feat)
            {
                return new CraftyFeatBuilder(name, guid, title, description, base_Feat).AddToDB();
            }
        }

        internal static void CreateFeats(List<FeatDefinition> feats)
        {
            FeatureDefinitionAttributeModifier crafty_int = BuildNewAttributeModifier(
                "AttributeModifierFeatCraftyInt",
                "b23c3b73-7690-42ba-aa49-7ca3451daa05",
                "SolastaCraftyFeats/&AttributeIntTitle",
                "SolastaCraftyFeats/&AttributeIntDescription",
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Pakri.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Pakri);

            FeatureDefinitionAttributeModifier crafty_wis = BuildNewAttributeModifier(
                "AttributeModifierFeatCraftyWis",
                "23f944c7-2359-43cc-8bdc-71833bf35302",
                "SolastaCraftyFeats/&AttributeWisTitle",
                "SolastaCraftyFeats/&AttributeWisDescription",
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Maraike.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Maraike);

            FeatureDefinitionAttributeModifier crafty_dex = BuildNewAttributeModifier(
                "AttributeModifierFeatCraftyDex",
                "4db12466-67da-47a4-8d96-a9bf9cf3a251",
                "SolastaCraftyFeats/&AttributeDexTitle",
                "SolastaCraftyFeats/&AttributeDexDescription",
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Misaye.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Misaye);

            FeatureDefinitionProficiency crafty_arcana = BuildNewProficiency(
                "CraftyArcana",
                "44a54666-80ba-475c-90b1-774e86f1a69a",
                "SolastaCraftyFeats/&CraftySkillsTitle",
                "SolastaCraftyFeats/&CraftyArcanaDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.SkillOrExpertise,
                new List<string> { DatabaseHelper.SkillDefinitions.Arcana.name }
                );

            FeatureDefinitionProficiency crafty_medicine = BuildNewProficiency(
                "CraftyMedicine",
                "1ac54869-a8ce-4a51-a858-1f7e34680b96",
                "SolastaCraftyFeats/&CraftySkillsTitle",
                "SolastaCraftyFeats/&CraftyMedicineDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.SkillOrExpertise,
                new List<string> { DatabaseHelper.SkillDefinitions.Medecine.name }
                );

            FeatureDefinitionProficiency crafty_nature = BuildNewProficiency(
                "CraftyNature",
                "7399b06a-bfda-4e60-8366-17e0d6cec0d0",
                "SolastaCraftyFeats/&CraftySkillsTitle",
                "SolastaCraftyFeats/&CraftyNatureDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.SkillOrExpertise,
                new List<string> { DatabaseHelper.SkillDefinitions.Nature.name }
                );

            FeatureDefinitionProficiency crafty_herbalism_kit = BuildNewProficiency(
                "CraftyHerbalismKit",
                "9345e1fd-ec4c-4509-acb5-3f3257b25ec4",
                "SolastaCraftyFeats/&CraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string> { DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.name }
                );

            FeatureDefinitionProficiency crafty_manacalon_rosary = BuildNewProficiency(
                "CraftyManacalonRosary",
                "0685a944-76cd-423a-81a1-9ceec507d69a",
                "SolastaCraftyFeats/&CraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string> { DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.name }
                );

            FeatureDefinitionProficiency crafty_poisoners_kit = BuildNewProficiency(
                "CraftyPoisonersKit", "32ddae84-66e7-4b56-b5ec-0ec91a713e2e",
                "SolastaCraftyFeats/&CraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string> { DatabaseHelper.ToolTypeDefinitions.PoisonersKitType.name }
                );

            FeatureDefinitionProficiency crafty_scroll_kit = BuildNewProficiency(
                "CraftyScrollKit",
                "5309bd7f-b533-40ff-ae95-d977e02d61fe",
                "SolastaCraftyFeats/&CraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string> { DatabaseHelper.ToolTypeDefinitions.ScrollKitType.name }
                );

            FeatureDefinitionProficiency crafty_smiths_tools = BuildNewProficiency(
                "CraftySmithsTools", "48905450-4b35-480f-9868-f340c7902920",
                "SolastaCraftyFeats/&CraftyToolsTitle",
                "Feature/&ToolProficiencyPluralShortDescription",
                null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string> { DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType.name }
                );

            FeatureDefinitionProficiency crafty_bows = BuildNewProficiency(
                "CraftyBows",
                "62a71277-b62d-41e6-9546-19f6faa2b5a7",
                "SolastaCraftyFeats/&CraftyBowsTitle",
                "SolastaCraftyFeats/&CraftyBowsDescription",
                 null,
                DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyAllLanguages,
                RuleDefinitions.ProficiencyType.Weapon,
                new List<string>
                {
                    DatabaseHelper.WeaponTypeDefinitions.ShortbowType.name,
                    DatabaseHelper.WeaponTypeDefinitions.LongbowType.name,
                    DatabaseHelper.WeaponTypeDefinitions.LightCrossbowType.name,
                    DatabaseHelper.WeaponTypeDefinitions.HeavyCrossbowType.name
                });

            FeatDefinition ApothecaryIntFeat = CraftyFeatBuilder.CreateCopyFrom(
                "ApothecaryInt", "ef387249-45e0-4899-aadd-44810f8aeb6d",
                "SolastaCraftyFeats/&ApothecaryIntFeatTitle", "SolastaCraftyFeats/&ApothecaryIntFeatDescription",
                DatabaseHelper.FeatDefinitions.ArmorMaster);

            ApothecaryIntFeat.Features.Clear();
            ApothecaryIntFeat.Features.Add(crafty_int);
            ApothecaryIntFeat.Features.Add(crafty_herbalism_kit);
            ApothecaryIntFeat.Features.Add(crafty_arcana);
            feats.Add(ApothecaryIntFeat);

            FeatDefinition ApothecaryWisFeat = CraftyFeatBuilder.CreateCopyFrom
                ("ApothecaryWis", "4fd80bf9-7749-4c01-9d95-6eb56c644fe2",
                "SolastaCraftyFeats/&ApothecaryWisFeatTitle", "SolastaCraftyFeats/&ApothecaryWisFeatDescription",
                DatabaseHelper.FeatDefinitions.ArmorMaster);

            ApothecaryWisFeat.Features.Clear();
            ApothecaryWisFeat.Features.Add(crafty_wis);
            ApothecaryWisFeat.Features.Add(crafty_herbalism_kit);
            ApothecaryWisFeat.Features.Add(crafty_medicine);
            feats.Add(ApothecaryWisFeat);

            FeatDefinition ManacalonCrafter = CraftyFeatBuilder.CreateCopyFrom
                ("ManacalonCrafter", "290f73c8-201c-489e-bdcb-7a39ab40915c",
                "SolastaCraftyFeats/&ManacalonCrafterFeatTitle", "SolastaCraftyFeats/&ManacalonCrafterFeatDescription",
                DatabaseHelper.FeatDefinitions.MasterEnchanter);

            ManacalonCrafter.Features.Clear();
            ManacalonCrafter.Features.Add(crafty_int);
            ManacalonCrafter.Features.Add(crafty_manacalon_rosary);
            ManacalonCrafter.Features.Add(crafty_arcana);
            feats.Add(ManacalonCrafter);

            FeatDefinition ToxicologistInt = CraftyFeatBuilder.CreateCopyFrom
                ("ToxicologistInt", "702d1b4d-953c-406d-a900-d5d376ed29d3",
                "SolastaCraftyFeats/&ToxicologistIntFeatTitle", "SolastaCraftyFeats/&ToxicologistIntFeatDescription",
                DatabaseHelper.FeatDefinitions.ArmorMaster);

            ToxicologistInt.Features.Clear();
            ToxicologistInt.Features.Add(crafty_int);
            ToxicologistInt.Features.Add(crafty_poisoners_kit);
            ToxicologistInt.Features.Add(crafty_nature);
            feats.Add(ToxicologistInt);

            FeatDefinition ToxicologistWis = CraftyFeatBuilder.CreateCopyFrom
                ("ToxicologistWis", "1bb4acbd-1890-48ae-9f86-46c2cb95cb79",
                "SolastaCraftyFeats/&ToxicologistWisFeatTitle", "SolastaCraftyFeats/&ToxicologistWisFeatDescription",
                DatabaseHelper.FeatDefinitions.ArmorMaster);

            ToxicologistWis.Features.Clear();
            ToxicologistWis.Features.Add(crafty_wis);
            ToxicologistWis.Features.Add(crafty_poisoners_kit);
            ToxicologistWis.Features.Add(crafty_medicine);
            feats.Add(ToxicologistWis);

            FeatDefinition CraftyScribe = CraftyFeatBuilder.CreateCopyFrom
                ("CraftyScribe", "bd83e063-2751-4898-8070-f74ca925f8b5",
                "SolastaCraftyFeats/&CraftyScribeFeatTitle", "SolastaCraftyFeats/&CraftyScribeFeatDescription",
                DatabaseHelper.FeatDefinitions.MasterEnchanter);

            CraftyScribe.Features.Clear();
            CraftyScribe.Features.Add(crafty_int);
            CraftyScribe.Features.Add(crafty_scroll_kit);
            CraftyScribe.Features.Add(crafty_arcana);
            feats.Add(CraftyScribe);

            FeatDefinition CraftyFletcher = CraftyFeatBuilder.CreateCopyFrom
                ("CraftyFletcher", "67c5f2d2-a98c-49a1-a1ab-16cc8f4b4ba4",
                "SolastaCraftyFeats/&CraftyFletcherFeatTitle", "SolastaCraftyFeats/&CraftyFletcherFeatDescription",
                DatabaseHelper.FeatDefinitions.ArmorMaster);
            CraftyFletcher.Features.Clear();
            CraftyFletcher.Features.Add(crafty_dex);
            CraftyFletcher.Features.Add(crafty_smiths_tools);
            CraftyFletcher.Features.Add(crafty_bows);
            feats.Add(CraftyFletcher);
        }

        public class CopyAndCreateNewBlueprint<TDefinition> : BaseDefinitionBuilder<TDefinition> where TDefinition : BaseDefinition
        {
            protected CopyAndCreateNewBlueprint(string name, string guid, string title, string description, AssetReferenceSprite sprite_reference, TDefinition base_Blueprint) : base(base_Blueprint, name, guid)
            {
                Definition.GuiPresentation.SetTitle(title);
                Definition.GuiPresentation.SetDescription(description);
                Definition.GuiPresentation.SetSpriteReference(sprite_reference);
            }

            public static TDefinition CreateCopy(string name, string guid, string title, string description, AssetReferenceSprite sprite_reference, TDefinition base_Blueprint)
            {
                return new CopyAndCreateNewBlueprint<TDefinition>(name, guid, title, description, sprite_reference, base_Blueprint).AddToDB();
            }
        }

        public static FeatureDefinitionAttributeModifier BuildNewAttributeModifier(string name, string guid, string title, string description, AssetReferenceSprite sprite_reference, FeatureDefinitionAttributeModifier baseAttributeModifier)
        {
            return CopyAndCreateNewBlueprint<FeatureDefinitionAttributeModifier>.CreateCopy(
                name,
                guid,
                title,
                description,
                sprite_reference,
                baseAttributeModifier
            );
        }

        public static FeatureDefinitionProficiency BuildNewProficiency(string name, string guid, string title, string description, AssetReferenceSprite sprite_reference, FeatureDefinitionProficiency baseProficiency, RuleDefinitions.ProficiencyType ProficiencyType, List<string> Proficiencies)
        {
            var unit = CopyAndCreateNewBlueprint<FeatureDefinitionProficiency>.CreateCopy(
                name,
                guid,
                title,
                description,
                sprite_reference,
                baseProficiency
            );

            unit.SetProficiencyType(ProficiencyType);
            unit.Proficiencies.Clear();
            unit.Proficiencies.AddRange(Proficiencies);

            return unit;
        }
    }
}
