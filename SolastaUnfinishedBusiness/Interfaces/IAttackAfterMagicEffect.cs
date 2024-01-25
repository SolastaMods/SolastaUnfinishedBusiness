using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IAttackAfterMagicEffect
{
    public delegate bool CanAttackHandler(GameLocationCharacter caster, GameLocationCharacter target);

    public delegate bool CanUseHandler(
        CursorLocationSelectTarget targeting,
        GameLocationCharacter caster,
        GameLocationCharacter target,
        out string failure);

    [CanBeNull]
    public delegate IEnumerable<CharacterActionParams> GetAttackAfterUseHandler(
        CharacterActionMagicEffect actionMagicEffect);

    public CanUseHandler CanBeUsedToAttack { get; }
    public GetAttackAfterUseHandler PerformAttackAfterUse { get; }
    public CanAttackHandler CanAttack { get; }
}
