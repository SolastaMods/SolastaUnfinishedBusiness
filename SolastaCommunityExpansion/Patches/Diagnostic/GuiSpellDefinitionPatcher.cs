using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Diagnostic
{
    [HarmonyPatch(typeof(GuiSpellDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiSpellDefinition_EnumerateTags
    {
        public static void Postfix(GuiSpellDefinition __instance)
        {
            if (SpellsContext.Spells.TryGetValue(__instance.SpellDefinition, out var record))
            {
                TagsDefinitions.AddTagAsNeeded(__instance.TagsMap,
                    "CommunityExpansion", TagsDefinitions.Criticity.Normal, true);
            }
            else if (DiagnosticsContext.IsCeDefinition(__instance.BaseDefinition))
            {
                // Not all CE spells are registered in SpellsContext
                TagsDefinitions.AddTagAsNeeded(__instance.TagsMap,
                    "CommunityExpansion", TagsDefinitions.Criticity.Normal, true);
            }
        }
    }
}
