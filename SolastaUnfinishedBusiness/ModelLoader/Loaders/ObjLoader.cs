using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public class ObjLoader : LoaderBase, IObjLoader
{
    private readonly IDataStore _dataStore;
    private readonly List<ITypeParser> _typeParsers = [];

    private readonly List<string> _unrecognizedLines = [];

    public ObjLoader(
        IDataStore dataStore,
        IFaceParser faceParser,
        IGroupParser groupParser,
        INormalParser normalParser,
        ITextureParser textureParser,
        IVertexParser vertexParser,
        IMaterialLibraryParser materialLibraryParser,
        IUseMaterialParser useMaterialParser)
    {
        _dataStore = dataStore;
        SetupTypeParsers(
            vertexParser,
            faceParser,
            normalParser,
            textureParser,
            groupParser,
            materialLibraryParser,
            useMaterialParser);
    }

    public LoadResult Load(Stream lineStream)
    {
        StartLoad(lineStream);

        return CreateResult();
    }

    private void SetupTypeParsers(params ITypeParser[] parsers)
    {
        foreach (var parser in parsers)
        {
            _typeParsers.Add(parser);
        }
    }

    protected override void ParseLine(string keyword, string data)
    {
        foreach (var typeParser in _typeParsers.Where(typeParser => typeParser.CanParse(keyword)))
        {
            typeParser.Parse(data);
            return;
        }

        _unrecognizedLines.Add(keyword + " " + data);
    }

    private LoadResult CreateResult()
    {
        var result = new LoadResult
        {
            Vertices = _dataStore.Vertices,
            Textures = _dataStore.Textures,
            Normals = _dataStore.Normals,
            Groups = _dataStore.Groups,
            Materials = _dataStore.Materials
        };
        return result;
    }
}
