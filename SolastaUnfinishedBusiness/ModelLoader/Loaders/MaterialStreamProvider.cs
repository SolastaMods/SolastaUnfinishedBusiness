using System.IO;

namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public class MaterialStreamProvider : IMaterialStreamProvider
{
    public Stream Open(string materialFilePath)
    {
        return File.Open(materialFilePath, FileMode.Open, FileAccess.Read);
    }
}

public class MaterialNullStreamProvider : IMaterialStreamProvider
{
    public Stream Open(string materialFilePath)
    {
        return null;
    }
}
