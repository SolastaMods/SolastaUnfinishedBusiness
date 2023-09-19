using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnReducedToZeroHpByEnemy
{
    public IEnumerator HandleReducedToZeroHpByEnemy(
        GameLocationCharacter attacker,
        GameLocationCharacter source,
        [UsedImplicitly] RulesetAttackMode attackMode,
        [UsedImplicitly] RulesetEffect activeEffect);
}
