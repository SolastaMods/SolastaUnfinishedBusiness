namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RechargeInvocationOnShortRest
{
    private RechargeInvocationOnShortRest()
    {
    }

    public static RechargeInvocationOnShortRest Marker { get; } = new();
}
