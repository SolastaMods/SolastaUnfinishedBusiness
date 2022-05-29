using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize.GameUi
{
    // this patch scales down the rest sub panel whenever the party size is bigger than 4
    //
    // this patch is protected by partyCount result
    //
    [HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RestSubPanel_OnBeginShow
    {
        internal static void Prefix(RestSubPanel __instance)
        {
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > DungeonMakerContext.GAME_PARTY_SIZE)
            {
                var scale = (float)Math.Pow(DungeonMakerContext.REST_PANEL_DEFAULT_SCALE,
                    partyCount - DungeonMakerContext.GAME_PARTY_SIZE);

                __instance.restModulesTable.localScale = new Vector3(scale, scale, scale);
                __instance.characterPlatesTable.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                __instance.restModulesTable.localScale = new Vector3(1, 1, 1);
                __instance.characterPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
