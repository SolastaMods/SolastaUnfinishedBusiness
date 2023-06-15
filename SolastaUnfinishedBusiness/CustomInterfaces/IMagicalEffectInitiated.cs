using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicalEffectInitiated
{
    public IEnumerator OnMagicalEffectInitiated(CharacterActionMagicEffect characterActionMagicEffect);
}
