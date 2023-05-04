using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ISourceReducedToZeroHp
{
    public IEnumerator HandleSourceReducedToZeroHp(
        GameLocationCharacter attacker,
        GameLocationCharacter source,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
