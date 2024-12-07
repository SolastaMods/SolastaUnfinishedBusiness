namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public class MaterialLibraryLoaderFacade(
    IMaterialLibraryLoader loader,
    IMaterialStreamProvider materialStreamProvider)
    : IMaterialLibraryLoaderFacade
{
    public void Load(string materialFileName)
    {
        using var stream = materialStreamProvider.Open(materialFileName);
        if (stream != null)
        {
            loader.Load(stream);
        }
    }
}
