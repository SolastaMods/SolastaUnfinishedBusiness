using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnReducedToZeroHpMe
{
    public IEnumerator HandleReducedToZeroHpMe(
        GameLocationCharacter attacker,
        GameLocationCharacter source,
        [UsedImplicitly] RulesetAttackMode attackMode,
        [UsedImplicitly] RulesetEffect activeEffect);
}
