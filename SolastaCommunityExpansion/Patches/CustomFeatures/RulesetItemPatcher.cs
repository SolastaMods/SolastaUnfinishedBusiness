using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Classes.Monk;

namespace SolastaCommunityExpansion.Patches.CustomFeatures;

internal static class RulesetItemPatcher
{
    [HarmonyPatch(typeof(RulesetItem), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetItem_FillTags
    {
        public static void Postfix(RulesetItem __instance,
            Dictionary<string, TagsDefinitions.Criticity> tagsMap,
            object context,
            bool active = false)
        {
            if (Monk.IsMonkWeapon(__instance))
            {
                tagsMap.TryAdd(Monk.WeaponTag, TagsDefinitions.Criticity.Normal);
            }
        }
    }
}