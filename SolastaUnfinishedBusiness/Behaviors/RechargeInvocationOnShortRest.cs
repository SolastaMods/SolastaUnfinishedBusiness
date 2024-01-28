namespace SolastaUnfinishedBusiness.Behaviors;

public class RechargeInvocationOnShortRest
{
    private RechargeInvocationOnShortRest()
    {
    }

    public static RechargeInvocationOnShortRest Marker { get; } = new();
}
