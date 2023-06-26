using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinishedByEnemy
{
    [UsedImplicitly] public ActionDefinition ActionDefinition { get; }

    [UsedImplicitly]
    public IEnumerator OnActionFinishedByEnemy(GameLocationCharacter target, CharacterAction characterAction);
}
