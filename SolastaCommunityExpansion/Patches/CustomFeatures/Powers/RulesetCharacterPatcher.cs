using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers
{
    [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_UsePower
    {
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            __instance.UpdateUsageForPowerPool(usablePower, usablePower.PowerDefinition.CostPerUse);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RepayPowerUse")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RepayPowerUse
    {
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            __instance.UpdateUsageForPowerPool(usablePower, -usablePower.PowerDefinition.CostPerUse);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "GrantPowers")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_GrantPowers
    {
        public static void Postfix(RulesetCharacter __instance)
        {
            CustomFeaturesContext.RechargeLinkedPowers(__instance, RuleDefinitions.RestType.LongRest);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_ApplyRest
    {
        internal static void Postfix(
            RulesetCharacter __instance, RuleDefinitions.RestType restType, bool simulate)
        {
            if (!simulate)
            {
                CustomFeaturesContext.RechargeLinkedPowers(__instance, restType);
            }

            // The player isn't recharging the shared pool features, just the pool.
            // Hide the features that use the pool from the UI.
            foreach (var feature in __instance.RecoveredFeatures.Where(f => f is IPowerSharedPool).ToArray())
            {
                __instance.RecoveredFeatures.Remove(feature);
            }
        }
        
        // Makes powers that have their max usage extended by pool modifiers show up correctly during rest
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var maxUses =
                new Func<RulesetUsablePower, RulesetCharacter, int>(CustomFeaturesContext.GetMaxUsesForPool).Method;
            
            var bind = typeof(RulesetUsablePower).GetMethod("get_MaxUses", BindingFlags.Public | BindingFlags.Instance);

            var bindIndex = codes.FindIndex(x => x.Calls(bind));

            if (bindIndex > 0)
            {
                codes[bindIndex] = new CodeInstruction(OpCodes.Call, maxUses);
                codes.Insert(bindIndex, new CodeInstruction(OpCodes.Ldarg_0));
            }

            return codes.AsEnumerable();
        }
        
    }
}
