namespace SolastaUnfinishedBusiness.ModelLoader.Data.VertexData;

public struct Normal(float x, float y, float z)
{
    public float X { get; private set; } = x;
    public float Y { get; private set; } = y;
    public float Z { get; private set; } = z;
}
