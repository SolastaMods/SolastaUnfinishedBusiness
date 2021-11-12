using HarmonyLib;
using System;

namespace SolastaCommunityExpansion.Patches.Cheats
{
    internal static partial class RulesetCharacterHeroPatcher_LevelUp
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
                if (Main.Settings.ExperienceModifier != 100 && Main.Settings.ExperienceModifier > 0)
                {
                    var original = experiencePoints;

                    experiencePoints = (int)Math.Round(experiencePoints * Main.Settings.ExperienceModifier / 100.0f, MidpointRounding.AwayFromZero);

                    Main.Log($"GrantExperience: Multiplying experience gained by {Main.Settings.ExperienceModifier}%. Original={original}, modified={experiencePoints}.");
                }
            }
        }

        /// <summary>
        /// This is *only* called from FunctorGrantExperience.  We adjust the amount of XP required to cancel the adjustment made in RulesetCharacterHero_GrantExperience_Patch.
        /// This results in a call from FunctorGrantExperience with GrantExperienceMode.ReachLevel working as expected.
        /// </summary>
        [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeNeededExperienceToReachLevel")]
        internal static class RulesetCharacterHero_ComputeNeededExperienceToReachLevel_Patch
        {
            internal static void Postfix(ref int __result)
            {
                if (Main.Settings.ExperienceModifier != 100 && Main.Settings.ExperienceModifier > 0)
                {
                    var original = __result;

                    __result = (int)Math.Round(__result / (Main.Settings.ExperienceModifier / 100.0f), MidpointRounding.AwayFromZero);

                    Main.Log($"ComputeNeededExperienceToReachLevel: Dividing experience gained by {Main.Settings.ExperienceModifier}%. Original={original}, modified={__result}.");
                }
            }
        }
    }
}
