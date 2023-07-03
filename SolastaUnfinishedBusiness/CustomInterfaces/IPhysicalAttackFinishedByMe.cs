using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPhysicalAttackFinishedByMe
{
    [UsedImplicitly]
    IEnumerator OnAttackFinishedByMe(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetAttackMode attackerAttackMode,
        RuleDefinitions.RollOutcome attackRollOutcome,
        int damageAmount);
}
