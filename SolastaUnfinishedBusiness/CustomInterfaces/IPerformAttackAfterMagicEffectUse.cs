using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IPerformAttackAfterMagicEffectUse
{
    delegate bool CanAttackHandler(GameLocationCharacter caster, GameLocationCharacter target);

    delegate bool CanUseHandler(CursorLocationSelectTarget targeting, GameLocationCharacter caster,
        GameLocationCharacter target, out string failure);

    delegate List<CharacterActionParams> GetAttackAfterUseHandler(CharacterActionMagicEffect actionMagicEffect);

    CanUseHandler CanBeUsedToAttack { get; }
    GetAttackAfterUseHandler PerformAttackAfterUse { get; }
    CanAttackHandler CanAttack { get; }
}
