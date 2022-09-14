using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Subclasses.Wizard;

namespace SolastaUnfinishedBusiness.Patches;

internal static class RulesetCharacterMonsterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FinalizeMonster_Patch
    {
        internal static void Postfix(RulesetCharacterMonster __instance)
        {
            //TODO: Consider using `FeatureDefinitionSummoningAffinity` for this

            //PATCH: allows us to change monsters created by Dead Master
            WizardDeadMaster.OnMonsterCreated(__instance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "RegisterAttributes")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RegisterAttributes_Patch
    {
        internal static void Postfix(RulesetCharacterMonster __instance)
        {
            //PATCH: support for rage/ki/other stuff while shape-shifted
            if (__instance.originalFormCharacter is not RulesetCharacterHero hero)
            {
                return;
            }

            // sync rage points (ruleset keeps rage data in other places so we need to call this method to sync)
            var rageCount = hero.UsedRagePoints;

            while (rageCount-- > 0)
            {
                __instance.SpendRagePoint();
            }

            // sync ki points (ruleset keeps ki data in other places so we need to call this method to sync)
            __instance.ForceKiPointConsumption(hero.UsedKiPoints);

            // copy modifiers from original hero
            hero.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(__instance.FeaturesToBrowse);

            foreach (var feature in __instance.FeaturesToBrowse)
            {
                if (feature is not FeatureDefinitionAttributeModifier mod ||
                    !__instance.TryGetAttribute(mod.ModifiedAttribute, out _))
                {
                    continue;
                }

                mod.ApplyModifiers(__instance.Attributes, AttributeDefinitions.TagConjure);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), "RefreshAll")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAll_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for rage/ki/other stuff while shape-shifted

            // refresh values of attribute modifiers before refreshing attributes
            var refreshAttributes = typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var refreshAttributeModifiers = typeof(RulesetActor).GetMethod("RefreshAttributeModifierFromAbilityScore");

            foreach (var code in instructions)
            {
                if (code.Calls(refreshAttributes))
                {
                    yield return new CodeInstruction(OpCodes.Call, refreshAttributeModifiers);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                }

                yield return code;
            }
        }
    }
}
