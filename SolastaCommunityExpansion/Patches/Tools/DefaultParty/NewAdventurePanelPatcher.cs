using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty;

[HarmonyPatch(typeof(NewAdventurePanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class NewAdventurePanel_Refresh
{
    internal static bool ShouldAssignDefaultParty { get; set; }

    internal static void Postfix(NewAdventurePanel __instance)
    {
        if (!Main.Settings.EnableTogglesToOverwriteDefaultTestParty
            || !ShouldAssignDefaultParty
            || Global.IsMultiplayer)
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

            var heroname = Main.Settings.defaultPartyHeroes[i];

            __instance.AutotestSelectCharacter(i, heroname);
        }

        ShouldAssignDefaultParty = false;
    }
}
