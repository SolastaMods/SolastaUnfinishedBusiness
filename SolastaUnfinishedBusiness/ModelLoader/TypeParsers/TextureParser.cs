using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class TextureParser(ITextureDataStore textureDataStore) : TypeParserBase, ITextureParser
{
    protected override string Keyword => "vt";

    public override void Parse(string line)
    {
        var parts = line.Split(' ');

        var x = parts[0].ParseInvariantFloat();
        var y = parts[1].ParseInvariantFloat();

        var texture = new Texture(x, y);
        textureDataStore.AddTexture(texture);
    }
}
