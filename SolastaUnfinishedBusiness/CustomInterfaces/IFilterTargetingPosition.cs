using System.Collections.Generic;
using TA;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IFilterTargetingPosition
{
    public void Filter(CursorLocationSelectPosition __instance, GameLocationCharacter source, List<int3> positions);
}
