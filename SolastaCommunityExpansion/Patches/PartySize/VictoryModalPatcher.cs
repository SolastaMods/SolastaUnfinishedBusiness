using HarmonyLib;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.PartySize
{
    // this patch scales down the victory modal whenever the party size is bigger than 4
    [HarmonyPatch(typeof(VictoryModal), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class VictoryModal_OnBeginShow
    {
        internal static void Prefix(RectTransform ___heroStatsGroup)
        {
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > Settings.GAME_PARTY_SIZE)
            {
                float scale = (float)Math.Pow(Settings.VICTORY_MODAL_DEFAULT_SCALE, partyCount - Settings.GAME_PARTY_SIZE);

                ___heroStatsGroup.localScale = new Vector3(scale, 1, scale);
            }
            else
            {
                ___heroStatsGroup.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
