using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetItemPatcher
{
    [HarmonyPatch(typeof(RulesetItem), "FillTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class FillTags_Patch
    {
        public static void Postfix(
            RulesetItem __instance,
            Dictionary<string, TagsDefinitions.Criticity> tagsMap)
        {
            var item = __instance.itemDefinition;

            //PATCH: adds custom weapon tags (like `Polearm`) to appropriate weapons
            CustomWeaponsContext.AddCustomTags(item, tagsMap);

            //PATCH: adds custom `Returning` tag to appropriate weapons
            ReturningWeapon.AddCustomTags(__instance, tagsMap);
            
            //PATCH: removes `Loading` and `Ammunition` tags to appropriate weapons
            RepeatingShot.ModifyTags(__instance, tagsMap);

            //PATCH: adds `Unfinished Business` tag to all CE items 
            CeContentPackContext.AddCeTag(item, tagsMap);
        }
    }
}
