using System.IO;

namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public interface IObjLoader
{
    LoadResult Load(Stream lineStream);
}
