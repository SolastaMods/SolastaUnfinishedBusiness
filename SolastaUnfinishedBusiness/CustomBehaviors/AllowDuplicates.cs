namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class AllowDuplicates
{
    public static AllowDuplicates Mark { get; } = new();

    private AllowDuplicates()
    {
    }
}
