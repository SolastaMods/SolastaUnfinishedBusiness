using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnDefenderFailedSavingThrow
{
    IEnumerator OnDefenderFailedSavingThrow(
        GameLocationBattleManager __instance,
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck
    );
}
