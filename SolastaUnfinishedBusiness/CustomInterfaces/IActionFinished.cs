using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinished
{
    [UsedImplicitly]
    public IEnumerator OnActionFinished(CharacterAction characterAction);
}
