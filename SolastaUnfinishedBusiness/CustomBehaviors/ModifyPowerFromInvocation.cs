namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class ModifyPowerFromInvocation : ModifyPowerVisibility
{
    private ModifyPowerFromInvocation() : base((_, _, _) => false)
    {
    }

    public static ModifyPowerVisibility Marker { get; } = new ModifyPowerFromInvocation();
}
