using System.Collections.Generic;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**
 * Can be used to trigger Additional Damage feature with custom checks
 */
public abstract class CustomAdditionalDamage(IAdditionalDamageProvider provider)
{
    public IAdditionalDamageProvider Provider { get; } = provider;

    [UsedImplicitly]
    internal abstract bool IsValid(
        GameLocationBattleManager battleManager,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode,
        bool rangedAttack,
        AdvantageType advantageType,
        List<EffectForm> actualEffectForms,
        RulesetEffect rulesetEffect,
        bool criticalHit,
        bool firstTarget,
        out CharacterActionParams reactionParams);
}
