using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class RulesetCharacterPatcher
    {
        // ensures that original character rage pool is in sync with substitute
        [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
        internal static class RulesetCharacterUsePower
        {
            internal static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (usablePower.PowerDefinition != PowerBarbarianRageStart)
                {
                    return;
                } 
                
                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    hero.SpendRagePoint();
                }
            }
        }

        // ensures that original character sorcery point pool is in sync with substitute 
        [HarmonyPatch(typeof(RulesetCharacter), "CreateSorceryPoints")]
        internal static class RulesetCharacterCreateSorceryPoints
        {
            internal static void Postfix(RulesetCharacter __instance, int slotLevel, RulesetSpellRepertoire repertoire)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    hero.CreateSorceryPoints(slotLevel, repertoire);
                }
            }
        }

        // ensures that original character sorcery point pool is in sync with substitute
        [HarmonyPatch(typeof(RulesetCharacter), "GainSorceryPoints")]
        internal static class RulesetCharacterGainSorceryPoints
        {
            internal static void Postfix(RulesetCharacter __instance, int sorceryPointsGain)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    hero.GainSorceryPoints(sorceryPointsGain);
                }
            }
        }
    }
}
