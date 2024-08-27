using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

// triggers on any magical attack regardless of an attack roll or not
public interface IMagicEffectFinishedByMeOrAlly
{
    [UsedImplicitly]
    public IEnumerator OnMagicEffectFinishedByMeOrAlly(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter helper,
        List<GameLocationCharacter> targets);
}
