using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

public static class GroupFeats
{
    private static readonly List<FeatDefinition> Groups = new();

    public static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        feats.Add(BuildElementalTouchGroup());
        feats.Add(BuildCreedGroup());
        feats.AddRange(Groups);
    }

    public static FeatDefinition MakeGroup(string name, string family, params FeatDefinition[] feats)
    {
        return MakeGroup(name, family, feats.ToList());
    }

    public static FeatDefinition MakeGroup(FeatDefinition group)
    {
        Groups.Add(group);

        return group;
    }

    public static FeatDefinition MakeGroup(FeatDefinition group, params FeatDefinition[] feats)
    {
        group.SetCustomSubFeatures(new GroupedFeat(feats));

        return MakeGroup(group);
    }

    public static FeatDefinition MakeGroup(string name, string family, List<FeatDefinition> feats)
    {
        var group = FeatDefinitionBuilder
            .Create(name)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(new GroupedFeat(feats))
            .SetFeatFamily(family)
            .SetFeatures()
            .AddToDB();

        Groups.Add(group);

        return group;
    }

    private static FeatDefinition BuildElementalTouchGroup()
    {
        return FeatDefinitionBuilder
            .Create("FeatGroupElementalTouch")
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
            .Create("FeatGroupCreed")
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
