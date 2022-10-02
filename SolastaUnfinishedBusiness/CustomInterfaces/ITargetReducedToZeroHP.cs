using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ITargetReducedToZeroHP
{
    internal IEnumerator HandleCharacterReducedToZeroHP(GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode, RulesetEffect activeEffect);
}
