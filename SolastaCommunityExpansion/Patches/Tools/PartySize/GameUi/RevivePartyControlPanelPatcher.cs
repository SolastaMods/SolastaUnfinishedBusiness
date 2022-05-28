using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize.GameUi
{
    // this patch scales down the revive party control panel whenever the party size is bigger than 4
    //
    // this patch is protected by partyCount result
    //
    [HarmonyPatch(typeof(RevivePartyControlPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RevivePartyControlPanel_OnBeginShow
    {
        internal static void Prefix(RevivePartyControlPanel __instance)
        {
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > DungeonMakerContext.GAME_PARTY_SIZE)
            {
                var scale = (float)Math.Pow(DungeonMakerContext.REVIVE_PARTY_CONTROL_PANEL_DEFAULT_SCALE,
                    partyCount - DungeonMakerContext.GAME_PARTY_SIZE);

                __instance.partyPlatesTable.localScale = new Vector3(scale, 1, scale);
            }
            else
            {
                __instance.partyPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
