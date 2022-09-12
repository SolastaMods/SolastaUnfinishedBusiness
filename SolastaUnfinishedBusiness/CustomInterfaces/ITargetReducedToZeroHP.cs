using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ITargetReducedToZeroHP
{
    public IEnumerator HandleCharacterReducedToZeroHP(GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        RulesetAttackMode attackMode, RulesetEffect activeEffect);
}
