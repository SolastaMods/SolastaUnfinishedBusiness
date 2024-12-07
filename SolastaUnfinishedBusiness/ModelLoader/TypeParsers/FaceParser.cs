using System;
using SolastaUnfinishedBusiness.ModelLoader.Common;
using SolastaUnfinishedBusiness.ModelLoader.Data.DataStore;
using SolastaUnfinishedBusiness.ModelLoader.Data.Elements;
using SolastaUnfinishedBusiness.ModelLoader.TypeParsers.Interfaces;

namespace SolastaUnfinishedBusiness.ModelLoader.TypeParsers;

public class FaceParser(IFaceGroup faceGroup) : TypeParserBase, IFaceParser
{
    protected override string Keyword => "f";

    public override void Parse(string line)
    {
        var vertices = line.Split([' '], StringSplitOptions.RemoveEmptyEntries);

        var face = new Face();

        foreach (var vertexString in vertices)
        {
            var faceVertex = ParseFaceVertex(vertexString);
            face.AddVertex(faceVertex);
        }

        faceGroup.AddFace(face);
    }

    private static FaceVertex ParseFaceVertex(string vertexString)
    {
        var fields = vertexString.Split(['/'], StringSplitOptions.None);

        var vertexIndex = fields[0].ParseInvariantInt();
        var faceVertex = new FaceVertex(vertexIndex, 0, 0);

        if (fields.Length > 1)
        {
            var textureIndex = fields[1].Length == 0 ? 0 : fields[1].ParseInvariantInt();
            faceVertex.TextureIndex = textureIndex;
        }

        // ReSharper disable once InvertIf
        if (fields.Length > 2)
        {
            var normalIndex = fields.Length > 2 && fields[2].Length == 0 ? 0 : fields[2].ParseInvariantInt();
            faceVertex.NormalIndex = normalIndex;
        }

        return faceVertex;
    }
}
