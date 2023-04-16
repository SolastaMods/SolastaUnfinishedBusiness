namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class PowerFromInvocation : PowerVisibilityModifier
{
    private PowerFromInvocation() : base((_, _, _) => false)
    {
    }

    public static PowerVisibilityModifier Marker { get; } = new PowerFromInvocation();
}
