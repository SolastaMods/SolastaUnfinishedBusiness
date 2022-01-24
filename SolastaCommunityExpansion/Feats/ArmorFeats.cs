using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Features;
using SolastaModApi.Infrastructure;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaModApi.DatabaseHelper.ArmorCategoryDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaCommunityExpansion.Feats
{
    internal static class ArmorFeats
    {
        public static readonly Guid ArmorNamespace = new Guid("d37cf3a0-6dbe-461f-8af5-58761414ef6b");

        public static void CreateArmorFeats(List<FeatDefinition> feats)
        {
            var lightArmorProficiency = BuildProficiency("FeatLightArmorProficiency",
                ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory);

            var mediumArmorProficiency = BuildProficiency("FeatMediumArmorProficiency",
                ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory);

            var dexterityModifier = BuildAttributeModifier("FeatDexIncrement",
                AttributeModifierOperation.Additive, AttributeDefinitions.Dexterity, 1);

            // Note: this originally had AttributeDefinitions.Dexterity
            var strengthModifier = BuildAttributeModifier("FeatStrengthIncrement",
                AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1);

            var lightArmorFeat = BuildFeat("FeatLightArmor", lightArmorProficiency, dexterityModifier);

            // Note: Medium armor feats have prereq of LightArmorCategory
            var mediumDexArmorFeat = BuildFeat("FeatMediumArmorDex", LightArmorCategory, mediumArmorProficiency, dexterityModifier);
            var mediumStrengthArmorFeat = BuildFeat("FeatMediumArmorStrength", LightArmorCategory, mediumArmorProficiency, strengthModifier);

            // but heavy armor feat has prereq HeavyArmorCategory - is this correct?
            var heavyArmorFeat = BuildFeat("FeatHeavyArmorMasterClass", HeavyArmorCategory,
                DamageAffinityBludgeoningResistance, DamageAffinitySlashingResistance, DamageAffinityPiercingResistance);

            feats.AddRange(lightArmorFeat, mediumDexArmorFeat, mediumStrengthArmorFeat, heavyArmorFeat);
        }

        public static FeatDefinition BuildFeat(string name, ArmorCategoryDefinition prerequisite, params FeatureDefinition[] features)
        {
            return new FeatDefinitionBuilder(name, ArmorNamespace, "Feat", features).SetArmorProficiencyPrerequisite(prerequisite).AddToDB();
        }

        public static FeatDefinition BuildFeat(string name, params FeatureDefinition[] features)
        {
            return new FeatDefinitionBuilder(name, ArmorNamespace, "Feat", features).AddToDB();
        }

        public static FeatureDefinitionProficiency BuildProficiency(string name, ProficiencyType type, params string[] proficiencies)
        {
            return new FeatureDefinitionProficiencyBuilder(name, ArmorNamespace, type, proficiencies.AsEnumerable(), "Feat").AddToDB();
        }

        public static FeatureDefinitionAttributeModifier BuildAttributeModifier(string name, AttributeModifierOperation modifierType, string attribute, int amount)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, ArmorNamespace, modifierType, attribute, amount, "Feat").AddToDB();
        }
    }
}
