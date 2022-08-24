using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;

namespace SolastaCommunityExpansion.Patches;

internal static class EffectDescriptionPatcher
{
    [HarmonyPatch(typeof(EffectDescription), "ComputeRoundsDuration")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeRoundsDuration_Patch
    {
        internal static bool Prefix([NotNull] EffectDescription __instance, int slotLevel, ref int __result)
        {
            //PATCH: implements computation of extra effect duration advancement types
            return EnumImplementation.ComputeExtraAdvancementDuration(__instance, slotLevel, ref __result);
        }
    }
}