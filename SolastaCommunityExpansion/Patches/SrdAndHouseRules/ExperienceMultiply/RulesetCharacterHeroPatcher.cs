using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.ExperienceMultiply;

[HarmonyPatch(typeof(RulesetCharacterHero), "GrantExperience")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_GrantExperience
{
    internal static void Prefix(ref int experiencePoints)
    {
        if (Main.Settings.MultiplyTheExperienceGainedBy != 100 && Main.Settings.MultiplyTheExperienceGainedBy > 0)
        {
            var original = experiencePoints;

            experiencePoints =
                (int)Math.Round(experiencePoints * Main.Settings.MultiplyTheExperienceGainedBy / 100.0f,
                    MidpointRounding.AwayFromZero);

            Main.Log(
                $"GrantExperience: Multiplying experience gained by {Main.Settings.MultiplyTheExperienceGainedBy}%. Original={original}, modified={experiencePoints}.");
        }
    }
}

/// <summary>
///     This is *only* called from FunctorGrantExperience as of 1.1.12.
///     By default don't modify the return value from this method.  This means requests to level up will be scaled by
///     MultiplyTheExperienceGainedBy.
///     At certain quest specific points the level up must not be scaled.
/// </summary>
//[HarmonyPatch(typeof(RulesetCharacterHero), "ComputeNeededExperienceToReachLevel")]
// [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
// internal static class RulesetCharacterHero_ComputeNeededExperienceToReachLevel
// {
//     internal static void Postfix(ref int __result)
//     {
//         if (Main.Settings.MultiplyTheExperienceGainedBy != 100 && Main.Settings.MultiplyTheExperienceGainedBy > 0)
//         {
//             var gameQuestService = ServiceRepository.GetService<IGameQuestService>();
//
// #if DEBUG
//             gameQuestService?.ActiveQuests.ForEach(x => Main.Log($"Quest: {x.QuestTreeDefinition.Name}"));
// #endif
//
//             // Level up essential for Caer_Cyflen_Quest_AfterTutorial.
//             // var levelupRequired =
//             //     gameQuestService?.ActiveQuests?.Any(x =>
//             //         x.QuestTreeDefinition == Caer_Cyflen_Quest_AfterTutorial) == true;
//
//             if (levelupRequired)
//             {
//                 // Adjust the amount of XP required in order to cancel the adjustment made in RulesetCharacterHero_GrantExperience_Patch.
//                 // This results in a call from FunctorGrantExperience with GrantExperienceMode.ReachLevel working as expected and 
//                 // the relevant quest step is then not blocked.
//                 var original = __result;
//
//                 __result = (int)Math.Round(__result / (Main.Settings.MultiplyTheExperienceGainedBy / 100.0f),
//                     MidpointRounding.AwayFromZero);
//
//                 Main.Log(
//                     $"ComputeNeededExperienceToReachLevel: Dividing experience gained by {Main.Settings.MultiplyTheExperienceGainedBy}%. Original={original}, modified={__result}.");
//             }
//         }
//     }
// }
