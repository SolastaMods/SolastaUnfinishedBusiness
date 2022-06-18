using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures;

internal static class GameLocationCharacterPatcher
{
    [HarmonyPatch(typeof(GameLocationCharacter), "RefreshActionPerformances")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_RefreshActionPerformances
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var customMethod =
                new Action<RulesetActor, List<FeatureDefinition>,
                    Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin>>(CustomEnumerate).Method;

            var bindIndex = codes.FindIndex(x =>
            {
                if (x.operand == null)
                {
                    return false;
                }

                var operand = x.operand.ToString();
                if (operand.Contains("EnumerateFeaturesToBrowse") && operand.Contains("IActionPerformanceProvider"))
                {
                    return true;
                }

                return false;
            });

            if (bindIndex > 0)
            {
                codes[bindIndex] = new CodeInstruction(OpCodes.Call, customMethod);
            }

            return codes.AsEnumerable();
        }

        private static void CustomEnumerate(RulesetActor actor, List<FeatureDefinition> features,
            Dictionary<FeatureDefinition, RuleDefinitions.FeatureOrigin> featuresOrigin = null)
        {
            actor.EnumerateFeaturesToBrowse<IActionPerformanceProvider>(features);
            if (actor is not RulesetCharacter character)
            {
                return;
            }

            features.RemoveAll(f =>
            {
                var validator = f.GetFirstSubFeatureOfType<IFeatureApplicationValidator>();
                return validator != null && !validator.IsValid(character);
            });
        }
    }

    // Removes check that makes `ShoveBonus` unavailable if character has no shield
    [HarmonyPatch(typeof(GameLocationCharacter), "GetActionStatus")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationCharacter_GetActionStatus
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var customMethod = new Func<RulesetActor, bool>(CustomMethod).Method;

            var bindIndex = codes.FindIndex(x =>
            {
                if (x.operand == null)
                {
                    return false;
                }

                var operand = x.operand.ToString();
                return operand.Contains("IsWearingShield");
            });

            if (bindIndex > 0)
            {
                codes[bindIndex] = new CodeInstruction(OpCodes.Call, customMethod);
            }

            return codes.AsEnumerable();
        }

        private static bool CustomMethod(RulesetActor actor)
        {
            return true;
        }
    }
}
