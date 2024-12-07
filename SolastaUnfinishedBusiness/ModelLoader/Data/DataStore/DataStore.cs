using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.Data.Elements;
using SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;

namespace SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;

public class DataStore : IDataStore, IGroupDataStore, IVertexDataStore, ITextureDataStore, INormalDataStore,
    IFaceGroup, IMaterialLibrary, IElementGroup
{
    private readonly List<Group> _groups = [];
    private readonly List<Material> _materials = [];
    private readonly List<Normal> _normals = [];
    private readonly List<Texture> _textures = [];

    private readonly List<Vertex> _vertices = [];
    private Group _currentGroup;

    public IList<Vertex> Vertices => _vertices;

    public IList<Texture> Textures => _textures;

    public IList<Normal> Normals => _normals;

    public IList<Material> Materials => _materials;

    public IList<Group> Groups => _groups;

    public void SetMaterial(string materialName)
    {
        var material = _materials.SingleOrDefault(x => x.Name.EqualsOrdinalIgnoreCase(materialName));
        PushGroupIfNeeded();
        _currentGroup.Material = material;
    }

    public void AddFace(Face face)
    {
        PushGroupIfNeeded();

        _currentGroup.AddFace(face);
    }

    public void PushGroup(string groupName)
    {
        _currentGroup = new Group(groupName);
        _groups.Add(_currentGroup);
    }

    public void Push(Material material)
    {
        _materials.Add(material);
    }

    public void AddNormal(Normal normal)
    {
        _normals.Add(normal);
    }

    public void AddTexture(Texture texture)
    {
        _textures.Add(texture);
    }

    public void AddVertex(Vertex vertex)
    {
        _vertices.Add(vertex);
    }

    private void PushGroupIfNeeded()
    {
        if (_currentGroup == null)
        {
            PushGroup("default");
        }
    }
}
