namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class AllowDuplicates
{
    private AllowDuplicates()
    {
    }

    public static AllowDuplicates Mark { get; } = new();
}
