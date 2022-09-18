using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IGroupedFeat
{
    List<FeatDefinition> GetSubFeats();
}

public class GroupedFeat : IGroupedFeat
{
    private readonly List<FeatDefinition> feats = new();

    public GroupedFeat()
    {
        feats.Add(DatabaseHelper.FeatDefinitions.ToxicTouch);
        feats.Add(DatabaseHelper.FeatDefinitions.ElectrifyingTouch);
        feats.Add(DatabaseHelper.FeatDefinitions.IcyTouch);
        feats.Add(DatabaseHelper.FeatDefinitions.MeltingTouch);
    }

    public List<FeatDefinition> GetSubFeats()
    {
        return feats;
    }
}