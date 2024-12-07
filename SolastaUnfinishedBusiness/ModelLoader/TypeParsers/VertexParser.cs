using System;
using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class VertexParser(IVertexDataStore vertexDataStore) : TypeParserBase, IVertexParser
{
    protected override string Keyword => "v";

    public override void Parse(string line)
    {
        var parts = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        var x = parts[0].ParseInvariantFloat();
        var y = parts[1].ParseInvariantFloat();
        var z = parts[2].ParseInvariantFloat();

        var vertex = new Vertex(x, y, z);
        vertexDataStore.AddVertex(vertex);
    }
}
