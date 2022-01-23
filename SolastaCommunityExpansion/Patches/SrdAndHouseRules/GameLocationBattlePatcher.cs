using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules
{
    [HarmonyPatch(typeof(GameLocationBattle), "StartContenders")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattle_StartContenders
    {
        internal static bool Prefix(GameLocationBattle __instance, bool partySurprised, bool enemySurprised)
        {
            if (Main.Settings.UseOfficialCombatSurpriseRules && (partySurprised || enemySurprised))
            {
                Models.SrdAndHouseRulesContext.StartContenders(partySurprised, __instance.PlayerContenders, __instance.EnemyContenders);
                Models.SrdAndHouseRulesContext.StartContenders(enemySurprised, __instance.EnemyContenders, __instance.PlayerContenders);

                return false;
            }

            return true;
        }
    }
}
