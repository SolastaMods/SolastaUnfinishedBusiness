using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPhysicalAttackInitiatedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackInitiatedOnMeOrAlly(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier attackModifier,
        RulesetAttackMode attackMode);
}
