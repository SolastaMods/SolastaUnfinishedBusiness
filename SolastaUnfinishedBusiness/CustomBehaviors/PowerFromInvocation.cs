namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class PowerFromInvocation : PowerVisibilityModifier
{
    public static PowerVisibilityModifier Marker { get; } = new PowerFromInvocation();
    private PowerFromInvocation() : base((_, _, _) => false)
    {
    }

}
