namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RecurrenceOnlyOnSelfTurn
{
    private RecurrenceOnlyOnSelfTurn()
    {
    }

    public static RecurrenceOnlyOnSelfTurn Mark { get; } = new();
}
