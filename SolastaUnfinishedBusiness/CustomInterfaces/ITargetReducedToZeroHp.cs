using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ITargetReducedToZeroHp
{
    public IEnumerator HandleCharacterReducedToZeroHp(GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode, RulesetEffect activeEffect);
}
