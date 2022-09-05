using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetItemPatcher
{
    [HarmonyPatch(typeof(RulesetItem), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetItem_FillTags
    {
        public static void Postfix(
            RulesetItem __instance,
            Dictionary<string, TagsDefinitions.Criticity> tagsMap,
            object context,
            bool active = false)
        {
            var item = __instance.itemDefinition;

            //PATCH: adds custom weapon tags (like `Polearm`) to appropriate weapons
            CustomWeaponsContext.AddCustomTags(item, tagsMap);

            //PATCH: adds `Community Expansion` tag to all CE items 
            CeContentPackContext.AddCETag(item, tagsMap);
        }
    }
}
