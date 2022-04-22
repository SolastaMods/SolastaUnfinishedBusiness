using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Spells
{
    public class CustomSpellPatches
    {
        //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect depending on some caster properties
        //and IModifySpellEffect which modifies existing effect (changing elemental damage type for example)
        [HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
        class RulesetEffectSpell_EffectDescription
        {
            static void Postfix(ref EffectDescription __result, RulesetEffectSpell __instance)
            {
                __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
            }
        }

        //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
        [HarmonyPatch(typeof(GuiSpellDefinition), "EffectDescription", MethodType.Getter)]
        class GuiSpellDefinitionl_EffectDescription
        {
            static void Postfix(ref EffectDescription __result, GuiSpellDefinition __instance)
            {
                __result = CustomFeaturesContext.ModifySpellEffectGui(__result, __instance);
            }
        }
    }
}
