namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class ModifyInvocationVisibility
{
    private ModifyInvocationVisibility()
    {
    }

    public static ModifyInvocationVisibility Marker { get; } = new();
}
