using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifySelectionMaxTargets
{
    [UsedImplicitly]
    int MaxTargets(CursorLocationSelectTarget cursorLocationSelectTarget);
}
