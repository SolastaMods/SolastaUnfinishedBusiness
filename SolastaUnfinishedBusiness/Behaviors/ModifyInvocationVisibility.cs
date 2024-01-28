namespace SolastaUnfinishedBusiness.Behaviors;

internal class ModifyInvocationVisibility
{
    private ModifyInvocationVisibility()
    {
    }

    public static ModifyInvocationVisibility Marker { get; } = new();
}
