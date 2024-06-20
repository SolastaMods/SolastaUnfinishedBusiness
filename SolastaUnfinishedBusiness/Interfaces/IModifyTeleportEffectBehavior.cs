using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyTeleportEffectBehavior
{
    bool AllyOnly { get; }

    [UsedImplicitly]
    int MaxTargets(CursorLocationSelectTarget cursorLocationSelectTarget);
}
