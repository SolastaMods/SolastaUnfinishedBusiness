using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetEffectSpellPatcher
{
    //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
    [HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EffectDescription_Getter_Patch
    {
        public static void Postfix(RulesetEffectSpell __instance, ref EffectDescription __result)
        {
            // allowing to pick and/or tweak spell effect depending on some caster properties
            __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectSpell), "SaveDC", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveDC_Getter_Patch
    {
        public static void Postfix(RulesetEffectSpell __instance, ref int __result)
        {
            //PATCH: allow devices have DC based on user or item summoner stats, instead of static value
            var originItem = __instance.OriginItem;

            if (originItem == null || __result > -1)
            {
                return;
            }

            var caster = __instance.Caster;
            string className = null;

            if (__result == -2)
            {
                caster = EffectHelpers.GetCharacterByEffectGuid(originItem.SourceSummoningEffectGuid) ?? caster;
            }

            var classHolder = originItem.ItemDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>();
            if (classHolder != null)
            {
                className = classHolder.Class.Name;
            }

            __result = EffectHelpers.CalculateSaveDc(caster, __instance.spellDefinition.effectDescription, className);
        }
    }

    //PATCH: Multiclass: enforces cantrips to be cast at character level 
    [HarmonyPatch(typeof(RulesetEffectSpell), "GetClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetClassLevel_Patch
    {
        public static void Postfix(RulesetEffectSpell __instance, ref int __result, RulesetCharacter character)
        {
            if (character is RulesetCharacterHero hero && __instance.SpellDefinition.SpellLevel == 0)
            {
                __result = hero.GetAttribute(AttributeDefinitions.CharacterLevel).CurrentValue;
            }
        }
    }

    //PATCH: enforces cantrips to be cast at character level
    [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ComputeTargetParameter_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //replaces repertoire's SpellCastingLevel with character level for cantrips
            var spellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_SpellCastingLevel");
            var spellCastingLevel =
                new Func<RulesetSpellRepertoire, RulesetEffectSpell, int>(MulticlassContext.SpellCastingLevel).Method;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(spellCastingLevelMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                    yield return new CodeInstruction(OpCodes.Call, spellCastingLevel);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
