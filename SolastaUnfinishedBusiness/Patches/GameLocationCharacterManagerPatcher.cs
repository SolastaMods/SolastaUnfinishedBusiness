﻿using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationCharacterManagerPatcher
{
    //PATCH: recalculates additional party members positions (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationCharacterManager), "UnlockCharactersForLoading")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UnlockCharactersForLoading_Patch
    {
        internal static void Prefix([NotNull] GameLocationCharacterManager __instance)
        {
            var partyCharacters = __instance.PartyCharacters;

            for (var idx = DungeonMakerContext.GamePartySize; idx < partyCharacters.Count; idx++)
            {
                var position = partyCharacters[idx % DungeonMakerContext.GamePartySize].LocationPosition;

                partyCharacters[idx].LocationPosition = new int3(position.x, position.y, position.z);
            }
        }
    }
}
