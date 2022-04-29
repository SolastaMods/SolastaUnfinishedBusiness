using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    // For some reason only Die value determination increments feature uses,
    // this fix increments for all other types otherwise additional damage features that use other types
    // (like Elemental Forms of Elementalist Warlock use PB as dmage bonus) will trigger on each hit,
    // regardless of usage limit setting
    [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
    internal static class GameLocationBattleManager_ComputeAndNotifyAdditionalDamage
    {
        internal static void Postfix(
            GameLocationCharacter attacker,
            IAdditionalDamageProvider provider)
        {
            if (!Main.Settings.BugFixCorrectlyCalculateDamageOnMultipleHits)
            {
                return;
            }

            if (provider.DamageValueDetermination != RuleDefinitions.AdditionalDamageValueDetermination.Die)
            {
                if (attacker.UsedSpecialFeatures.ContainsKey(provider.Name))
                    attacker.UsedSpecialFeatures[provider.Name]++;
                else
                    attacker.UsedSpecialFeatures[provider.Name] = 1;
            }
        }
    }
}
