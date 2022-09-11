using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.Tools.OverrideMinMaxLevel;

//PATCH: this patch changes the min/max requirements on locations or campaigns
[HarmonyPatch(typeof(CharacterSelectionModal), "SelectFromPool")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterSelectionModal_SelectFromPool
{
    internal static void Prefix(CharacterSelectionModal __instance)
    {
        if (!Main.Settings.OverrideMinMaxLevel)
        {
            return;
        }

        __instance.MinLevel = DungeonMakerContext.DungeonMinLevel;
        __instance.MaxLevel = DungeonMakerContext.DungeonMaxLevel;
    }
}
