using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IReactToAttackFinished
{
    IEnumerator HandleReactToAttackFinished(GameLocationCharacter character, GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome, CharacterActionParams actionParams, RulesetAttackMode mode,
        ActionModifier modifier);
}
