namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

public class RestrictRecurrentEffectsOnSelfTurnOnly
{
    private RestrictRecurrentEffectsOnSelfTurnOnly()
    {
    }

    public static RestrictRecurrentEffectsOnSelfTurnOnly Mark { get; } = new();
}
