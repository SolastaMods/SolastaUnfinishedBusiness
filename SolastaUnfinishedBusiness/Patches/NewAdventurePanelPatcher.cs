using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

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

            // adds new PlayerInfoGroups if required
            var template = __instance.playersInRoomInfoGroups[3];

            while (__instance.playersInRoomInfoGroups.Count < Main.Settings.OverridePartySize)
            {
                var playerInRoomInfoGroup = Object.Instantiate(template, template.transform.parent);

                __instance.playersInRoomInfoGroups.Add(playerInRoomInfoGroup);
            }

            // adds new character plates if required
            for (var i = DungeonMakerContext.GamePartySize; i < Main.Settings.OverridePartySize; i++)
            {
                var firstChild = __instance.characterSessionPlatesTable.GetChild(0);

                Object.Instantiate(firstChild, firstChild.parent);
            }

            // scales down the plates table if required
            var parentRectTransform = __instance.characterSessionPlatesTable.parent.GetComponent<RectTransform>();

            switch (Main.Settings.OverridePartySize)
            {
                case 6:
                    parentRectTransform.anchoredPosition = new Vector2(45, -78);
                    parentRectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    break;
                case 5:
                    parentRectTransform.anchoredPosition = new Vector2(125, -78);
                    parentRectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    break;
                default:
                    parentRectTransform.anchoredPosition = new Vector2(210, -78);
                    parentRectTransform.localScale = new Vector3(1, 1, 1);
                    break;
            }
        }
    }
}
