using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class DoNotTerminateWhileUnconscious : IShouldTerminateEffect
{
    public static readonly IShouldTerminateEffect Marker = new DoNotTerminateWhileUnconscious();

    private DoNotTerminateWhileUnconscious()
    {
    }

    public bool Validate(RulesetEffect rulesetEffect)
    {
        var user = EffectHelpers.GetCharacterByGuid(rulesetEffect.SourceGuid);

        return user is not { IsUnconscious: true };
    }
}
