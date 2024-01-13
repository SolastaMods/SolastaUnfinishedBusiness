using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IFilterTargetingPosition
{
    public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition);
}
