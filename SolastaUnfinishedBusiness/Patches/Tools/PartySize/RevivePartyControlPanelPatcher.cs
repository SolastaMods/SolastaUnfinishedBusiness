using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches.Tools.PartySize;

//PATCH: scales down the revive party control panel whenever the party size is bigger than 4
//
// this patch is protected by partyCount result
//
[HarmonyPatch(typeof(RevivePartyControlPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RevivePartyControlPanel_OnBeginShow
{
    internal static void Prefix([NotNull] RevivePartyControlPanel __instance)
    {
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
