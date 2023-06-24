using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinishedOnMe
{
    public IEnumerator OnActionFinishedOnMe(GameLocationCharacter me, CharacterAction action);
}
