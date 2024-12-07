using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.ModelLoader.Data.Elements;

public class Face
{
    private readonly List<FaceVertex> _vertices = [];

    public FaceVertex this[int i] => _vertices[i];

    public int Count => _vertices.Count;

    public void AddVertex(FaceVertex vertex)
    {
        _vertices.Add(vertex);
    }
}

public struct FaceVertex(int vertexIndex, int textureIndex, int normalIndex)
{
    public int VertexIndex { get; set; } = vertexIndex;
    public int TextureIndex { get; set; } = textureIndex;
    public int NormalIndex { get; set; } = normalIndex;
}
