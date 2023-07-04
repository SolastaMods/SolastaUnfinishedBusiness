using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnSourceReducedToZeroHp
{
    public IEnumerator HandleSourceReducedToZeroHp(
        GameLocationCharacter attacker,
        GameLocationCharacter source,
        [UsedImplicitly] RulesetAttackMode attackMode,
        [UsedImplicitly] RulesetEffect activeEffect);
}
