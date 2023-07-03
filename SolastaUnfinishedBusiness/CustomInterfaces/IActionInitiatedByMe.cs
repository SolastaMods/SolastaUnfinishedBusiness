using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionInitiatedByMe
{
    [UsedImplicitly]
    IEnumerator OnActionInitiatedByMe(CharacterAction characterAction);
}
