using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ArmorCategoryDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class ArmorFeats
{
    internal static void CreateArmorFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyFeatMediumArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatMediumArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

        var featMediumArmorDex = BuildFeat("FeatMediumArmorDex", LightArmorCategory,
            proficiencyFeatMediumArmor,
            AttributeModifierCreed_Of_Misaye);

        var featMediumArmorStr = BuildFeat("FeatMediumArmorStr", LightArmorCategory,
            proficiencyFeatMediumArmor,
            AttributeModifierCreed_Of_Einar);

        feats.AddRange(featMediumArmorDex, featMediumArmorStr);

        GroupFeats.MakeGroup("FeatGroupArmor", null,
            GroupFeats.MakeGroup("FeatGroupMediumArmor", "MediumArmor",
                featMediumArmorDex,
                featMediumArmorStr),
            ArmorMaster,
            DiscretionOfTheCoedymwarth,
            MightOfTheIronLegion,
            SturdinessOfTheTundra);
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
}
