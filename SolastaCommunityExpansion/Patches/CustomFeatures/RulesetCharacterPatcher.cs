using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    [HarmonyPatch(typeof(RulesetCharacter), "Kill")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_Kill
    {
        internal static void Prefix(RulesetCharacter __instance)
        {
            foreach (var keyValuePair in __instance.ConditionsByCategory)
            {
                foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                {
                    if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
                    {
                        notifiedDefinition.BeforeDyingWithCondition(__instance, rulesetCondition);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RechargePowersForTurnStart")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RechargePowersForTurnStart
    {
        internal static void Postfix(RulesetCharacter __instance)
        {
            foreach (RulesetUsablePower usablePower in __instance.UsablePowers)
            {
                if (usablePower?.PowerDefinition is IStartOfTurnRecharge startOfTurnRecharge && usablePower.RemainingUses < usablePower.MaxUses)
                {
                    usablePower.Recharge();

                    if (!startOfTurnRecharge.IsRechargeSilent && __instance.PowerRecharged != null)
                    {
                        __instance.PowerRecharged(__instance, usablePower);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RollAbilityCheck")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RollAbilityCheck
    {
        internal static void Prefix(
            RulesetCharacter __instance,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            ref int minRoll)
        {
            if (abilityScoreName != AttributeDefinitions.Strength)
            {
                return;
            }

            int? modifiedMinRoll = RulesetCharacter_ResolveContestCheck.MinimumStrengthAbilityCheckDieRoll(__instance, baseBonus, rollModifier, proficiencyName);

            if (modifiedMinRoll.HasValue && modifiedMinRoll.Value > minRoll)
            {
                minRoll = modifiedMinRoll.Value;
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "ResolveContestCheck")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_ResolveContestCheck
    {
        internal static void Prefix(
            RulesetCharacter __instance,
            int baseBonus,
            int rollModifier,
            string abilityScoreName,
            string proficiencyName,
            RulesetCharacter opponent,
            int opponentBaseBonus,
            int opponentRollModifier,
            string opponentAbilityScoreName,
            string opponentProficiencyName)
        {
            // ResolveContestCheck calls RulesetActor.RollDie twice (once for you, once for your opponent).
            // SetNextAbilityCheckMinimum below tells the RulesetActor.RollDie patch that the next call to RollDie (for the specified RulesetCharacter) must have a certain minimum value.

            if (abilityScoreName == AttributeDefinitions.Strength)
            {
                int? instanceMinRoll = MinimumStrengthAbilityCheckDieRoll(__instance, baseBonus, rollModifier, proficiencyName);

                if (instanceMinRoll.HasValue)
                {
                    RulesetActor_RollDie.SetNextAbilityCheckMinimum(__instance, instanceMinRoll.Value);
                }
            }

            if (opponentAbilityScoreName == AttributeDefinitions.Strength)
            {
                int? opponentMinRoll = MinimumStrengthAbilityCheckDieRoll(opponent, opponentBaseBonus, opponentRollModifier, opponentProficiencyName);

                if (opponentMinRoll.HasValue)
                {
                    RulesetActor_RollDie.SetNextAbilityCheckMinimum(opponent, opponentMinRoll.Value);
                }
            }
        }

        internal static int? MinimumStrengthAbilityCheckDieRoll(RulesetCharacter character, int baseBonus, int rollModifier, string proficiencyName)
        {
            if (character == null)
            {
                return null;
            }

            int? minimumTotal = MinimumStrengthAbilityCheckTotal(character, proficiencyName);

            if (!minimumTotal.HasValue)
            {
                return null;
            }

            // Set the minimum die roll based on the bonuses, which indirectly sets the minimum total.
            // This can result in impossible die rolls, like rolling 23 on a d20, which the game doesn't seem to mind.
            return minimumTotal.Value - (baseBonus + rollModifier);
        }

        private static int? MinimumStrengthAbilityCheckTotal(RulesetCharacter character, string proficiencyName)
        {
            var featuresToBrowse = new List<FeatureDefinition>();

            character.EnumerateFeaturesToBrowse<IMinimumAbilityCheckTotal>(featuresToBrowse);

            return featuresToBrowse
                .OfType<IMinimumAbilityCheckTotal>()
                .Max(feature => feature.MinimumStrengthAbilityCheckTotal(character, proficiencyName));
        }
    }

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
            Models.CustomFeaturesContext.RechargeLinkedPowers(__instance, RuleDefinitions.RestType.LongRest);
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
                Models.CustomFeaturesContext.RechargeLinkedPowers(__instance, restType);
            }

            // The player isn't recharging the shared pool features, just the pool.
            // Hide the features that use the pool from the UI.
            foreach (FeatureDefinition feature in __instance.RecoveredFeatures.Where(f => f is IPowerSharedPool).ToArray())
            {
                __instance.RecoveredFeatures.Remove(feature);
            }
        }
    }
}
