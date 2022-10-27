using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ITargetReducedToZeroHp
{
    public IEnumerator HandleCharacterReducedToZeroHp(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode, RulesetEffect activeEffect);
}
