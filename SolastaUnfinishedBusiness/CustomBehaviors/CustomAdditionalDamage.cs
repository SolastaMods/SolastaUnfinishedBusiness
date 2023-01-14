using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**
 * Can be used to trigger Additional Damage feature with custom checks
 */
public abstract class CustomAdditionalDamage
{
    protected CustomAdditionalDamage(IAdditionalDamageProvider provider)
    {
        Provider = provider;
    }

    public IAdditionalDamageProvider Provider { get; }

    internal abstract bool IsValid(GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        RuleDefinitions.AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool criticalHit,
        bool firstTarget,
        out CharacterActionParams reactionParams);
}
