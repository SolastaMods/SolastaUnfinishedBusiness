using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IFilterTargetingPosition
{
    public IEnumerator Filter(CursorLocationSelectPosition cursorLocationSelectPosition);
}
