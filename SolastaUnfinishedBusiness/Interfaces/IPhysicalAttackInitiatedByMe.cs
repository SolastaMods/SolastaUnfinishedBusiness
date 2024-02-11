using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackInitiatedByMe
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackInitiatedByMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode);
}
