using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnReducedToZeroHpByMeOrAlly
{
    public IEnumerator HandleReducedToZeroHpByMeOrAlly(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        GameLocationCharacter ally,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
