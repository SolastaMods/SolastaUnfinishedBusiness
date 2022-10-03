using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class PartyControlPanelPatcher
{
    [HarmonyPatch(typeof(PartyControlPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] PartyControlPanel __instance)
        {
            //PATCH: scales down the party control panel whenever the party size is bigger than 4 (PARTYSIZE)
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount > DungeonMakerContext.GamePartySize)
            {
                var scale = DungeonMakerContext.GetPartyControlScale();
                __instance.partyPlatesTable.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                __instance.partyPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    [HarmonyPatch(typeof(PartyControlPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Refresh_Patch
    {
        public static void Postfix(PartyControlPanel __instance)
        {
            //PATCH: scales down the party control panel whenever the party size is bigger than 4 (PARTYSIZE)
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

            if (partyCount <= DungeonMakerContext.GamePartySize)
            {
                return;
            }

            var scale = DungeonMakerContext.GetPartyControlScale();
            var y = 10f + (scale * __instance.partyPlatesTable.rect.height);
            var guestPlatesTable = __instance.guestPlatesTable;

            guestPlatesTable.anchoredPosition = new Vector2(guestPlatesTable.anchoredPosition.x, -y);
        }
    }
}
