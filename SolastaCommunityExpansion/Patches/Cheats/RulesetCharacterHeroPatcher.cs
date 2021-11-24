using HarmonyLib;
using System;
using System.Linq;
using static SolastaModApi.DatabaseHelper.QuestTreeDefinitions;

namespace SolastaCommunityExpansion.Patches
{
    // use this patch to enable the No Experience on Level up cheat
    [HarmonyPatch(typeof(RulesetCharacterHero), "CanLevelUp", MethodType.Getter)]
    internal static class RulesetCharacterHero_CanLevelUp
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
    internal static class RulesetCharacterHero_GrantExperience
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
    /// This is *only* called from FunctorGrantExperience as of 1.1.12. 
    /// By default don't modify the return value from this method.  This means requests to level up will be scaled by ExperienceModifier.
    /// At certain quest specific points the level up must not be scaled.
    /// </summary>
    [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeNeededExperienceToReachLevel")]
    internal static class RulesetCharacterHero_ComputeNeededExperienceToReachLevel
    {
        internal static void Postfix(ref int __result)
        {
            if (Main.Settings.ExperienceModifier != 100 && Main.Settings.ExperienceModifier > 0)
            {
                var gameQuestService = ServiceRepository.GetService<IGameQuestService>();

#if DEBUG
                gameQuestService?.ActiveQuests.ForEach(x => Main.Log($"Quest: {x.QuestTreeDefinition.Name}"));
#endif

                // Level up essential for Caer_Cyflen_Quest_AfterTutorial.
                bool levelupRequired = gameQuestService?.ActiveQuests?.Any(x => x.QuestTreeDefinition == Caer_Cyflen_Quest_AfterTutorial) == true;

                if (levelupRequired)
                {
                    // Adjust the amount of XP required in order to cancel the adjustment made in RulesetCharacterHero_GrantExperience_Patch.
                    // This results in a call from FunctorGrantExperience with GrantExperienceMode.ReachLevel working as expected and 
                    // the relevant quest step is then not blocked.
                    var original = __result;

                    __result = (int)Math.Round(__result / (Main.Settings.ExperienceModifier / 100.0f), MidpointRounding.AwayFromZero);

                    Main.Log($"ComputeNeededExperienceToReachLevel: Dividing experience gained by {Main.Settings.ExperienceModifier}%. Original={original}, modified={__result}.");
                }
            }
        }
    }
}
