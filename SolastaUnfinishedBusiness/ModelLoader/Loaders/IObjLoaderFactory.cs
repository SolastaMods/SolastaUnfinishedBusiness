namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public interface IObjLoaderFactory
{
    IObjLoader Create(IMaterialStreamProvider materialStreamProvider);
    IObjLoader Create();
}
