namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public sealed class AllowFreeWeaponSwitching
{
    private AllowFreeWeaponSwitching()
    {
    }

    public static AllowFreeWeaponSwitching Mark { get; } = new();
}
