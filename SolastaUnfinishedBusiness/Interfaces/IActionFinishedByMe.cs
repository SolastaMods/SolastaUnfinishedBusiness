using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IActionFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnActionFinishedByMe(CharacterAction characterAction);
}
