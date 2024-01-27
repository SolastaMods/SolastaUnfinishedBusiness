namespace SolastaUnfinishedBusiness.Behaviors;

public sealed class AllowFreeWeaponSwitching
{
    private AllowFreeWeaponSwitching()
    {
    }

    public static AllowFreeWeaponSwitching Mark { get; } = new();
}
