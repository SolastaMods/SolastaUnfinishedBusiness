using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches.Bugfix;

[HarmonyPatch(typeof(EffectDescription), "ComputeRoundsDuration")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class EffectDescription_ComputeRoundsDuration
{
    internal static bool Prefix([NotNull] EffectDescription __instance, int slotLevel, ref int __result)
    {
        //
        // BUGFIX: dominate spells
        //

        if (__instance.EffectAdvancement.AlteredDuration >= 0)
        {
            // use standard calculation
            return true;
        }

        var alteredDuration = (ExtraAdvancementDuration)__instance.EffectAdvancement.AlteredDuration;

        var result = alteredDuration switch
        {
            // TA DominateBeast and DominatePerson use AdvancementDuration.Minutes_1_10_480_1440_Infinite
            // which is only computed correctly for BestowCurse.

            ExtraAdvancementDuration.DominateBeast => slotLevel switch
            {
                <= 4 => ComputeRoundsDuration(DurationType.Minute, 1),
                5 => ComputeRoundsDuration(DurationType.Minute, 10),
                6 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.DominatePerson => slotLevel switch
            {
                <= 5 => ComputeRoundsDuration(DurationType.Minute, 1),
                6 => ComputeRoundsDuration(DurationType.Minute, 10),
                7 => ComputeRoundsDuration(DurationType.Hour, 1),
                _ => ComputeRoundsDuration(DurationType.Hour, 8)
            },
            ExtraAdvancementDuration.DominateMonster => slotLevel switch // currently a DubHerder CE specific spell
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
