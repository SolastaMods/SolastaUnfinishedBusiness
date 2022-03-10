using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi
{
    [HarmonyPatch(typeof(GuiSpellDefinition), "EnumerateTags")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiSpellDefinition_EnumerateTags
    {
        public static void Postfix(GuiSpellDefinition __instance)
        {
            if (SpellsContext.RegisteredSpells.TryGetValue(__instance.SpellDefinition, out var record))
            {
                TagsDefinitions.AddTagAsNeeded(__instance.TagsMap, 
                    record.IsFromOtherMod ? "OtherModContent" : "CommunityExpansion", 
                    TagsDefinitions.Criticity.Normal, 
                    true);
            }
        }
    }
}
