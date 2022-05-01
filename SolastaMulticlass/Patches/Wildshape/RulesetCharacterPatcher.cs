using System.Collections.Generic;
using HarmonyLib;
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
                if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
                {
                    __result = false;
                }
            }
        }

        // ensures that the wildshape hero has access to spell repertoires for calculating slot related features
        [HarmonyPatch(typeof(RulesetCharacter), "SpellRepertoires", MethodType.Getter)]
        internal static class RulesetCharacterSpellRepertoires
        {
            internal static void Postfix(RulesetCharacter __instance, ref List<RulesetSpellRepertoire> __result)
            {
                if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
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
                if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
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
                if (__instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
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
                if (usablePower.PowerDefinition == PowerBarbarianRageStart
                    && __instance.OriginalFormCharacter is RulesetCharacterHero hero && hero != __instance)
                {
                    hero.SpendRagePoint();
                }
            }
        }
    }
}
