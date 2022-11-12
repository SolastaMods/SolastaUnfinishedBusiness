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
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var proficiencyFeatMediumArmor = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatMediumArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .AddToDB();

        var featMediumArmorDex = FeatDefinitionBuilder
            .Create("FeatMediumArmorDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(proficiencyFeatMediumArmor, AttributeModifierCreed_Of_Misaye)
            .SetArmorProficiencyPrerequisite(LightArmorCategory)
            .AddToDB();

        var featMediumArmorStr = FeatDefinitionBuilder
            .Create("FeatMediumArmorStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(proficiencyFeatMediumArmor, AttributeModifierCreed_Of_Einar)
            .SetArmorProficiencyPrerequisite(LightArmorCategory)
            .AddToDB();

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
}
