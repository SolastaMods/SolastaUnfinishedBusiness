using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

// add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect depending on some caster properties
// and IModifySpellEffect which modifies existing effect (changing elemental damage type for example)
[HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetEffectSpell_EffectDescription
{
    internal static void Postfix(RulesetEffectSpell __instance, ref EffectDescription __result)
    {
        if (!Main.Settings.EnableCustomSpellsPatch)
        {
            return;
        }

        __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
    }
}
