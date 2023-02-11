using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IOnDefenderFailedSavingThrow
{
    [UsedImplicitly]
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
