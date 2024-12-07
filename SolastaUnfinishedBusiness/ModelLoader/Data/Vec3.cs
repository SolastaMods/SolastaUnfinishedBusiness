namespace SolastaUnfinishedBusiness.ModelLoader.Data;

public struct Vec3(float x, float y, float z)
{
    public float X { get; private set; } = x;
    public float Y { get; private set; } = y;
    public float Z { get; private set; } = z;
}
