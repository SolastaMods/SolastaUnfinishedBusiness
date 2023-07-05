using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinishedByEnemy
{
    [UsedImplicitly]
    public IEnumerator OnActionFinishedByEnemy(CharacterAction characterAction, GameLocationCharacter target);
}
