using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMagicEffectInitiatedByMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectInitiatedByMe(
        CharacterAction action,
        RulesetEffect activeEffect,
        GameLocationCharacter attacker,
        List<GameLocationCharacter> targets);
}
