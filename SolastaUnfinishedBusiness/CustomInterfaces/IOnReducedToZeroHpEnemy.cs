using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnReducedToZeroHpEnemy
{
    public IEnumerator HandleReducedToZeroHpEnemy(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
