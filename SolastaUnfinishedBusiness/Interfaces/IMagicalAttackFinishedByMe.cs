using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any magical attack regardless of an attack roll or not
public interface IMagicalAttackFinishedByMe
{
    [UsedImplicitly]
    IEnumerator OnMagicalAttackFinishedByMe(
        CharacterActionMagicEffect action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender);
}
