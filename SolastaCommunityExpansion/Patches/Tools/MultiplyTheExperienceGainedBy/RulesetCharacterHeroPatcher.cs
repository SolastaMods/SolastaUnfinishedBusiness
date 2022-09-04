using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Tools.MultiplyTheExperienceGainedBy;

[HarmonyPatch(typeof(RulesetCharacterHero), "GrantExperience")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_GrantExperience
{
    internal static void Prefix(ref int experiencePoints)
    {
        if (Main.Settings.MultiplyTheExperienceGainedBy is 100 or <= 0)
        {
            return;
        }

        var original = experiencePoints;

        experiencePoints =
            (int)Math.Round(experiencePoints * Main.Settings.MultiplyTheExperienceGainedBy / 100.0f,
                MidpointRounding.AwayFromZero);

        Main.Log(
            $"GrantExperience: Multiplying experience gained by {Main.Settings.MultiplyTheExperienceGainedBy}%. Original={original}, modified={experiencePoints}.");
    }
}
