namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class AllowConditionDuplicates
{
    private AllowConditionDuplicates()
    {
    }

    public static AllowConditionDuplicates Mark { get; } = new();
}
