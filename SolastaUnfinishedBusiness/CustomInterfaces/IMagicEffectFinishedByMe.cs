using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicEffectFinishedByMe
{
    IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action);
}
