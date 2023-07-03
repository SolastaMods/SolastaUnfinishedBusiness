using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnActionFinishedByMe(CharacterAction characterAction);
}
