using System.Collections;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IFilterTargetingPosition
{
    public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition);
}
