using System.Collections.Generic;
using SolastaUnfinishedBusiness.ModelLoader.Data;
using SolastaUnfinishedBusiness.ModelLoader.Data.Elements;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;

namespace SolastaUnfinishedBusiness.ModelLoader.Loaders;

public class LoadResult
{
    public IList<Vertex> Vertices { get; set; }
    public IList<Texture> Textures { get; set; }
    public IList<Normal> Normals { get; set; }
    public IList<Group> Groups { get; set; }
    public IList<Material> Materials { get; set; }
}
