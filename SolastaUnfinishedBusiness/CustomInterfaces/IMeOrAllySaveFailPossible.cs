using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// triggers on player ally save fail
// allows reaction compared with another similar interface
public interface IMeOrAllySaveFailPossible
{
    IEnumerator OnMeOrAllySaveFailPossible(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter featureOwner,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck
            );
}
