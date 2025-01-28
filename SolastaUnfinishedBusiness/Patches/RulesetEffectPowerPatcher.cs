using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetEffectPowerPatcher
{
    [HarmonyPatch(typeof(RulesetEffectPower), ".ctor", MethodType.Constructor)]
    [HarmonyPatch([
        typeof(RulesetCharacter), // user
        typeof(RulesetUsablePower), // usablePower
        typeof(RulesetItemDevice), // originItem = null
        typeof(RulesetDeviceFunction) // usableDeviceFunction = null
    ])]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static class RulesetEffectPower_Ctor_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var method = typeof(EffectDescription).GetMethod(nameof(EffectDescription.ComputeRoundsDuration));
            var custom = new Func<EffectDescription, int, int, RulesetCharacter, RulesetUsablePower, int>(Custom)
                .Method;
            //BUGFIX: replace classLevel calculations for HalfClassLevelHours durations - native one assumes power comes
            //from class that already has subclass picked, so it breaks if such power is granted on a class before subclass pick
            return instructions.ReplaceCalls(method,
                "RulesetEffectPower.ctor",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, custom));
        }

        private static int Custom(EffectDescription effect, int slotLevel, int classLevel, RulesetCharacter user,
            RulesetUsablePower usablePower)
        {
            var power = usablePower.PowerDefinition;

            if (classLevel > 1 || power.EffectDescription.DurationType != DurationType.HalfClassLevelHours)
            {
                return effect.ComputeRoundsDuration(slotLevel, classLevel);
            }

            var holdingClass = user.FindClassHoldingFeature(power);
            if (holdingClass != null)
            {
                classLevel = user.GetClassLevel(holdingClass);
            }

            return effect.ComputeRoundsDuration(slotLevel, classLevel);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), nameof(RulesetEffectPower.SaveDC), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    public static class SaveDC_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            //PATCH: allow devices have DC based on user or item summoner stats, instead of static value
            var originItem = __instance.OriginItem;

            if (originItem == null || originItem.UsableDeviceDescription.SaveDC >= 0)
            {
                return;
            }

            var user = __instance.User;
            CharacterClassDefinition classDefinition = null;

            if (originItem.UsableDeviceDescription.SaveDC == EffectHelpers.BasedOnItemSummoner)
            {
                user = EffectHelpers.GetCharacterByEffectGuid(originItem.SourceSummoningEffectGuid) ?? user;
            }

            var classHolder = originItem.ItemDefinition.GetFirstSubFeatureOfType<ClassHolder>();

            if (classHolder != null)
            {
                classDefinition = classHolder.Class;
            }

            var usablePower = __instance.UsablePower;

            PowerProvider.UpdateSaveDc(user, usablePower, classDefinition);
            __result = usablePower.SaveDC;
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), nameof(RulesetEffectPower.GetClassLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetClassLevel_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetEffectPower __instance, ref int __result, RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return;
            }

            //PATCH: support for `IClassHoldingFeature`
            var holder = __instance.PowerDefinition.GetFirstSubFeatureOfType<ClassHolder>();

            if (holder == null)
            {
                return;
            }

            __result = hero.GetClassLevel(holder.Class);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), nameof(RulesetEffectPower.EffectDescription), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EffectDescription_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetEffectPower __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // allowing to pick and/or tweak power effect depending on some properties of the user
            __result = PowerBundle.ModifyPowerEffect(__result, __instance);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), nameof(RulesetEffectPower.MagicAttackBonus), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MagicAttackBonus_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            //PATCH: allow devices have magic attack bonus based on spell attack
            var power = __instance.PowerDefinition;

            if (power.AttackHitComputation !=
                (PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
            {
                return;
            }

            var user = __instance.User;

            // this is required by Artillerist which has powers tied to caster
            var summoner = user.GetMySummoner();

            if (summoner != null)
            {
                user = summoner.RulesetCharacter;
            }

            var repertoire = user.GetClassSpellRepertoire(user.FindClassHoldingFeature(power));

            if (repertoire != null)
            {
                __result = repertoire.SpellAttackBonus;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), nameof(RulesetEffectPower.MagicAttackTrends), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class MagicAttackTrends_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetEffectPower __instance, ref List<TrendInfo> __result)
        {
            //PATCH: allow devices have magic attack trends based on spell attack
            var power = __instance.PowerDefinition;

            if (power.AttackHitComputation !=
                (PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
            {
                return;
            }

            var user = __instance.User;

            // this is required by Artillerist which has powers tied to caster
            var summoner = user.GetMySummoner();

            if (summoner != null)
            {
                user = summoner.RulesetCharacter;
            }

            var repertoire = user.GetClassSpellRepertoire(user.FindClassHoldingFeature(power));

            if (repertoire != null)
            {
                __result = repertoire.MagicAttackTrends;
            }
        }
    }
}
