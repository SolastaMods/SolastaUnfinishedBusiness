using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GuiItemDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiItemDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EnumerateTags_Patch
    {
        public static void Postfix(GuiItemDefinition __instance)
        {
            var tags = __instance.itemTags;
            var item = __instance.ItemDefinition;

            //PATCH: adds custom weapon tags (like `Polearm`) to appropriate weapons
            CustomWeaponsContext.AddCustomTags(item, tags);

            //PATCH: adds `Unfinished Business` tag to all CE items
            CeContentPackContext.AddCETag(item, tags);
        }
    }
}
