using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.LevelUp
{
    //
    // Dynamic Feature Sets Fetching
    //

    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        public static List<FeatureDefinition> FeatureSetDynamic(FeatureDefinitionFeatureSet featureDefinitionFeatureSet, FeatureDescriptionItem featureDescriptionItem)
        {
            if (featureDefinitionFeatureSet is FeatureDefinitionFeatureSetDynamic
                && FeatureDescriptionItemPatcher.FeatureDescriptionItems.TryGetValue(featureDescriptionItem, out var featureSet))
            {
                return featureSet;
            }

            return featureDefinitionFeatureSet.FeatureSet;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var featureSetMethod = typeof(FeatureDefinitionFeatureSet).GetMethod("get_FeatureSet");
            var featureSetDynamicMethod = typeof(FeatureDescriptionItem_Bind).GetMethod("FeatureSetDynamic");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(featureSetMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, featureSetDynamicMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(FeatureDescriptionItem), "GetCurrentFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_GetCurrentFeature
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var featureSetMethod = typeof(FeatureDefinitionFeatureSet).GetMethod("get_FeatureSet");
            var featureSetDynamicMethod = typeof(FeatureDescriptionItem_Bind).GetMethod("FeatureSetDynamic");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(featureSetMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, featureSetDynamicMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    internal static class FeatureDescriptionItemPatcher
    {
        internal static Dictionary<FeatureDescriptionItem, List<FeatureDefinition>> FeatureDescriptionItems { get; } = new();

        private static void KeepSelectionsUnique(FeatureDescriptionItem __instance)
        {
            var ___choiceDropdown = __instance.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

            foreach (var featureDescriptionItem in FeatureDescriptionItems
                .Where(x => x.Key != __instance && x.Key.Feature == __instance.Feature))
            {
                var choiceDropdown = featureDescriptionItem.Key.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                if (choiceDropdown.value == ___choiceDropdown.value)
                {
                    ___choiceDropdown.value = (___choiceDropdown.value + 1) % ___choiceDropdown.options.Count;
                }
            }
        }

        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class FeatureDescriptionItem_Bind
        {
            // supports dynamic feature sets
            internal static void Prefix(FeatureDescriptionItem __instance, FeatureDefinition featureDefinition)
            {
                List<FeatureDefinition> featureSet = null;

                if (featureDefinition is FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
                {
                    featureSet = featureDefinitionFeatureSetDynamic.DynamicFeatureSet.Invoke(featureDefinitionFeatureSetDynamic);
                }

                if (featureDefinition is FeatureDefinitionFeatureSet)
                {
                    // need a TryAdd here to avoid issues with other level up screens
                    FeatureDescriptionItems.TryAdd(__instance, featureSet);
                }
            }

            // enforce unique choices when more than one feature set is available
            internal static void Postfix(FeatureDescriptionItem __instance)
            {
                if (!Main.Settings.EnableEnforceUniqueFeatureSetChoices)
                {
                    return;
                }

                __instance.ValueChanged += KeepSelectionsUnique;
                KeepSelectionsUnique(__instance);
            }
        }
    }
}
