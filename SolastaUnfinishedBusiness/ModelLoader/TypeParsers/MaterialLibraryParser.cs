using SolastaUnfinishedBusiness.ModelLoader.Loaders;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class MaterialLibraryParser(IMaterialLibraryLoaderFacade libraryLoaderFacade)
    : TypeParserBase, IMaterialLibraryParser
{
    protected override string Keyword => "mtllib";

    public override void Parse(string line)
    {
        libraryLoaderFacade.Load(line);
    }
}
