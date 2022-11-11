using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorCategoryDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ArmorFeats
{
    internal static void CreateArmorFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyFeatLightArmor = BuildProficiency("ProficiencyFeatLightArmor",
            ProficiencyType.Armor, EquipmentDefinitions.LightArmorCategory);

        var proficiencyFeatMediumArmor = BuildProficiency("ProficiencyFeatMediumArmor",
            ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory);

        var featLightArmor = BuildFeat("FeatLightArmor", null,
            proficiencyFeatLightArmor,
            AttributeModifierCreed_Of_Misaye);

        var featMediumArmorDex = BuildFeat("FeatMediumArmorDex", LightArmorCategory,
            proficiencyFeatMediumArmor,
            AttributeModifierCreed_Of_Misaye);

        var featMediumArmorStr = BuildFeat("FeatMediumArmorStr", LightArmorCategory,
            proficiencyFeatMediumArmor,
            AttributeModifierCreed_Of_Einar);

        // sounds too OP
#if false
// Feat/&FeatHeavyArmorMasterDescription=You gain resistance to bludgeoning, slashing, and piercing damage.
// Feat/&FeatHeavyArmorMasterTitle=Heavy Defense Mastery
        var featHeavyArmorMaster = BuildFeat("FeatHeavyArmorMaster", HeavyArmorCategory,
            DamageAffinityBludgeoningResistance,
            DamageAffinitySlashingResistance,
            DamageAffinityPiercingResistance);
#endif

        feats.AddRange(featLightArmor, featMediumArmorDex, featMediumArmorStr);

        _ = GroupFeats.MakeGroup("FeatGroupMediumArmor", "MediumArmor",
            featMediumArmorDex,
            featMediumArmorStr);
    }

    private static FeatDefinition BuildFeat(
        string name,
        ArmorCategoryDefinition prerequisite,
        params FeatureDefinition[] features)
    {
        return FeatDefinitionBuilder
            .Create(name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(features)
            .SetArmorProficiencyPrerequisite(prerequisite)
            .AddToDB();
    }

    private static FeatureDefinitionProficiency BuildProficiency(
        string name,
        ProficiencyType type,
        params string[] proficiencies)
    {
        return FeatureDefinitionProficiencyBuilder
            .Create(name)
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(type, proficiencies)
            .AddToDB();
    }
}
