using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IUsePowerFinished
{
    [UsedImplicitly]
    public IEnumerator OnUsePowerFinished(CharacterActionUsePower characterActionUsePower);
}
