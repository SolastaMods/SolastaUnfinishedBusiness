using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IAttackAfterMagicEffect
{
    public delegate bool CanAttackHandler(GameLocationCharacter caster, GameLocationCharacter target);

    public delegate bool CanUseHandler(
        CursorLocationSelectTarget targeting,
        GameLocationCharacter caster,
        GameLocationCharacter target,
        out string failure);

    public delegate List<CharacterActionParams> GetAttackAfterUseHandler(CharacterActionMagicEffect actionMagicEffect);

    public CanUseHandler CanBeUsedToAttack { get; }
    public GetAttackAfterUseHandler PerformAttackAfterUse { get; }
    public CanAttackHandler CanAttack { get; }
}
