using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IGroupedFeat
{
    List<FeatDefinition> GetSubFeats();
}

public class GroupedFeat : IGroupedFeat
{
    private readonly List<FeatDefinition> feats = new();

    public GroupedFeat(params FeatDefinition[] feats) : this(feats.ToList())
    {
    }

    public GroupedFeat(IEnumerable<FeatDefinition> feats)
    {
        this.feats.AddRange(feats);
        this.feats.Sort(FeatsContext.CompareFeats);
    }

    public List<FeatDefinition> GetSubFeats()
    {
        return feats;
    }
}
