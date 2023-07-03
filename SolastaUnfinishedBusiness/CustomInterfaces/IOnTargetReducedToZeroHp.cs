using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnTargetReducedToZeroHp
{
    public IEnumerator HandleCharacterReducedToZeroHp(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
