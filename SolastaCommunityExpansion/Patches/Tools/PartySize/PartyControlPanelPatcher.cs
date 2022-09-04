using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize;

//PATCH: scales down the party control panel whenever the party size is bigger than 4
//
// this patch is protected by partyCount result
//
[HarmonyPatch(typeof(PartyControlPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class PartyControlPanel_OnBeginShow
{
    internal static void Prefix([NotNull] PartyControlPanel __instance)
    {
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
internal static class PartyControlPanel_Refresh
{
    internal static void Postfix(PartyControlPanel __instance)
    {
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
