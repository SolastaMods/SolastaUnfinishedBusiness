using System.Collections;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IOnReducedToZeroHpByMe
{
    public IEnumerator HandleReducedToZeroHpByMe(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
