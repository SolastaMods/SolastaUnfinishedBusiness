namespace SolastaUnfinishedBusiness.CustomValidators;

public class ForcePowerUseInSpendPowerAction
{
    private ForcePowerUseInSpendPowerAction()
    {
    }

    public static ForcePowerUseInSpendPowerAction Marker { get; } = new();
}
