using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty;

//PATCH: EnableTogglesToOverwriteDefaultTestParty
[HarmonyPatch(typeof(NewAdventurePanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NewAdventurePanel_Refresh
{
    internal static bool ShouldAssignDefaultParty { get; set; }

    // ReSharper disable once UnusedMember.Global
    internal static void Postfix(NewAdventurePanel __instance)
    {
        if (Global.IsMultiplayer
            || !Main.Settings.EnableTogglesToOverwriteDefaultTestParty
            || !ShouldAssignDefaultParty)
        {
            return;
        }

        var max = Math.Min(Main.Settings.defaultPartyHeroes.Count,
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

            var heroName = Main.Settings.defaultPartyHeroes[i];

            __instance.AutotestSelectCharacter(i, heroName);
        }

        ShouldAssignDefaultParty = false;
    }
}

//PATCH: EnableTogglesToOverwriteDefaultTestParty
[HarmonyPatch(typeof(NewAdventurePanel), "OnEndHide")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NewAdventurePanel_OnEndHide
{
    // ReSharper disable once UnusedMember.Global
    internal static void Prefix()
    {
        Global.IsSettingUpMultiplayer = false;
    }
}
