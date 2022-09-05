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

    [HarmonyPatch(typeof(GuiSpellDefinition), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EffectDescription_Patch
    {
        internal static void Postfix(GuiSpellDefinition __instance, ref EffectDescription __result)
        {
            //PATCH: support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
            __result = CustomFeaturesContext.ModifySpellEffectGui(__result, __instance);
        }
    }
}
