using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Patches.Multiclass.Wildshape
{
    // ensures that the wildshape hero cannot cast any spells
    [HarmonyPatch(typeof(RulesetCharacter), "CanCastSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_CanCastSpells
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_SpellRepertoires
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_CreateSorceryPoints
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_GainSorceryPoints
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
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_UsePower
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
