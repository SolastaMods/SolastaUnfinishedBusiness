using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomMagicEffectAction
{
    IEnumerator ProcessCustomEffect(CharacterActionMagicEffect action);
}
