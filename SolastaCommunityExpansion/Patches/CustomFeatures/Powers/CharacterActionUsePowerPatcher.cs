using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers;

// ignores interruptions processing for certain powers
[HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionBefore")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterActionUsePower_CheckInterruptionBefore
{
    internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
    {
        return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
    }
}

// ignores interruptions processing for certain powers
[HarmonyPatch(typeof(CharacterActionUsePower), "CheckInterruptionAfter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterActionUsePower_CheckInterruptionAfter
{
    internal static bool Prefix([NotNull] CharacterActionUsePower __instance)
    {
        return !PowersContext.PowersThatIgnoreInterruptions.Contains(__instance.activePower.PowerDefinition);
    }
}
