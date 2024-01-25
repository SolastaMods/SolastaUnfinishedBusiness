using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IOnReducedToZeroHpByMeOrAlly
{
    [UsedImplicitly]
    public IEnumerator HandleReducedToZeroHpByMeOrAlly(
        GameLocationCharacter attacker,
        GameLocationCharacter downedCreature,
        GameLocationCharacter ally,
        RulesetAttackMode attackMode,
        RulesetEffect activeEffect);
}
