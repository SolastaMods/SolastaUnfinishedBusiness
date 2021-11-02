using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantItem")]
    internal static class RulesetCharacterHero_GrantItem
    {
        public static void Prefix(ref bool tryToEquip)
        {
            if (Main.Settings.DisableAutoEquip)
            {
                tryToEquip = false;
            }
        }
    }
}