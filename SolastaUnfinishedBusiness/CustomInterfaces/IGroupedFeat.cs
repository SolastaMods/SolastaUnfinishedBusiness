using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IGroupedFeat
{
    public bool HideSubFeats { get; }
    public List<FeatDefinition> GetSubFeats(bool includeHidden = false, bool onlyModded = false);
}

public class GroupedFeat : IGroupedFeat
{
    private readonly List<FeatDefinition> _feats = new();

    public GroupedFeat(IEnumerable<FeatDefinition> feats)
    {
        _feats.AddRange(feats);
        _feats.Sort(FeatsContext.CompareFeats);
    }

    public List<FeatDefinition> GetSubFeats(bool includeHidden = false, bool onlyModded = false)
    {
        return _feats
            .Where(x =>
                (includeHidden || !x.GuiPresentation.hidden) &&
                (!onlyModded || x.ContentPack == CeContentPackContext.CeContentPack))
            .ToList();
    }

    public bool HideSubFeats => true;

    public void AddFeats(params FeatDefinition[] featDefinitions)
    {
        _feats.AddRange(featDefinitions);
    }
}
