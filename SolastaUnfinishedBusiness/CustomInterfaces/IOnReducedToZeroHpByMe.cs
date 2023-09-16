using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnReducedToZeroHpByMe
{
    public IEnumerator HandleReducedToZeroHpByMe(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
