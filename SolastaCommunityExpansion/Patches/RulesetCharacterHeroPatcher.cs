using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetCharacterHeroPatcher
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "FindClassHoldingFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FindClassHoldingFeature_Patch
    {
        internal static void Postfix(
            RulesetCharacterHero __instance,
            FeatureDefinition featureDefinition,
            ref CharacterClassDefinition __result)
        {
            //PATCH: replaces feature holding class with one provided by custom interface
            //used for features that are not granted directly through class but need to scale with class levels
            var classHolder = featureDefinition.GetFirstSubFeatureOfType<IClassHoldingFeature>()?.Class;
            if (classHolder == null)
            {
                return;
            }

            // Only override if the character actually has levels in the class, to prevent errors
            if (__instance.ClassesAndLevels.TryGetValue(classHolder, out var levelsInClass) && levelsInClass > 0)
            {
                __result = classHolder;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "ComputeAttackModeAbilityScoreReplacement")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeAttackModeAbilityScoreReplacement_Patch
    {
        internal static void Prefix(RulesetCharacterHero __instance, RulesetAttackMode attackMode, RulesetItem weapon)
        {
            //PATCH: Allows changing what attribute is used for weapon's attack and damage rolls
            __instance.GetSubFeaturesByType<IModifyAttackAttributeForWeapon>()
                .ForEach(modifier => modifier.ModifyAttribute(__instance, attackMode, weapon));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAttackMode")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class R1efreshAttackMode_Patch
    {
        //TODO: remove unused arguments
        internal static void Postfix(RulesetCharacterHero __instance,
            ref RulesetAttackMode __result,
            ActionDefinitions.ActionType actionType,
            ItemDefinition itemDefinition,
            WeaponDescription weaponDescription,
            bool freeOffHand,
            bool canAddAbilityDamageBonus,
            string slotName,
            List<IAttackModificationProvider> attackModifiers,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin,
            RulesetItem weapon = null)
        {
            //PATCH: Allows changing damage and other stats of an attack mode
            var mode = __result;
            __instance.GetSubFeaturesByType<IModifyAttackModeForWeapon>()
                .ForEach(modifier => modifier.ModifyAttackMode(__instance, mode, weapon));
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAttackModes")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAttackModes_Patch
    {
        private static bool _callRefresh;

        internal static void Prefix(ref bool callRefresh)
        {
            //save refresh flag, so it can be used in postfix
            _callRefresh = callRefresh;
            //reset refresh flag, so default code won't do refresh before postfix
            callRefresh = false;
        }

        internal static void Postfix(RulesetCharacterHero __instance, bool callRefresh = false)
        {
            //PATCH: Allows adding extra attack modes
            __instance.GetSubFeaturesByType<IAddExtraAttack>()
                .ForEach(provider => provider.TryAddExtraAttack(__instance));

            //refresh character if needed after postfix
            if (_callRefresh && __instance.CharacterRefreshed != null)
            {
                __instance.CharacterRefreshed(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshAll")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshAll_Patch
    {
        internal static void Prefix(RulesetCharacterHero __instance)
        {
            //PATCH: Support for `IHeroRefreshedListener`
            __instance.GetSubFeaturesByType<IHeroRefreshedListener>()
                .ForEach(listener => listener.OnHeroRefreshed(__instance));
        }
    }


    [HarmonyPatch(typeof(RulesetCharacterHero), "RefreshActiveFightingStyles")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RefreshActiveFightingStyles_Patch
    {
        internal static void Postfix(RulesetCharacterHero __instance)
        {
            //PATCH: enables some corner-case fighting styles (like archery for hand crossbows and dual wielding for shield expert)
            FightingStyleContext.RefreshFightingStylesPatch(__instance);
        }
    }
    
    [HarmonyPatch(typeof(RulesetCharacterHero), "AcknowledgeAttackUse")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class AcknowledgeAttackUse
    {
        // ReSharper disable once RedundantAssignment
        internal static void Prefix(RulesetCharacterHero __instance,
            RulesetAttackMode mode)
        {
            //PATCH: supports turning Produced Flame into a weapon
            //destroys Produced Flame after attacking with it
            CustomWeaponsContext.ProcessProducedFlameAttack(__instance, mode);
        }
    }
}