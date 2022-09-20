using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

public static class GroupFeats
{
    private static readonly List<FeatDefinition> Groups = new();
    private static readonly List<FeatDefinition> Children = new();

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildElementalTouchGroup());
        feats.Add(BuildCreedGroup());
        feats.AddRange(Groups);

        //TODO: should this be an option on mod UI?
        // remove children from mod UI selection
        FeatsContext.Feats.RemoveWhere(x => Children.Contains(x));
    }

    public static FeatDefinition MakeGroup(string name, string family, params FeatDefinition[] feats)
    {
        return MakeGroup(name, family, feats.ToList());
    }

    public static FeatDefinition MakeGroup(string name, string family, List<FeatDefinition> feats)
    {
        var group = FeatDefinitionBuilder
            .Create(name, DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(feats))
            .SetFeatFamily(family)
            .SetFeatures()
            .AddToDB();
        Groups.Add(group);
        Children.AddRange(feats);
        return group;
    }

    private static FeatDefinition BuildElementalTouchGroup()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupElementalTouch", DefinitionBuilder.CENamespaceGuid)
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


    private static FeatDefinition BuildCreedGroup()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupCreed", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(
                FeatDefinitions.Creed_Of_Arun,
                FeatDefinitions.Creed_Of_Einar,
                FeatDefinitions.Creed_Of_Maraike,
                FeatDefinitions.Creed_Of_Misaye,
                FeatDefinitions.Creed_Of_Pakri,
                FeatDefinitions.Creed_Of_Solasta
            ))
            .SetFeatures()
            .AddToDB();
    }
}
