using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches
{
    // this patch tweaks the UI to allow less/more heroes to be selected on a campaign
    [HarmonyPatch(typeof(NewAdventurePanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class NewAdventurePanel_OnBeginShow
    {
        internal static void Prefix(RectTransform ___characterSessionPlatesTable)
        {
            // overrides campaign party size
            DatabaseHelper.CampaignDefinitions.UserCampaign.SetPartySize<CampaignDefinition>(Main.Settings.UserDungeonsPartySize);

            // adds new character plates if required
            for (int i = Settings.GAME_PARTY_SIZE; i < Main.Settings.UserDungeonsPartySize; i++)
            {
                var firstChild = ___characterSessionPlatesTable.GetChild(0);

                Object.Instantiate(firstChild, firstChild.parent);
            }

            // scales down the plates table if required
            if (Main.Settings.UserDungeonsPartySize > Settings.GAME_PARTY_SIZE)
            {
                var scale = (float)System.Math.Pow(Settings.ADVENTURE_PANEL_DEFAULT_SCALE, Main.Settings.UserDungeonsPartySize - Settings.GAME_PARTY_SIZE);

                ___characterSessionPlatesTable.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                ___characterSessionPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
