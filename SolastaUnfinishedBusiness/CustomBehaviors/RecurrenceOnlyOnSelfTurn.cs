namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RecurrenceOnlyOnSelfTurn
{
    public static RecurrenceOnlyOnSelfTurn Mark { get; } = new();

    private RecurrenceOnlyOnSelfTurn()
    {
    }
}
