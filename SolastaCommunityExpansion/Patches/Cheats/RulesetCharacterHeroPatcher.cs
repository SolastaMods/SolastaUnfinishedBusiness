using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Cheats
{
    internal static class RulesetCharacterHeroPatcher_LevelUp
    {
        // use this patch to enable the No Experience on Level up cheat
        [HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
        internal static class RulesetCharacterHero_CanLevelUp_Patch
        {
            internal static bool Prefix(RulesetCharacterHero __instance, ref bool __result)
            {
                if (Main.Settings.NoExperienceOnLevelUp)
                {
                    var levelCap = Main.Settings.EnableLevel20 ? Models.Level20Context.MOD_MAX_LEVEL : Models.Level20Context.GAME_MAX_LEVEL - 1;

                    __result = __instance.ClassesHistory.Count < levelCap;

                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(RulesetCharacterHero), "GrantExperience")]
        internal static class RulesetCharacterHero_GrantExperience_Patch
        {
            internal static void Prefix(ref int experiencePoints)
            {
                experiencePoints = (int) (experiencePoints * (Main.Settings.ExperienceModifier / 100f));
            }
        }
    }
}