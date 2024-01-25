namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class ForcePowerUseInSpendPowerAction
{
    private ForcePowerUseInSpendPowerAction()
    {
    }

    public static ForcePowerUseInSpendPowerAction Marker { get; } = new();
}
