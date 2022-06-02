using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.VariableReplacement;

[HarmonyPatch(typeof(GuiQuest), "GetStepTitle")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiQuest_GetStepTitle
{
    internal static void Postfix(ref string __result)
    {
        __result = DungeonMakerContext.ReplaceVariable(__result);
    }
}

[HarmonyPatch(typeof(GuiQuest), "GetStepDescription")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiQuest_GetStepDescription
{
    internal static void Postfix(ref string __result)
    {
        __result = DungeonMakerContext.ReplaceVariable(__result);
    }
}
