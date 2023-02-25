using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAttackHitPossible
{
    [UsedImplicitly]
    IEnumerator DefenderAttackHitPossible(
        GameLocationBattleManager battle,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect,
        ActionModifier attackModifier,
        int attackRoll
    );
}
