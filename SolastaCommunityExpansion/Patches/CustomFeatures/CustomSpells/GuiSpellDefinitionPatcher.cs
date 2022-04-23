using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
    [HarmonyPatch(typeof(GuiSpellDefinition), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GuiSpellDefinitionl_EffectDescription
    {
        internal static void Postfix(GuiSpellDefinition __instance, ref EffectDescription __result)
        {
            __result = CustomFeaturesContext.ModifySpellEffectGui(__result, __instance);
        }
    }
}
