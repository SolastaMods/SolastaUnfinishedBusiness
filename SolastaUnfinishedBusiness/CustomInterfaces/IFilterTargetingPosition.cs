using System.Collections.Generic;
using TA;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IFilterTargetingPosition
{
    public void EnumerateValidPositions(
        CursorLocationSelectPosition cursorLocationSelectPosition, List<int3> validPositions);
}
