using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.PartySize
{
    // this patch scales down the revive party control panel whenever the party size is bigger than 4
    [HarmonyPatch(typeof(RevivePartyControlPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RevivePartyControlPanel_OnBeginShow
    {
        internal static void Prefix(RectTransform ___partyPlatesTable)
        {
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > Settings.GAME_PARTY_SIZE)
            {
                float scale = (float)Math.Pow(Settings.REVIVE_PARTY_CONTROL_PANEL_DEFAULT_SCALE, partyCount - Settings.GAME_PARTY_SIZE);

                ___partyPlatesTable.localScale = new Vector3(scale, 1, scale);
            }
            else
            {
                ___partyPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
