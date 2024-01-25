using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NewAdventurePanelPatcher
{
    private static void AssignDefaultHeroes(NewAdventurePanel __instance)
    {
        var max = Math.Min(Main.Settings.DefaultPartyHeroes.Count,
            __instance.characterSessionPlatesTable.childCount);

        __instance.RecreateSession();

        for (var i = 0; i < max; i++)
        {
            var characterPlateSession =
                __instance.characterSessionPlatesTable.GetChild(i).GetComponent<CharacterPlateSession>();

            if (!characterPlateSession.gameObject.activeSelf)
            {
                continue;
            }

            var name = Main.Settings.DefaultPartyHeroes[i];
            var isBuiltIn = ToolsContext.IsBuiltIn(name);
            var filename =
                Path.Combine(
                    !isBuiltIn
                        ? TacticalAdventuresApplication.GameCharactersDirectory
                        : TacticalAdventuresApplication.GameBuiltInCharactersDirectory, name) + ".chr";

            __instance.selectedSlot = i;
            __instance.CharacterSelected(filename);
        }
    }

    //PATCH: tweaks the UI to allow less/more heroes to be selected on a campaign (PARTYSIZE)
    [HarmonyPatch(typeof(NewAdventurePanel), nameof(NewAdventurePanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
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
            for (var i = ToolsContext.GamePartySize; i < Main.Settings.OverridePartySize; i++)
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

    //PATCH: auto assign heroes on new adventures (DEFAULT_PARTY)
    [HarmonyPatch(typeof(NewAdventurePanel), nameof(NewAdventurePanel.OnEndShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEndShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix(NewAdventurePanel __instance)
        {
            if (Global.IsMultiplayer || !Main.Settings.EnableTogglesToOverwriteDefaultTestParty)
            {
                return;
            }

            AssignDefaultHeroes(__instance);
        }
    }

    //PATCH: clear flag that prevents hero auto assignment under MP (DEFAULT_PARTY)
    [HarmonyPatch(typeof(NewAdventurePanel), nameof(NewAdventurePanel.SelectCampaignAndRecreateSession))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectCampaignAndRecreateSession_Patch
    {
        [UsedImplicitly]
        public static void Postfix(NewAdventurePanel __instance)
        {
            if (Global.IsMultiplayer || !Main.Settings.EnableTogglesToOverwriteDefaultTestParty)
            {
                return;
            }

            AssignDefaultHeroes(__instance);
        }
    }

    //PATCH: clear flag that prevents hero auto assignment under MP (DEFAULT_PARTY)
    [HarmonyPatch(typeof(NewAdventurePanel), nameof(NewAdventurePanel.OnEndHide))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnEndHide_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            Global.IsSettingUpMultiplayer = false;
        }
    }
}
