using System.IO;

namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public interface IMaterialLibraryLoader
{
    void Load(Stream lineStream);
}
