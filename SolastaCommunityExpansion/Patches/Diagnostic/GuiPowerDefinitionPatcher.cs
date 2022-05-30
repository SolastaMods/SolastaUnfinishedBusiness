using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Diagnostic;

[HarmonyPatch(typeof(GuiPowerDefinition), "EnumerateTags")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GuiPowerDefinition_EnumerateTags
{
    public static void Postfix(GuiPowerDefinition __instance)
    {
        if (DiagnosticsContext.IsCeDefinition(__instance.BaseDefinition))
        {
            TagsDefinitions.AddTagAsNeeded(__instance.TagsMap,
                "CommunityExpansion", TagsDefinitions.Criticity.Normal);
        }
    }
}
