using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMagicEffectFinishedOnMeAny
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectFinishedOnMeAny(
        CharacterActionMagicEffect action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        List<GameLocationCharacter> targets);
}
