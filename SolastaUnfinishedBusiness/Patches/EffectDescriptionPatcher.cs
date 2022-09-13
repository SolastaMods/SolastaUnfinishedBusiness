using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

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

    [HarmonyPatch(typeof(EffectDescription), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillTags_Patch
    {
        internal static void Postfix(EffectDescription __instance,
            Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            // PATCH: fill tags for CustomEffectForm
            foreach (var customEffect in __instance.EffectForms.OfType<CustomEffectForm>())
            {
                customEffect.FillTags(tagsMap);
            }
        }
    }
}
