using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMagicEffectFinishedOnMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectFinishedOnMe(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        List<GameLocationCharacter> targets);
}
