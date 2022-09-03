using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

/**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
public sealed class FeatureDefinitionOnRollAttackMode : FeatureDefinition, IOnRollAttackMode
{
    private OnRollAttackModeDelegate beforeRollAttackMode;

    internal void SetOnRollAttackModeDelegate([CanBeNull] OnRollAttackModeDelegate del = null)
    {
        beforeRollAttackMode = del;
    }

    public void OnRollAttackMode(
        RulesetCharacter attacker,
        ref RulesetAttackMode attackMode,
        RulesetActor target,
        BaseDefinition attackMethod,
        ref List<RuleDefinitions.TrendInfo> toHitTrends,
        bool ignoreAdvantage,
        ref List<RuleDefinitions.TrendInfo> advantageTrends,
        bool opportunity,
        int rollModifier)
    {
        beforeRollAttackMode?.Invoke(attacker, ref attackMode, target, attackMethod, ref toHitTrends, ignoreAdvantage,
            ref advantageTrends, opportunity, rollModifier);
    }
}
