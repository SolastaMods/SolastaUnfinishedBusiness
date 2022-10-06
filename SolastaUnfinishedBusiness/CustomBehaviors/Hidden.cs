namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class Hidden
{
    private Hidden()
    {
    }

    public static Hidden Marker { get; } = new();
}
