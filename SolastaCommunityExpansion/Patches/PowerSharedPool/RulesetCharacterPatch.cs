using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.PowerSharedPool
{
    class RulesetCharacterPatch
    {
        [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
        internal static class RulesetCharacter_UsePower
        {
            public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
            {
                UpdateUsageForPowerPool(__instance, usablePower, usablePower.PowerDefinition.CostPerUse);
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "RepayPowerUse")]
        internal static class RulesetCharacter_RepayPowerUse
        {
            public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
            {
                UpdateUsageForPowerPool(__instance, usablePower, -usablePower.PowerDefinition.CostPerUse);
            }
        }

        private static void UpdateUsageForPowerPool(RulesetCharacter character, RulesetUsablePower modifiedPower, int poolUsage)
        {
            if (!(modifiedPower.PowerDefinition is IPowerSharedPool))
            {
                return;
            }
            IPowerSharedPool sharedPoolPower = (IPowerSharedPool)modifiedPower.PowerDefinition;
            foreach (RulesetUsablePower poolPower in character.UsablePowers)
            {
                if (poolPower.PowerDefinition == sharedPoolPower.GetUsagePoolPower())
                {
                    int remainingUses = Mathf.Clamp(poolPower.RemainingUses - poolUsage, 0, poolPower.MaxUses);
                    Traverse.Create(poolPower).Field<int>("remainingUses").Value = remainingUses;

                    // Find powers that rely on this pool
                    foreach (RulesetUsablePower usablePower in character.UsablePowers)
                    {
                        if (usablePower.PowerDefinition is IPowerSharedPool)
                        {
                            FeatureDefinitionPower pointPoolPower = ((IPowerSharedPool)usablePower.PowerDefinition).GetUsagePoolPower();
                            if (pointPoolPower == poolPower.PowerDefinition)
                            {
                                // Set remaining uses to max uses
                                Traverse.Create(usablePower).Field("remainingUses").SetValue(remainingUses / usablePower.PowerDefinition.CostPerUse);
                            }
                        }
                    }
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "GrantPowers")]
        internal static class RulesetCharacter_GrantPowers
        {
            public static void Postfix(RulesetCharacter __instance)
            {
                SetLinkedPowerUsages(__instance, true);
            }
        }

        [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
        internal static class RulesetCharacter_ApplyRest
        {
            internal static void Postfix(RulesetCharacter __instance,
                                        bool simulate)
            {
                if (!simulate)
                {
                    SetLinkedPowerUsages(__instance, false);
                }
                // The player isn't recharging the shared pool features, just the pool.
                // Hide the features that use the pool from the UI.
                foreach (FeatureDefinition feature in __instance.RecoveredFeatures.ToArray())
                {
                    if (feature is IPowerSharedPool)
                    {
                        __instance.RecoveredFeatures.Remove(feature);
                    }
                }
            }
        }

        private static void SetLinkedPowerUsages(RulesetCharacter character, bool recalculateMaxUsage)
        {
            List<FeatureDefinitionPower> pointPoolPowerDefinitions = new List<FeatureDefinitionPower>();
            foreach (RulesetUsablePower usablePower in character.UsablePowers)
            {
                if (usablePower.PowerDefinition is IPowerSharedPool)
                {
                    FeatureDefinitionPower pointPoolPower = ((IPowerSharedPool)usablePower.PowerDefinition).GetUsagePoolPower();
                    if (!pointPoolPowerDefinitions.Contains(pointPoolPower))
                    {
                        pointPoolPowerDefinitions.Add(pointPoolPower);
                    }
                }
            }

            // Find the UsablePower of the point pool powers.
            foreach (RulesetUsablePower poolPower in character.UsablePowers)
            {
                if (pointPoolPowerDefinitions.Contains(poolPower.PowerDefinition))
                {
                    if (recalculateMaxUsage)
                    {
                        int poolSize = GetMaxUsesForPool(poolPower, character);
                        // We don't set the max uses for the base pool because 
                        poolPower.AddUses(poolSize - poolPower.MaxUses);
                        poolPower.Recharge();
                    }
                    int totalPoolSize = poolPower.MaxUses;

                    // Find powers that rely on this pool
                    foreach (RulesetUsablePower usablePower in character.UsablePowers)
                    {
                        if (usablePower.PowerDefinition is IPowerSharedPool)
                        {
                            FeatureDefinitionPower pointPoolPower = ((IPowerSharedPool)usablePower.PowerDefinition).GetUsagePoolPower();
                            if (pointPoolPower == poolPower.PowerDefinition)
                            {
                                // Set max uses modified by power cost
                                usablePower.AddUses(totalPoolSize / usablePower.PowerDefinition.CostPerUse - usablePower.MaxUses);
                                // Set remaining uses to max uses
                                usablePower.Recharge();
                            }
                        }
                    }
                }
            }
        }

        private static int GetMaxUsesForPool(RulesetUsablePower poolPower, RulesetCharacter character)
        {
            int totalPoolSize = poolPower.MaxUses;

            foreach (RulesetUsablePower modifierPower in character.UsablePowers)
            {
                if (modifierPower.PowerDefinition is IPowerPoolModifier)
                {
                    FeatureDefinitionPower poolModifierDef = ((IPowerPoolModifier)modifierPower.PowerDefinition).GetUsagePoolPower();
                    if (poolModifierDef == poolPower.PowerDefinition)
                    {
                        totalPoolSize += modifierPower.MaxUses;
                    }
                }
            }
            return totalPoolSize;
        }
    }
}
