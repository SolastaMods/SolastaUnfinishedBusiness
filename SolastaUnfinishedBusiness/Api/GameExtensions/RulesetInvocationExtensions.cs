using System.Linq;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class RulesetInvocationExtensions
{
    public static bool IsAvailable(this RulesetInvocation invocation, RulesetCharacter user)
    {
        var definition = invocation.invocationDefinition;
        return definition.GrantedSpell &&
               ((!definition.ConsumesSpellSlot && !definition.LongRestRecharge) ||
                ((!definition.LongRestRecharge || !invocation.used) &&
                 (!definition.ConsumesSpellSlot || user.spellRepertoires.Any(invocation.IsSpellSlotAvailableToUse))));
    }
}
