using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Behaviors;

//Currently implemented only for reaction restoration of ActionAffinity features
public abstract class FeatureUseLimiter
{
    public static readonly FeatureUseLimiter OncePerTurn = new OncePerTurn();
    public abstract bool CanBeUsed(GameLocationCharacter character, FeatureDefinition feature);
}

internal class OncePerTurn : FeatureUseLimiter
{
    public override bool CanBeUsed(GameLocationCharacter character, FeatureDefinition feature)
    {
        return character.OncePerTurnIsValid(feature.Name);
    }
}
