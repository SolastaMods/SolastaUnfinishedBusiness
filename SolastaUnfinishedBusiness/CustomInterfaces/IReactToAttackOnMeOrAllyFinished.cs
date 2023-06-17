using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToAttackOnMeOrAllyFinished
{
    [UsedImplicitly]
    public IEnumerator OnReactToAttackOnAllyFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        GameLocationCharacter ally,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
