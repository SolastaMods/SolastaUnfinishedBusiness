using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackInitiatedOnMeOrAlly
{
    [UsedImplicitly]
    IEnumerator OnPhysicalAttackInitiatedOnMeOrAlly(
        GameLocationBattleManager __instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter ally,
        ActionModifier attackModifier,
        RulesetAttackMode attackerAttackMode);
}
