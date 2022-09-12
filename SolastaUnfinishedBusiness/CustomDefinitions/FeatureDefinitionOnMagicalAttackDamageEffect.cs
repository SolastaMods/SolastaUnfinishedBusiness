using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

/**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */
public sealed class FeatureDefinitionOnMagicalAttackDamageEffect : FeatureDefinition, IOnMagicalAttackDamageEffect
{
    private OnMagicalAttackDamageDelegate afterOnMagicalAttackDamage;
    private OnMagicalAttackDamageDelegate beforeOnMagicalAttackDamage;

    public void BeforeOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit)
    {
        beforeOnMagicalAttackDamage?.Invoke(attacker, defender, magicModifier, rulesetEffect, actualEffectForms,
            firstTarget, criticalHit);
    }

    public void AfterOnMagicalAttackDamage(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier magicModifier,
        RulesetEffect rulesetEffect,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool criticalHit)
    {
        afterOnMagicalAttackDamage?.Invoke(attacker, defender, magicModifier, rulesetEffect, actualEffectForms,
            firstTarget, criticalHit);
    }

    internal void SetOnMagicalAttackDamageDelegates([CanBeNull] OnMagicalAttackDamageDelegate before = null,
        [CanBeNull] OnMagicalAttackDamageDelegate after = null)
    {
        beforeOnMagicalAttackDamage = before;
        afterOnMagicalAttackDamage = after;
    }
}
