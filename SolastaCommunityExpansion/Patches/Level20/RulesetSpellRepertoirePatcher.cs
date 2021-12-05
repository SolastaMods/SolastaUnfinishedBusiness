using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(RulesetSpellRepertoire), "MaxSpellLevelOfSpellCastingLevel", MethodType.Getter)]
    internal static class RulesetSpellRepertoire_MaxSpellLevelOfSpellCastingLevel_Getter
    {
        internal static void Postfix(RulesetSpellRepertoire __instance, ref int __result)
        {
            if (Main.Settings.EnableLevel20 && __instance.SpellCastingFeature != null)
            {
                FeatureDefinitionCastSpell.SlotsByLevelDuplet slotsPerLevel = __instance.SpellCastingFeature?.SlotsPerLevels[__instance.SpellCastingLevel - 1];

                __result = slotsPerLevel.Slots.IndexOf(0);
            }
        }
    }
}
