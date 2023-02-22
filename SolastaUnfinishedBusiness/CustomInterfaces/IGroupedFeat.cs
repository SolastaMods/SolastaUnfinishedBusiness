using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IGroupedFeat
{
    public bool HideSubFeats { get; }
    public List<FeatDefinition> GetSubFeats(bool includeHidden = false);
}

public class GroupedFeat : IGroupedFeat
{
    private readonly List<FeatDefinition> feats = new();

    public GroupedFeat(IEnumerable<FeatDefinition> feats)
    {
        this.feats.AddRange(feats);
        this.feats.Sort(FeatsContext.CompareFeats);
    }

    public List<FeatDefinition> GetSubFeats(bool includeHidden = false)
    {
        return feats.Where(x => includeHidden || !x.GuiPresentation.hidden).ToList();
    }

    public bool HideSubFeats => true;

    public void AddFeats(params FeatDefinition[] featDefinitions)
    {
        feats.AddRange(featDefinitions);
    }
}
