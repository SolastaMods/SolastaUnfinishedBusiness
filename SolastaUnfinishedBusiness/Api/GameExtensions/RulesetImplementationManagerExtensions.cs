using System.Runtime.CompilerServices;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class RulesetImplementationManagerExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RulesetEffectPower MyInstantiateEffectPower(
        this RulesetImplementationManager rulesetImplementationManager,
        RulesetCharacter user,
        RulesetUsablePower usablePower,
        bool delayRegistration)
    {
        var rulesetEffectPower = rulesetImplementationManager.InstantiateEffectPower(
            user, usablePower, delayRegistration);

        user.PowersUsedByMe.TryAdd(rulesetEffectPower);

        return rulesetEffectPower;
    }
}
