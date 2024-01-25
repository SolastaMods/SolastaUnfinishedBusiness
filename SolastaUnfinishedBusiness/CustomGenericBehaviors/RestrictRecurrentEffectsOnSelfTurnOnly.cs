namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public class RestrictRecurrentEffectsOnSelfTurnOnly
{
    private RestrictRecurrentEffectsOnSelfTurnOnly()
    {
    }

    public static RestrictRecurrentEffectsOnSelfTurnOnly Mark { get; } = new();
}
