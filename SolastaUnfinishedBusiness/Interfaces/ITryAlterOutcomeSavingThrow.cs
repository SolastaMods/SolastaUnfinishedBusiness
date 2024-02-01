using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ITryAlterOutcomeSavingThrow
{
    IEnumerator OnSavingThrowTryAlterOutcome(
        GameLocationBattleManager battleManager,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter featureOwner,
        ActionModifier saveModifier,
        bool hasHitVisual,
        [UsedImplicitly] bool hasBorrowedLuck);
}
