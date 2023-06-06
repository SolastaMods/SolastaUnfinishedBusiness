namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class HiddenInvocation
{
    private HiddenInvocation()
    {
    }

    public static HiddenInvocation Marker { get; } = new();
}
