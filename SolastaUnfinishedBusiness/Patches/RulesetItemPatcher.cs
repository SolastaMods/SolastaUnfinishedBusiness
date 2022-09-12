using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetItemPatcher
{
    [HarmonyPatch(typeof(RulesetItem), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillTags_Patch
    {
        public static void Postfix(
            RulesetItem __instance,
            Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            var item = __instance.itemDefinition;

            //PATCH: adds custom weapon tags (like `Polearm`) to appropriate weapons
            CustomWeaponsContext.AddCustomTags(item, tagsMap);

            //PATCH: adds `Unfinished Business` tag to all CE items 
            CeContentPackContext.AddCeTag(item, tagsMap);
        }
    }
}
