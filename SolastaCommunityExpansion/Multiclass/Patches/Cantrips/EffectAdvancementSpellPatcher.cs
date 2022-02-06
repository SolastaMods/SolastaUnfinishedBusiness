using HarmonyLib;
using static SolastaCommunityExpansion.Multiclass.Patches.Cantrips.RulesetEffectSpellPatcher;

namespace SolastaCommunityExpansion.Multiclass.Patches.Cantrips
{
    // these patches enforce cantrips to be cast at character level
    internal static class EffectAdvancementSpellPatcher
    {
        [HarmonyPatch(typeof(EffectAdvancement), "ComputeAdditionalSubtargetsByCasterLevel")]
        internal static class EffectAdvancementComputeAdditionalSubtargetsByCasterLevel
        {
            internal static void Prefix(ref int casterLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (CasterLevel > 0)
                {
                    casterLevel = CasterLevel;
                }
            }
        }

        [HarmonyPatch(typeof(EffectAdvancement), "ComputeAdditionalTargetsByCasterLevel")]
        internal static class EffectAdvancementComputeAdditionalTargetsByCasterLevel
        {
            internal static void Prefix(ref int casterLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (CasterLevel > 0)
                {
                    casterLevel = CasterLevel;
                }
            }
        }

        [HarmonyPatch(typeof(EffectAdvancement), "ComputeAdditionalDiceByCasterLevel")]
        internal static class EffectAdvancementComputeAdditionalDiceByCasterLevel
        {
            internal static void Prefix(ref int casterLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (CasterLevel > 0)
                {
                    casterLevel = CasterLevel;
                }
            }
        }

        [HarmonyPatch(typeof(EffectAdvancement), "ComputeAdditionalTargetCellsByCasterLevel")]
        internal static class EffectAdvancementComputeAdditionalTargetCellsByCasterLevel
        {
            internal static void Prefix(ref int casterLevel)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (CasterLevel > 0)
                {
                    casterLevel = CasterLevel;
                }
            }
        }
    }
}
