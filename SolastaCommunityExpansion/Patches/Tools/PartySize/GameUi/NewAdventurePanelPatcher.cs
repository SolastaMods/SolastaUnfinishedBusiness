using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Patches.Tools.DefaultParty;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize.GameUi;

// this patch tweaks the UI to allow less/more heroes to be selected on a campaign
//
// this shouldn't be protected
//
[HarmonyPatch(typeof(NewAdventurePanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NewAdventurePanel_OnBeginShow
{
    internal static void Prefix(NewAdventurePanel __instance)
    {
        NewAdventurePanel_Refresh.ShouldAssignDefaultParty = true;

        // overrides campaign party size
        DatabaseHelper.CampaignDefinitions.UserCampaign.partySize = Main.Settings.OverridePartySize;

        // adds new character plates if required
        for (var i = DungeonMakerContext.GAME_PARTY_SIZE; i < Main.Settings.OverridePartySize; i++)
        {
            var firstChild = __instance.characterSessionPlatesTable.GetChild(0);

            Object.Instantiate(firstChild, firstChild.parent);
        }

        // scales down the plates table if required
        if (Main.Settings.OverridePartySize > DungeonMakerContext.GAME_PARTY_SIZE)
        {
            var scale = (float)Math.Pow(DungeonMakerContext.ADVENTURE_PANEL_DEFAULT_SCALE,
                Main.Settings.OverridePartySize - DungeonMakerContext.GAME_PARTY_SIZE);

            __instance.characterSessionPlatesTable.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            __instance.characterSessionPlatesTable.localScale = new Vector3(1, 1, 1);
        }
    }
}
