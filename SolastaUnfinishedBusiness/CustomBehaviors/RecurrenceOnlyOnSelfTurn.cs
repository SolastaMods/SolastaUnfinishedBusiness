namespace SolastaUnfinishedBusiness.CustomValidators;

public class RecurrenceOnlyOnSelfTurn
{
    private RecurrenceOnlyOnSelfTurn()
    {
    }

    public static RecurrenceOnlyOnSelfTurn Mark { get; } = new();
}
