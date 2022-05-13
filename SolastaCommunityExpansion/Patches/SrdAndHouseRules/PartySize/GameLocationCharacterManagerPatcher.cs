using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using TA;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.PartySize
{
    // use this patch to recalculate the additional party members positions
    //
    // this shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationCharacterManager), "UnlockCharactersForLoading")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacterManager_UnlockCharactersForLoading
    {
        internal static void Prefix(GameLocationCharacterManager __instance)
        {
            var partyCharacters = __instance.PartyCharacters;

            for (int idx = DungeonMakerContext.GAME_PARTY_SIZE; idx < partyCharacters.Count; idx++)
            {
                var position = partyCharacters[idx % DungeonMakerContext.GAME_PARTY_SIZE].LocationPosition;

                partyCharacters[idx].LocationPosition = new int3(position.x, position.y, position.z);
            }
        }
    }
}
