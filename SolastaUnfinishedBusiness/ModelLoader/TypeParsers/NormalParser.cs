using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class NormalParser(INormalDataStore normalDataStore) : TypeParserBase, INormalParser
{
    protected override string Keyword => "vn";

    public override void Parse(string line)
    {
        var parts = line.Split(' ');

        var x = parts[0].ParseInvariantFloat();
        var y = parts[1].ParseInvariantFloat();
        var z = parts[2].ParseInvariantFloat();

        var normal = new Normal(x, y, z);
        normalDataStore.AddNormal(normal);
    }
}
