namespace SolastaUnfinishedBusiness.CustomBehaviors;

public sealed class FreeWeaponSwitching
{
    private FreeWeaponSwitching()
    {
    }

    public static FreeWeaponSwitching Mark { get; } = new();
}
