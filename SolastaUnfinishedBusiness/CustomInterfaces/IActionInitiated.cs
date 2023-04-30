using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionInitiated
{
    [UsedImplicitly]
    IEnumerator OnActionInitiated(CharacterAction characterAction);
}
