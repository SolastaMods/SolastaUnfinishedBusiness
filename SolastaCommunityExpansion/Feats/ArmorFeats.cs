using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static RuleDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ArmorCategoryDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaCommunityExpansion.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;

namespace SolastaCommunityExpansion.Feats;

internal static class ArmorFeats
{
    private static readonly Guid ArmorNamespace = new("d37cf3a0-6dbe-461f-8af5-58761414ef6b");

    public static void CreateArmorFeats([NotNull] List<FeatDefinition> feats)
    {
        var lightArmorProficiency = BuildProficiency("FeatLightArmorProficiency",
            ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory);

        var mediumArmorProficiency = BuildProficiency("FeatMediumArmorProficiency",
            ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory);

        var lightArmorFeat = BuildFeat("FeatLightArmor", lightArmorProficiency, AttributeModifierCreed_Of_Misaye);

        // Note: medium armor feats have pre-req of light armor
        var mediumDexArmorFeat = BuildFeat("FeatMediumArmorDex", LightArmorCategory, mediumArmorProficiency,
            AttributeModifierCreed_Of_Misaye);
        var mediumStrengthArmorFeat = BuildFeat("FeatMediumArmorStrength", LightArmorCategory,
            mediumArmorProficiency, AttributeModifierCreed_Of_Einar);

        // Note: heavy armor master has pre-req of heavy armor
        var heavyArmorMasterFeat = BuildFeat("FeatHeavyArmorMasterClass", HeavyArmorCategory,
            DamageAffinityBludgeoningResistance, DamageAffinitySlashingResistance,
            DamageAffinityPiercingResistance);

        feats.AddRange(lightArmorFeat, mediumDexArmorFeat, mediumStrengthArmorFeat, heavyArmorMasterFeat);
    }

    private static FeatDefinition BuildFeat(string name, ArmorCategoryDefinition prerequisite,
        params FeatureDefinition[] features)
    {
        return FeatDefinitionBuilder
            .Create(name, ArmorNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(features)
            .SetArmorProficiencyPrerequisite(prerequisite)
            .AddToDB();
    }

    private static FeatDefinition BuildFeat(string name, params FeatureDefinition[] features)
    {
        return FeatDefinitionBuilder
            .Create(name, ArmorNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(features)
            .AddToDB();
    }

    private static FeatureDefinitionProficiency BuildProficiency(string name, ProficiencyType type,
        params string[] proficiencies)
    {
        return FeatureDefinitionProficiencyBuilder
            .Create(name, ArmorNamespace)
            .SetGuiPresentation(Category.Feat)
            .SetProficiencies(type, proficiencies)
            .AddToDB();
    }
}
