using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

public static class FeatGroups
{
    public const string ElementalTouch = "FeatElementalTouch";
    public const string AddFightingStyle = "FeatAddFightingStyle";

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildElementalTouch());
        feats.Add(BuildAddFightingStyle());
    }

    private static FeatDefinition BuildElementalTouch()
    {
        return FeatDefinitionBuilder
            .Create(ElementalTouch, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.BurningTouch,
                FeatDefinitions.ToxicTouch,
                FeatDefinitions.ElectrifyingTouch,
                FeatDefinitions.IcyTouch,
                FeatDefinitions.MeltingTouch
            ))
            .SetFeatFamily(FeatDefinitions.BurningTouch.FamilyTag)
            .SetFeatures()
            .AddToDB();
    }
    
    private static FeatDefinition BuildAddFightingStyle()
    {
        return FeatDefinitionBuilder
            .Create(AddFightingStyle, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(FightingStyleFeats.Feats))
            .SetFeatures()
            .AddToDB();
    }
}