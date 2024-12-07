using System.Collections.Generic;
using SolastaUnfinishedBusiness.ModelLoader.Data.Elements;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;

namespace SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;

public interface IDataStore
{
    IList<Vertex> Vertices { get; }
    IList<Texture> Textures { get; }
    IList<Normal> Normals { get; }
    IList<Material> Materials { get; }
    IList<Group> Groups { get; }
}
