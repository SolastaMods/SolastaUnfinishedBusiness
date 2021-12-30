using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.PartySize
{
    // this patch scales down the rest sub panel whenever the party size is bigger than 4
    //
    // this patch is protected by partyCount result
    //
    [HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RestSubPanel_OnBeginShow
    {
        internal static void Prefix(RectTransform ___characterPlatesTable, RectTransform ___restModulesTable)
        {
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > DungeonMakerContext.GAME_PARTY_SIZE)
            {
                float scale = (float)Math.Pow(DungeonMakerContext.REST_PANEL_DEFAULT_SCALE, partyCount - DungeonMakerContext.GAME_PARTY_SIZE);

                ___restModulesTable.localScale = new Vector3(scale, scale, scale);
                ___characterPlatesTable.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                ___restModulesTable.localScale = new Vector3(1, 1, 1);
                ___characterPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
