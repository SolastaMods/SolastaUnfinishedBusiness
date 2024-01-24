namespace SolastaUnfinishedBusiness.CustomBehaviors;

public class RestrictRecurrentEffectsOnSelfTurnOnly
{
    private RestrictRecurrentEffectsOnSelfTurnOnly()
    {
    }

    public static RestrictRecurrentEffectsOnSelfTurnOnly Mark { get; } = new();
}
