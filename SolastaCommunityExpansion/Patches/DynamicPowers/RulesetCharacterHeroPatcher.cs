using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.ConditionalPowers
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
    internal static class RulesetCharacterHero_RefreshAll_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            // Anything that grants powers dynamically will stop working if this is turned off.
            // I'm making it a setting to allow it to be disabled if that becomes necesary, but 
            // this shouldn't get exposed in the UI.
            if (Main.Settings.AllowDynamicPowers)
            {
                // Grant powers when we do a refresh all. This allows powers from things like fighting styles and conditions.
                __instance.GrantPowers();
                __instance.RefreshPowers();
            }
        }
    }
}
