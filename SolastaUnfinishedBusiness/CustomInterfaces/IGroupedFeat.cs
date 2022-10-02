using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IGroupedFeat
{
    public bool HideSubFeats { get; }
    public List<FeatDefinition> GetSubFeats();
}

internal class GroupedFeat : IGroupedFeat
{
    private readonly List<FeatDefinition> feats = new();

    internal GroupedFeat(params FeatDefinition[] feats) : this(feats.ToList())
    {
    }

    internal GroupedFeat(IEnumerable<FeatDefinition> feats)
    {
        this.feats.AddRange(feats);
        this.feats.Sort(FeatsContext.CompareFeats);
    }

    public List<FeatDefinition> GetSubFeats()
    {
        return feats;
    }

    public bool HideSubFeats { get; } = true;
}
