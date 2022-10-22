using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize;

public static class GameLocationCharacterManagerPatcher
{
    //PATCH: recalculates additional party members positions (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationCharacterManager), "UnlockCharactersForLoading")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UnlockCharactersForLoading_Patch
    {
        public static void Prefix([NotNull] GameLocationCharacterManager __instance)
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
