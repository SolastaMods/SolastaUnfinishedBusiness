

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.CustomEffectForm
{
    [HarmonyPatch(typeof(EffectDescription), "FillTags")]
    internal static class EffectDescriptionFillTags
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
