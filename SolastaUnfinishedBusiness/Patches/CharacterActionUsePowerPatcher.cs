using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterActionUsePowerPatcher
{
    [HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionBefore")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CheckInterruptionBefore_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionAfter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CheckInterruptionAfter_Patch
    {
        internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
        {
            //PATCH: ignores interruptions processing for certain powers so they won't interrupt invisibility
            return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
        }
    }
}
