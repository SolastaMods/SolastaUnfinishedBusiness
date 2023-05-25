using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicalEffectFinished
{
    public IEnumerator OnMagicalEffectFinished(CharacterActionMagicEffect characterActionMagicEffect);
}
