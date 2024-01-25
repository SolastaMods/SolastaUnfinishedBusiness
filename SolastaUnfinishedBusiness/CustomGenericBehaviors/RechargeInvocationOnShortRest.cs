namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class RechargeInvocationOnShortRest
{
    private RechargeInvocationOnShortRest()
    {
    }

    public static RechargeInvocationOnShortRest Marker { get; } = new();
}
