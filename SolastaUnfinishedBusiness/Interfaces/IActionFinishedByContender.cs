using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IActionFinishedByContender
{
    [UsedImplicitly]
    public IEnumerator OnActionFinishedByContender(CharacterAction characterAction, GameLocationCharacter target);
}
