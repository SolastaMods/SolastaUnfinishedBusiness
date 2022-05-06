using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // handles all different scenarios to determine max spell level (must be a postfix)
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetSpellRepertoire_MaxSpellLevelOfSpellCastingLevel_Getter
    {
        internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
        {
            if (Main.Settings.EnableLevel20 && __instance.SpellCastingFeature != null && __instance.SpellCastingLevel > 0)
            {
                var slotsPerLevel = __instance.SpellCastingFeature.SlotsPerLevels[__instance.SpellCastingLevel - 1];

                __result = slotsPerLevel.Slots.IndexOf(0);
            }
        }
    }
}
