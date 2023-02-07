using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class EffectDescriptionPatcher
{
    [HarmonyPatch(typeof(EffectDescription), nameof(EffectDescription.ComputeRoundsDuration))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeRoundsDuration_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] EffectDescription __instance, int slotLevel, ref int __result)
        {
            //PATCH: implements computation of extra effect duration advancement types
            return EnumImplementation.ComputeExtraAdvancementDuration(__instance, slotLevel, ref __result, ref __instance.durationType);
        }
    }
#if false
    [HarmonyPatch(typeof(EffectDescription), nameof(EffectDescription.FillTags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly] public static class FillTags_Patch
    {
        [UsedImplicitly] public static void Postfix(EffectDescription __instance, Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            // PATCH: fill tags for CustomEffectForm
            foreach (var customEffect in __instance.EffectForms.OfType<CustomEffectForm>())
            {
                customEffect.FillTags(tagsMap);
            }
        }
    }
#endif
}
