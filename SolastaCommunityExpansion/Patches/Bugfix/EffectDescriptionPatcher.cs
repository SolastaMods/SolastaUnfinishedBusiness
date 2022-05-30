using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.Bugfix;

[HarmonyPatch(typeof(EffectDescription), "ComputeRoundsDuration")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class EffectDescription_ComputeRoundsDuration
{
    public static bool Prefix(EffectDescription __instance, int slotLevel, ref int __result)
    {
        if (!Main.Settings.BugFixDominateSpells)
        {
            return true;
        }

        if (__instance.EffectAdvancement.AlteredDuration >= 0)
        {
            // use standard calculation
            return true;
        }

        var alteredDuration = (AdvancementDurationEx)__instance.EffectAdvancement.AlteredDuration;

        var result = alteredDuration switch
        {
            // TA DominateBeast and DominatePerson use AdvancementDuration.Minutes_1_10_480_1440_Infinite
            // which is only computed correctly for BestowCurse.

            AdvancementDurationEx.DominateBeast => slotLevel switch
            {
                <= 4 => ComputeRoundsDuration(DurationType.Minute, 1),
                5 => ComputeRoundsDuration(DurationType.Minute, 10),
                6 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            AdvancementDurationEx.DominatePerson => slotLevel switch
            {
                <= 5 => ComputeRoundsDuration(DurationType.Minute, 1),
                6 => ComputeRoundsDuration(DurationType.Minute, 10),
                7 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            AdvancementDurationEx.DominateMonster => slotLevel switch // currently a DubHerder CE specific spell
            {
                <= 8 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            _ => -1
        };

        if (result == -1)
        {
            return true;
        }

        __result = result;

        return false;
    }
}

internal enum AdvancementDurationEx
{
    DominateBeast = -1,
    DominatePerson = -2,
    DominateMonster = -3
}
