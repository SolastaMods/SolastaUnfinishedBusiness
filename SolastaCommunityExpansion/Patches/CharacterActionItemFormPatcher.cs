using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class CharacterActionItemFormPatcher
{
    [HarmonyPatch(typeof(CharacterActionItemForm), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Refresh_Patch
    {
        internal static void Prefix(CharacterActionItemForm __instance)
        {
            //PATCH: supports extra attacks on action panel
            //Applies skipping of attack modes when item form refresh starts, so UI would display proper attack mode data
            ExtraAttacksOnActionPanel.StartSkipping(__instance);
        }

        internal static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: supports extra attacks on action panel
            //Stops skipping of attack modes when item form refresh ends, so UI would display proper attack mode data
            ExtraAttacksOnActionPanel.StopSkipping(__instance);
        }
    }
}
