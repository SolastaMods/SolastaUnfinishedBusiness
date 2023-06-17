using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IReactToAttackFinished
{
    [UsedImplicitly]
    public IEnumerator OnReactToAttackFinished(
        GameLocationCharacter me,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
