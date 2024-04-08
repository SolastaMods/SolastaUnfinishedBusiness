using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetEffectPowerPatcher
{
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

            var classHolder = originItem.ItemDefinition.GetFirstSubFeatureOfType<IModifyAdditionalDamageClassLevel>();

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
            var holder = __instance.PowerDefinition.GetFirstSubFeatureOfType<IModifyAdditionalDamageClassLevel>();

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
