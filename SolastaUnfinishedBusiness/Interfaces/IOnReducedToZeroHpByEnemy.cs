using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IOnReducedToZeroHpByEnemy
{
    public IEnumerator HandleReducedToZeroHpByEnemy(
        GameLocationCharacter attacker,
        GameLocationCharacter source,
        [UsedImplicitly] RulesetAttackMode attackMode,
        [UsedImplicitly] RulesetEffect activeEffect);
}
