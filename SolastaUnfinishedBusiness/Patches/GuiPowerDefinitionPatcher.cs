using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GuiPowerDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiPowerDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EnumerateTags_Patch
    {
        public static void Postfix(GuiPowerDefinition __instance)
        {
            //PATCH: adds `Unfinished Business` tag to all CE powers
            CeContentPackContext.AddCeTag(__instance.BaseDefinition, __instance.TagsMap);
        }
    }
}
