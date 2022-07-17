using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.IgnoreImmunityAndResistance;

[HarmonyPatch(typeof(RulesetActor), "ModulateSustainedDamage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_ModulateSustainedDamage
{
    internal static void Prefix(RulesetActor __instance, RulesetImplementationDefinitions.ApplyFormsParams formsParams, string damageType, int originalDamage,
        ulong sourceGuid, ref List<string> sourceTags)
    {
        if (__instance.IsDeadOrDyingOrUnconscious || __instance.IsDeadOrDyingOrUnconsciousWithNoHealth)
        {
            return;
        }
        
        sourceTags ??= new List<string>();

        foreach (var rulesetCondition in __instance.ConditionsByCategory.SelectMany(keyValuePair => keyValuePair.Value))
        {
            if (rulesetCondition.ConditionDefinition is not IDisableImmunityAndResistanceToDamageType validator)
            {
                continue;
            }

            if (!validator.DisableImmunityAndResistanceToDamageType(damageType))
            {
                continue;
            }

            Main.Log($"TemporaryDisableImmunityAndResistanceTo{damageType}", true);
            sourceTags.Add($"DisableImmunityAndResistanceTo{damageType}");
        }
    }
}

[HarmonyPatch(typeof(FeatureDefinitionDamageAffinity), "ModulateSustainedDamage")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FeatureDefinitionDamageAffinity_ModulateSustainedDamage
{
    internal static void Postfix(FeatureDefinitionDamageAffinity __instance, string damageType, float multiplier, List<string> sourceTags, string ancestryDamageType, ref float __result)
    {
        if (__instance.damageAffinityType != RuleDefinitions.DamageAffinityType.Immunity &&
            __instance.damageAffinityType != RuleDefinitions.DamageAffinityType.Resistance)
        {
            return;
        }

        if (multiplier < 1.0)
        {
            return;
        }
        
        var disabledTag = $"DisableImmunityAndResistanceTo{damageType}";
        if (sourceTags != null && !sourceTags.Contains(disabledTag))
        {
            return;
        }

        Main.Log($"DisableImmunityAndResistanceTo{damageType}", true);
        __result = 1.0f;
    }
}

[HarmonyPatch(typeof(RulesetActor), "IsImmuneToCondition")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_IsImmuneToCondition
{
    internal static void Postfix(RulesetActor __instance, string conditionDefinitionName, ulong sourceGuid, ref bool __result)
    {
        foreach (var rulesetCondition in __instance.ConditionsByCategory.SelectMany(keyValuePair => keyValuePair.Value))
        {
            if (rulesetCondition.ConditionDefinition is not IDisableImmunityToCondition validator)
            {
                continue;
            }

            if (!validator.DisableImmunityToCondition(conditionDefinitionName, sourceGuid))
            {
                continue;
            }

            Main.Log($"TemporaryDisableImmunityToCondition{conditionDefinitionName}", true);
            __result = false;
            return;
        }
    }
}
