using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyAttackOutcome
{
    [UsedImplicitly]
    public void OnAttackOutcome(
        [NotNull] RulesetCharacter __instance,
        ref int __result,
        int toHitBonus,
        RulesetActor target,
        BaseDefinition attackMethod,
        List<RuleDefinitions.TrendInfo> toHitTrends,
        bool ignoreAdvantage,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool rangeAttack,
        bool opportunity,
        int rollModifier,
        ref RuleDefinitions.RollOutcome outcome,
        ref int successDelta,
        int predefinedRoll,
        bool testMode,
        ActionDefinitions.ReactionCounterAttackType reactionCounterAttackType);
}
