using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackInitiatedOnMe
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackInitiatedOnMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode);
}
