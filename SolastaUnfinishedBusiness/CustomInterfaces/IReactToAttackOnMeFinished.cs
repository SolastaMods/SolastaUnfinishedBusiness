using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToAttackOnMeFinished
{
    [UsedImplicitly]
    public IEnumerator OnReactToAttackOnMeFinished(
        GameLocationCharacter attacker,
        GameLocationCharacter me,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
