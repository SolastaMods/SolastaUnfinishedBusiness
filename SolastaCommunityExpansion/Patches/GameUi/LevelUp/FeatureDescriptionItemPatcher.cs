using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
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
                && FeatureDescriptionItemPatcher.FeatureDescriptionItems.TryGetValue(featureDescriptionItem, out var tab))
            {
                return tab.FeatureSet.Keys.ToList();
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

    //
    // Dynamic Feature Sets Caching / Unique Feature Set Choices / Replace Features
    //

    internal static class FeatureDescriptionItemPatcher
    {
        internal const string ReplaceTag = "Replace";
        internal class FeatureDescriptionItemTab
        {
            public Dictionary<FeatureDefinition, string> FeatureSet { get; set; } = new();

            public FeatureDefinition SelectedFeature;
        }

        internal static Dictionary<FeatureDescriptionItem, FeatureDescriptionItemTab> FeatureDescriptionItems { get; } = new();

        private static void RefreshDropdownOptions(FeatureDescriptionItem __instance)
        {
            var ___choiceDropdown = __instance.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");
            var featureSetNamePrefix = __instance.name.Replace(ReplaceTag, string.Empty);

            if (!FeatureDescriptionItems.TryGetValue(__instance, out var tab))
            {
                return;
            }

            if (tab.SelectedFeature != null)
            {
                var optionToRemove = ___choiceDropdown.options.Find(x => x.text == tab.SelectedFeature.FormatTitle());

                foreach (var featureDescriptionItem in FeatureDescriptionItems
                    .Where(x => x.Key != __instance && x.Key.Feature.Name.StartsWith(featureSetNamePrefix)))
                {
                    var choiceDropDown = featureDescriptionItem.Key.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                    choiceDropDown.options.Remove(optionToRemove);
                }
            }

            var selectedOption = ___choiceDropdown.options[___choiceDropdown.value];

            foreach (var featureDescriptionItem in FeatureDescriptionItems
                .Where(x => x.Key != __instance && x.Key.Feature.Name.StartsWith(featureSetNamePrefix)))
            {
                var choiceDropDown = featureDescriptionItem.Key.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                choiceDropDown.options.Add(selectedOption);
                choiceDropDown.options.Sort((a,b) => a.text.CompareTo(b.text));
                choiceDropDown.RefreshShownValue();
            }

            tab.SelectedFeature = __instance.GetCurrentFeature();
        }

        private static void KeepSelectionsUnique(FeatureDescriptionItem __instance)
        {
            if (!Main.Settings.EnableEnforceUniqueFeatureSetChoices)
            {
                return;
            }

            var ___choiceDropdown = __instance.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

            foreach (var featureDescriptionItem in FeatureDescriptionItems
                .Where(x => x.Key != __instance && x.Key.Feature == __instance.Feature))
            {
                var choiceDropDown = featureDescriptionItem.Key.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                if (choiceDropDown.value == ___choiceDropdown.value)
                {
                    choiceDropDown.value = (choiceDropDown.value + 1) % choiceDropDown.options.Count;
                }
            }
        }

        [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class FeatureDescriptionItem_Bind
        {
            internal static void Prefix(
                FeatureDescriptionItem __instance,
                FeatureDefinition featureDefinition)
            {
                if (featureDefinition is not FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
                {
                    return;
                }

                var tab = new FeatureDescriptionItemTab();
                FeatureDescriptionItems.TryAdd(__instance, tab);

                tab.FeatureSet = featureDefinitionFeatureSetDynamic.DynamicFeatureSet.Invoke(featureDefinitionFeatureSetDynamic);
            }

            internal static void Postfix(
                FeatureDescriptionItem __instance,
                FeatureDefinition featureDefinition)
            {
                if (featureDefinition is not FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
                {
                    return;
                }

                if (featureDefinition.Name.EndsWith(ReplaceTag))
                {
                    __instance.ValueChanged += RefreshDropdownOptions;
                    RefreshDropdownOptions(__instance);
                }
                else
                {
                    __instance.ValueChanged += KeepSelectionsUnique;
                    KeepSelectionsUnique(__instance);
                }
            }
        }
    }
}
