using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    // Yes, the actual method name has a typo
    [HarmonyPatch(typeof(RulesetActor), "ProcessConditionsMatchingOccurenceType")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_ProcessConditionsMatchingOccurenceType
    {
        internal static void Postfix(RulesetActor __instance, RuleDefinitions.TurnOccurenceType occurenceType)
        {
            if (occurenceType != RuleDefinitions.TurnOccurenceType.StartOfTurn)
            {
                return;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService?.Battle == null)
            {
                return;
            }

            foreach (GameLocationCharacter contender in battleService.Battle.AllContenders
                .Where(x => x != null && x.Valid && x.RulesetActor != null))
            {
                var conditionsToRemove = new List<RulesetCondition>();

                foreach (KeyValuePair<string, List<RulesetCondition>> keyValuePair in contender.RulesetActor.ConditionsByCategory)
                {
                    foreach (RulesetCondition rulesetCondition in keyValuePair.Value)
                    {
                        if (rulesetCondition.SourceGuid == __instance.Guid && rulesetCondition.ConditionDefinition is IConditionRemovedOnSourceTurnStart)
                        {
                            conditionsToRemove.Add(rulesetCondition);
                        }
                    }
                }

                foreach (RulesetCondition conditionToRemove in conditionsToRemove)
                {
                    contender.RulesetActor.RemoveCondition(conditionToRemove);
                }
            }
        }
    }

    //
    // INotifyConditionRemoval patches
    //

    [HarmonyPatch(typeof(RulesetActor), "RemoveCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RemoveCondition
    {
        internal static void Postfix(RulesetActor __instance, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition?.ConditionDefinition is INotifyConditionRemoval notifiedDefinition)
            {
                notifiedDefinition.AfterConditionRemoved(__instance, rulesetCondition);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetActor), "RollDie")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RollDie
    {
        private static readonly ConcurrentDictionary<RulesetActor, int> NextAbilityCheckMinimum = new();

        internal static void Postfix(
            RulesetActor __instance,
            RuleDefinitions.DieType dieType,
            RuleDefinitions.RollContext rollContext,
            ref int __result)
        {
            if (dieType != RuleDefinitions.DieType.D20 || rollContext != RuleDefinitions.RollContext.AbilityCheck)
            {
                return;
            }

            // This will only come up when RulesetCharacter.ResolveContestCheck is called (usually for shove checks).
            // The ResolveContestCheck patch checks for what the minimum die roll should be when RollDie is called.

            if (!NextAbilityCheckMinimum.TryRemove(__instance, out int minimum))
            {
                // There isn't an entry for the current instance; do nothing
                return;
            }

            if (minimum > __result)
            {
                __result = minimum;
            }
        }

        internal static void SetNextAbilityCheckMinimum(RulesetActor actor, int minimum)
        {
            if (actor == null)
            {
                return;
            }

            NextAbilityCheckMinimum[actor] = minimum;
        }
    }

    //[HarmonyPatch(typeof(RulesetActor), "ModulateSustainedDamage")]
    //[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    //internal static class RulesetActor_ModulateSustainedDamage
    //{
    //    private static IEnumerable<T> ExtractFeaturesHierarchically<T>(RulesetActor rulesetActor) where T : class
    //    {
    //        var featureDefinitions = new List<FeatureDefinition>();

    //        rulesetActor.EnumerateFeaturesToBrowse<T>(featureDefinitions, null);
    //        return featureDefinitions.Select(x => x as T);
    //    }

    //    public static float MyModulateSustainedDamageMethod(
    //        IDamageAffinityProvider affinityProvider,
    //        string damageType,
    //        float multiplier,
    //        List<string> sourceTags,
    //        string ancestryDamageType,
    //        ulong sourceGuid)
    //    {
    //        var rulesetEntityService = ServiceRepository.GetService<IRulesetEntityService>();

    //        if (rulesetEntityService.TryGetEntityByGuid(sourceGuid, out var rulesetEntity) && rulesetEntity is RulesetCharacter rulesetCharacter)
    //        {
    //            var features = ExtractFeaturesHierarchically<IIgnoreDamageAffinity>(rulesetCharacter);

    //            foreach (var feature in features)
    //            {
    //                if (feature.CanIgnoreDamageAffinity(affinityProvider, damageType))
    //                {
    //                    return multiplier;
    //                }
    //            }
    //        }

    //        return affinityProvider.ModulateSustainedDamage(damageType, multiplier, sourceTags, ancestryDamageType);
    //    }

    //    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var found = 0;
    //        var modulateSustainedDamageMethod = typeof(FeatureDefinitionDamageAffinity).GetMethod("ModulateSustainedDamage");
    //        var myModulateSustainedDamageMethod = typeof(RulesetActor_ModulateSustainedDamage).GetMethod("MyModulateSustainedDamageMethod");

    //        foreach (CodeInstruction instruction in instructions)
    //        {
    //            if (instruction.Calls(modulateSustainedDamageMethod) && ++found == 2)
    //            {
    //                yield return new CodeInstruction(OpCodes.Ldarg, 3); // sourceGuid
    //                yield return new CodeInstruction(OpCodes.Call, myModulateSustainedDamageMethod);
    //            }
    //            else
    //            {
    //                yield return instruction;
    //            }
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(RulesetActor), "RollDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetActor_RollDamage
    {
        internal static void Prefix(DamageForm damageForm)
        {
            FeatureDefinitionModifyDamageRollOnDamageType.DamageFormContext = damageForm;
        }

        internal static void Postfix()
        {
            FeatureDefinitionModifyDamageRollOnDamageType.DamageFormContext = null;
        }
    }

    //[HarmonyPatch(typeof(RulesetActor), "RerollDieAsNeeded")]
    //[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    //class RulesetActor_RerollDieAsNeeded
    //{
    //    internal static bool Prefix(
    //        FeatureDefinitionDieRollModifier dieRollModifier,
    //        // RuleDefinitions.DieType dieType,
    //        int rollScore,
    //        ref int __result)
    //    {
    //        if (dieRollModifier is FeatureDefinitionModifyDamageRollDamageTypeDependent modifyDamageRollDamageTypeDependent
    //            && RulesetActor_RollDamage.CurrentDamageForm != null
    //            && !modifyDamageRollDamageTypeDependent.DamageTypes.Contains(RulesetActor_RollDamage.CurrentDamageForm.DamageType))
    //        {
    //            __result = rollScore;

    //            return false;
    //        }

    //        return true;
    //    }
    //}
}
