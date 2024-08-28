using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any magical attack regardless of an attack roll or not
public interface IMagicEffectFinishedByMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectFinishedByMe(
        CharacterAction action,
        GameLocationCharacter attacker,
        List<GameLocationCharacter> targets);
}
