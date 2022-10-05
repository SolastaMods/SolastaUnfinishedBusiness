using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Patches;

public static class NewAdventurePanelPatcher
{
    //PATCH: tweaks the UI to allow less/more heroes to be selected on a campaign (PARTYSIZE)
    [HarmonyPatch(typeof(NewAdventurePanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] NewAdventurePanel __instance)
        {
            // overrides campaign party size
            DatabaseRepository.GetDatabase<CampaignDefinition>()
                .Do(x => x.partySize = Main.Settings.OverridePartySize);

            // adds new character plates if required
            for (var i = DungeonMakerContext.GamePartySize; i < Main.Settings.OverridePartySize; i++)
            {
                var firstChild = __instance.characterSessionPlatesTable.GetChild(0);

                Object.Instantiate(firstChild, firstChild.parent);
            }

            // scales down the plates table if required
            if (Main.Settings.OverridePartySize > DungeonMakerContext.GamePartySize)
            {
                var scale = (float)Math.Pow(DungeonMakerContext.AdventurePanelDefaultScale,
                    Main.Settings.OverridePartySize - DungeonMakerContext.GamePartySize);

                __instance.characterSessionPlatesTable.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                __instance.characterSessionPlatesTable.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
