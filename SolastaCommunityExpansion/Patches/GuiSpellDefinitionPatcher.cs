using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class GuiSpellDefinitionPatcher
{
    [HarmonyPatch(typeof(GuiSpellDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EnumerateTags_Patch
    {
        public static void Postfix(GuiSpellDefinition __instance)
        {
            //PATCH: adds `Community Expansion` tag to all CE spells
            CeContentPackContext.AddCESpellTag(__instance.SpellDefinition, __instance.TagsMap);
        }
    }
}
