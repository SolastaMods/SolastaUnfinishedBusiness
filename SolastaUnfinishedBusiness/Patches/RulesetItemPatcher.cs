using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetItemPatcher
{
    [HarmonyPatch(typeof(RulesetItem), nameof(RulesetItem.FillTags))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FillTags_Patch
    {
        [UsedImplicitly]
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
