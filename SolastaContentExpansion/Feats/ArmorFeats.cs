using SolastaContentExpansion.Features;
using SolastaModApi;
using System;
using System.Collections.Generic;

namespace SolastaContentExpansion.Feats
{
    class ArmorFeats
    {
        public static Guid ArmorNamespace = new Guid("d37cf3a0-6dbe-461f-8af5-58761414ef6b");

        public static void CreateArmorFeats(List<FeatDefinition> feats)
        {
            // Light Armor prof
            GuiPresentationBuilder lightProfPresentation = new GuiPresentationBuilder(
                "Feat/&ProfLightArmorDescription",
                "Feat/&ProfLightArmorTitle");
            FeatureDefinition lightProf = BuildProficiency(RuleDefinitions.ProficiencyType.Armor, new List<string>()
            {
                EquipmentDefinitions.LightArmorCategory.ToString()
            }, "FeatLightArmorProficiency", lightProfPresentation.Build()
            );
            GuiPresentationBuilder dexPresentation = new GuiPresentationBuilder(
                "Feat/&FeatDexIncrementDescription",
                "Feat/&FeatDexIncrementTitle");
            FeatureDefinition dexIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Dexterity, 1, "FeatDexIncrement", dexPresentation.Build());

            GuiPresentationBuilder lightPresentation = new GuiPresentationBuilder(
                "Feat/&LightArmorDescription",
                "Feat/&LightArmorTitle");
            FeatDefinitionBuilder lightArmor = new FeatDefinitionBuilder("FeatLightArmor", GuidHelper.Create(ArmorNamespace, "FeatLightArmor").ToString(),
                new List<FeatureDefinition>()
            {
                lightProf,
                dexIncrement,
            }, lightPresentation.Build());
            feats.Add(lightArmor.AddToDB());

            // Medium Armor prof (dex)
            GuiPresentationBuilder mediumProfPresentation = new GuiPresentationBuilder(
                "Feat/&ProfMediumArmorDescription",
                "Feat/&ProfMediumArmorTitle");
            FeatureDefinition mediumProf = BuildProficiency(RuleDefinitions.ProficiencyType.Armor, new List<string>()
            {
                EquipmentDefinitions.MediumArmorCategory.ToString(),
                EquipmentDefinitions.ShieldCategory.ToString(),
            }, "FeatMediumArmorProficiency", mediumProfPresentation.Build()
            );

            GuiPresentationBuilder mediumDexPresentation = new GuiPresentationBuilder(
                "Feat/&MediumDexArmorDescription",
                "Feat/&MediumDexArmorTitle");
            FeatDefinitionBuilder mediumDexArmor = new FeatDefinitionBuilder("FeatMediumArmorDex", GuidHelper.Create(ArmorNamespace, "FeatMediumArmorDex").ToString(),
                new List<FeatureDefinition>()
            {
                mediumProf,
                dexIncrement,
            }, mediumDexPresentation.Build());
            mediumDexArmor.SetArmorProficiencyPrerequisite(DatabaseHelper.ArmorCategoryDefinitions.LightArmorCategory);
            feats.Add(mediumDexArmor.AddToDB());

            // Medium Armor prof (strength)
            GuiPresentationBuilder strengthPresentation = new GuiPresentationBuilder(
                "Feat/&FeatStrengthIncrementDescription",
                "Feat/&FeatStrengthIncrementTitle");
            FeatureDefinition strengthIncrement = BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.Dexterity, 1, "FeatStrengthIncrement", strengthPresentation.Build());

            GuiPresentationBuilder mediumStrengthPresentation = new GuiPresentationBuilder(
                "Feat/&MediumStrengthArmorDescription",
                "Feat/&MediumStrengthArmorTitle");
            FeatDefinitionBuilder mediumSrengthArmor = new FeatDefinitionBuilder("FeatMediumArmorStrength", GuidHelper.Create(ArmorNamespace, "FeatMediumArmorStrength").ToString(),
                new List<FeatureDefinition>()
            {
                mediumProf,
                strengthIncrement,
            }, mediumStrengthPresentation.Build());
            mediumSrengthArmor.SetArmorProficiencyPrerequisite(DatabaseHelper.ArmorCategoryDefinitions.LightArmorCategory);
            feats.Add(mediumSrengthArmor.AddToDB());

            GuiPresentationBuilder heavyArmorMaster = new GuiPresentationBuilder(
                "Feat/&HeavyArmorMasterDescription",
                "Feat/&HeavyArmorMasterTitle");
            FeatDefinitionBuilder heavyArmor = new FeatDefinitionBuilder("FeatHeavyArmorMasterClass", GuidHelper.Create(ArmorNamespace, "FeatHeavyArmorMasterClass").ToString(),
                new List<FeatureDefinition>()
            {
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance,
                DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance,
            }, heavyArmorMaster.Build());
            heavyArmor.SetArmorProficiencyPrerequisite(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory);
            feats.Add(heavyArmor.AddToDB());
        }

        public static FeatureDefinitionProficiency BuildProficiency(RuleDefinitions.ProficiencyType type,
            List<string> proficiencies, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionProficiencyBuilder builder = new FeatureDefinitionProficiencyBuilder(name, GuidHelper.Create(ArmorNamespace, name).ToString(), type, proficiencies, guiPresentation);
            return builder.AddToDB();
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation modifierType,
                string attribute, int amount, string name, GuiPresentation guiPresentation)
        {
            FeatureDefinitionAttributeModifierBuilder builder = new FeatureDefinitionAttributeModifierBuilder(name, GuidHelper.Create(ArmorNamespace, name).ToString(),
               modifierType, attribute, amount, guiPresentation);
            return builder.AddToDB();
        }
    }
}
