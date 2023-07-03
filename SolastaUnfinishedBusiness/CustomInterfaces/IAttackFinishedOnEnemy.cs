using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAttackFinishedOnEnemy
{
    [UsedImplicitly]
    public IEnumerator OnAttackFinishedOnEnemy(
        GameLocationCharacter ally,
        GameLocationCharacter me,
        GameLocationCharacter enemy,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode mode,
        ActionModifier modifier);
}
