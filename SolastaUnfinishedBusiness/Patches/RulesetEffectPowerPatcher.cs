using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetEffectPowerPatcher
{
    [HarmonyPatch(typeof(RulesetEffectPower), "SaveDC", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveDC_Getter_Patch
    {
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

            var classHolder = originItem.ItemDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>();

            if (classHolder != null)
            {
                classDefinition = classHolder.Class;
            }

            var usablePower = __instance.UsablePower;

            UsablePowersProvider.UpdateSaveDc(user, usablePower, classDefinition);
            __result = usablePower.SaveDC;
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), "GetClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class GetClassLevel_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref int __result, RulesetCharacter character)
        {
            if (character is not RulesetCharacterHero hero)
            {
                return;
            }

            int level;

            //PATCH: support for multiclass to correctly get the class level (MULTICLASS)
            // for some unknown reason when MC we're getting class level 0 here [i.e.: second wind]
            if (__result == 0)
            {
                hero.LookForFeatureOrigin(__instance.UsablePower.PowerDefinition, out var _, out var klass, out var _);

                if (hero.ClassesAndLevels.TryGetValue(klass, out level))
                {
                    __result = level;
                }
            }

            //PATCH: support for `IClassHoldingFeature`
            var holder = __instance.PowerDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>();

            if (holder == null)
            {
                return;
            }

            if (hero.ClassesAndLevels.TryGetValue(holder.Class, out level))
            {
                __result = level;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), "EffectDescription", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EffectDescription_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref EffectDescription __result)
        {
            //PATCH: support for `ICustomMagicEffectBasedOnCaster` and `IModifySpellEffect` 
            // allowing to pick and/or tweak power effect depending on some properties of the user
            __result = PowerBundle.ModifyPowerEffect(__result, __instance);
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), "MagicAttackBonus", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class MagicAttackBonus_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref int __result)
        {
            //PATCH: allow devices have magic attack bonus based on spell attack
            var power = __instance.PowerDefinition;

            if (power.AttackHitComputation !=
                (RuleDefinitions.PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
            {
                return;
            }

            var user = __instance.User;
            var repertoire = user.GetClassSpellRepertoire(user.FindClassHoldingFeature(power));

            if (repertoire != null)
            {
                __result = repertoire.SpellAttackBonus;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetEffectPower), "MagicAttackTrends", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class MagicAttackTrends_Getter_Patch
    {
        public static void Postfix(RulesetEffectPower __instance, ref List<RuleDefinitions.TrendInfo> __result)
        {
            //PATCH: allow devices have magic attack trends based on spell attack
            var power = __instance.PowerDefinition;

            if (power.AttackHitComputation !=
                (RuleDefinitions.PowerAttackHitComputation)ExtraPowerAttackHitComputation.SpellAttack)
            {
                return;
            }

            var user = __instance.User;
            var repertoire = user.GetClassSpellRepertoire(user.FindClassHoldingFeature(power));

            if (repertoire != null)
            {
                __result = repertoire.MagicAttackTrends;
            }
        }
    }
}
