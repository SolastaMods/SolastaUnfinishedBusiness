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
            if (featureDefinitionFeatureSet is FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
            {
                return featureDefinitionFeatureSetDynamic.DynamicFeatureSet.Invoke(featureDefinitionFeatureSet);
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
        internal static HashSet<FeatureDescriptionItem> FeatureDescriptionItems { get; } = new();

        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class FeatureDescriptionItem_Bind
        {
            private static void KeepSelectionsUnique(FeatureDescriptionItem __instance)
            {
                var ___choiceDropdown = __instance.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                foreach (var featureDescriptionItem in FeatureDescriptionItems
                    .Where(x => x != __instance && x.Feature == __instance.Feature))
                {
                    var choiceDropdown = featureDescriptionItem.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                    if (choiceDropdown.value == ___choiceDropdown.value)
                    {
                        ___choiceDropdown.value = (___choiceDropdown.value + 1) % ___choiceDropdown.options.Count;
                    }
                }
            }

            // keep a tab of FeatureDescriptionItem and enforce unique choices when more than one feature set is available
            internal static void Postfix(FeatureDescriptionItem __instance)
            {
                if (__instance.Feature is FeatureDefinitionFeatureSet && !FeatureDescriptionItems.Contains(__instance))
                {
                    FeatureDescriptionItems.Add(__instance);
                }

                if (Main.Settings.EnableEnforceUniqueFeatureSetChoices)
                {
                    __instance.ValueChanged += KeepSelectionsUnique;
                    KeepSelectionsUnique(__instance);
                }
            }
        }
    }
}
