using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class UseMaterialParser(IElementGroup elementGroup) : TypeParserBase, IUseMaterialParser
{
    protected override string Keyword => "usemtl";

    public override void Parse(string line)
    {
        elementGroup.SetMaterial(line);
    }
}
