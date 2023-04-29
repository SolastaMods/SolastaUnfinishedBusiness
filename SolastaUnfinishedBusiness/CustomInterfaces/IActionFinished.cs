using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IActionFinished
{
    public IEnumerator Execute(CharacterAction action);
}
