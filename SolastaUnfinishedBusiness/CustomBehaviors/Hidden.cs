namespace SolastaUnfinishedBusiness.CustomValidators;

internal class Hidden
{
    private Hidden()
    {
    }

    public static Hidden Marker { get; } = new();
}
