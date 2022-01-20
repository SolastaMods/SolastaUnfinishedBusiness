using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(EffectDescription), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EffectDescription_FillTags
    {
        public static void Postfix(EffectDescription __instance, Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            foreach (CustomFeatureDefinitions.CustomEffectForm customEffect in __instance.EffectForms.OfType<CustomFeatureDefinitions.CustomEffectForm>())
            {
                customEffect.FillTags(tagsMap);
            }
        }
    }
}
