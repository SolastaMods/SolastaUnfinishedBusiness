using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize.GameUi;

public static class RevivePartyControlPanelPatcher
{
    [HarmonyPatch(typeof(RevivePartyControlPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] RevivePartyControlPanel __instance)
        {
            //PATCH: scales down the revive party control panel whenever the party size is bigger than 4 (PARTYSIZE)
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > DungeonMakerContext.GamePartySize)
            {
                var scale = (float)Math.Pow(DungeonMakerContext.RevivePartyControlPanelDefaultScale,
                    partyCount - DungeonMakerContext.GamePartySize);

                __instance.partyPlatesTable.localScale = new Vector3(scale, 1, scale);
            }
            else
            {
                __instance.partyPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
