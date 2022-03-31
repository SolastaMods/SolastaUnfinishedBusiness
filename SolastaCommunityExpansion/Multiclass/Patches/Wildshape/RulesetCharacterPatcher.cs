using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaMulticlass.Patches.Wildshape
{
    internal static class RulesetCharacterPatcher
    {
        // ensures that the wildshape hero cannot cast any spells 
        [HarmonyPatch(typeof(RulesetCharacter), "CanCastSpells")]
        internal static class RulesetCharacterCanCastSpells
        {
            internal static void Postfix(RulesetCharacter __instance, ref bool __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    __result = false;
                }
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "FindClassHoldingFeature")]
        internal static class RulesetCharacterFindClassHoldingFeature
        {
            internal static void Postfix(RulesetCharacter __instance, ref CharacterClassDefinition __result, FeatureDefinition featureDefinition)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    __result = hero.FindClassHoldingFeature(featureDefinition);
                }
            }
        }

        // ensures that the wildshape hero has access to spell repertoires for calculating slot related features
        [HarmonyPatch(typeof(RulesetCharacter), "SpellRepertoires", MethodType.Getter)]
        internal static class RulesetCharacterSpellRepertoires
        {
            internal static void Postfix(RulesetCharacter __instance, ref List<RulesetSpellRepertoire> __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is RulesetCharacterHero hero && hero != __instance)
                {
                    __result = hero.SpellRepertoires;
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
    }
}
