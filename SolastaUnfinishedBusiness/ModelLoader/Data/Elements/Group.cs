using System.Collections.Generic;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;

namespace SolastaUnfinishedBusiness.ModelLoader.Data.Elements;

public class Group(string name) : IFaceGroup
{
    private readonly List<Face> _faces = [];

    public string Name { get; private set; } = name;
    public Material Material { get; set; }

    public IList<Face> Faces => _faces;

    public void AddFace(Face face)
    {
        _faces.Add(face);
    }
}
