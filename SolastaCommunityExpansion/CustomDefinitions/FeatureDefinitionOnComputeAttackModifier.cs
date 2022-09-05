using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

/**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
public sealed class FeatureDefinitionOnComputeAttackModifier : FeatureDefinition, IOnComputeAttackModifier
{
    private OnComputeAttackModifier afterComputeAttackModifier;

    public void ComputeAttackModifier(
        RulesetCharacter myself,
        RulesetCharacter defender,
        RulesetAttackMode attackMode,
        ref ActionModifier attackModifier)
    {
        afterComputeAttackModifier?.Invoke(myself, defender, attackMode, ref attackModifier);
    }

    internal void SetOnRollAttackModeDelegate([CanBeNull] OnComputeAttackModifier del = null)
    {
        afterComputeAttackModifier = del;
    }
}
