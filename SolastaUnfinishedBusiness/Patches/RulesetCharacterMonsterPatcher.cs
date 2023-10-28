using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetCharacterMonsterPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.FinalizeMonster))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FinalizeMonster_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterMonster __instance, bool keepMentalAbilityScores)
        {
            //PATCH: Fixes AC calculation for MC shape-shifters and support for rage/ki/other stuff while shape-shifted
            MulticlassWildshapeContext.FinalizeMonster(__instance, keepMentalAbilityScores);

            //PATCH: supports Awaken the Beast Within feat
            ClassFeats.ActionFinishedByMeFeatAwakenTheBeastWithin.GrantTempHP(__instance);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.RefreshAll))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAll_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for rage/ki/other stuff while shape-shifted

            // refresh values of attribute modifiers before refreshing attributes
            var refreshAttributes =
                typeof(RulesetEntity).GetMethod("RefreshAttributes");
            var refreshAttributeModifiers =
                typeof(RulesetCharacter).GetMethod("RefreshAttributeModifierFromAbilityScore");

            return instructions.ReplaceCalls(refreshAttributes, "RulesetCharacterMonster.RefreshAll",
                new CodeInstruction(OpCodes.Call, refreshAttributeModifiers),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, refreshAttributes)); // checked for Call vs CallVirtual
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.RefreshArmorClass))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshArmorClass_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: implements exclusivity for some AC modifiers
            // Makes sure various unarmored defense features don't stack with themselves and Dragon Resilience
            // Replaces calls to `RulesetAttributeModifier.SortAttributeModifiersList` with custom method
            // that removes inactive exclusive modifiers, and then calls `RulesetAttributeModifier.SortAttributeModifiersList`
            var sort = new Action<
                List<RulesetAttributeModifier>
            >(RulesetAttributeModifier.SortAttributeModifiersList).Method;

            var unstack = new Action<
                List<RulesetAttributeModifier>,
                RulesetCharacterMonster
            >(MulticlassWildshapeContext.ArmorClassStacking.ProcessWildShapeAc).Method;

            return instructions.ReplaceCalls(sort, "RulesetCharacterMonster.RefreshArmorClass",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, unstack));
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterMonster __instance,
            ref RulesetAttribute __result,
            bool callRefresh,
            bool dryRun,
            FeatureDefinition dryRunFeature)
        {
            foreach (var feature in __instance.GetSubFeaturesByType<IModifyAC>())
            {
                feature.ModifyAC(__instance, callRefresh, dryRun, dryRunFeature, __result);
            }

            RulesetAttributeModifier.SortAttributeModifiersList(__result.ActiveModifiers);
            __result.Refresh(true);
            __instance.SortArmorClassModifierTrends(__result);
            __result.Refresh();

            if (callRefresh && !dryRun)
            {
                __instance.CharacterRefreshed?.Invoke(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.ComputeBaseSavingThrowBonus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeBaseSavingThrowBonus_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterMonster __instance, ref int __result,
            string abilityScoreName,
            List<TrendInfo> savingThrowModifierTrends)
        {
            //PATCH: allows `AddPBToSummonCheck` to add summoner's PB to the saving throws
            AddPBToSummonCheck.ModifyCheckBonus<ISavingThrowPerformanceProvider>(
                __instance, ref __result, abilityScoreName, savingThrowModifierTrends);
        }
    }


    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.ComputeBaseAbilityCheckBonus))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeBaseAbilityCheckBonus_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterMonster __instance, ref int __result,
            List<TrendInfo> abilityCheckModifierTrends,
            string proficiencyName)
        {
            //PATCH: allows `AddPBToSummonCheck` to add summoner's PB to the skill checks
            AddPBToSummonCheck.ModifyCheckBonus<IAbilityCheckPerformanceProvider>(
                __instance, ref __result, proficiencyName, abilityCheckModifierTrends);
        }
    }

    //PATCH: This is very similar to RulesetCharacterHero patch but it's here to support wildshape scenarios
    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.RefreshAttackModes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshAttackModes_Patch
    {
        private static bool _callRefresh;

        [UsedImplicitly]
        public static void Prefix(ref bool callRefresh)
        {
            //save refresh flag, so it can be used in postfix
            _callRefresh = callRefresh;
            //reset refresh flag, so default code won't do refresh before postfix
            callRefresh = false;
        }

        [UsedImplicitly]
        public static void Postfix(RulesetCharacterMonster __instance)
        {
            //PATCH: allow monk bonus unarmed attacks on wild-shaped characters
            MulticlassWildshapeContext.HandleExtraUnarmedAttacks(__instance);

            //PATCH: Allows adding extra attack modes
            __instance.GetSubFeaturesByType<IAddExtraAttack>()
                .ForEach(provider => provider.TryAddExtraAttack(__instance));

            //PATCH: Allows changing damage and other stats of an attack mode
            __instance.AttackModes
                .ForEach(attackMode =>
                    __instance.GetSubFeaturesByType<IModifyWeaponAttackMode>()
                        .ForEach(provider => provider.ModifyAttackMode(__instance, attackMode)));

            //refresh character if needed after postfix
            if (_callRefresh)
            {
                __instance.CharacterRefreshed?.Invoke(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterMonster), nameof(RulesetCharacterMonster.GetRemainingAttackUses))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetRemainingAttackUses_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetCharacterMonster __instance, ref int __result, RulesetAttackMode mode)
        {
            //PATCH: allow monk bonus unarmed attacks on wild-shaped characters
            if (mode == null || __instance.OriginalFormCharacter is not RulesetCharacterHero)
            {
                return;
            }

            var attackModeRank = __instance.GetAttackModeRank(mode);

            if (attackModeRank == -1 && mode.ActionType == ActionDefinitions.ActionType.Bonus)
            {
                __result = -1;
            }
        }
    }
}
