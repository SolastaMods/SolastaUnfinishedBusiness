using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(FeatureDescriptionItem), "GetCurrentFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_GetCurrentFeature
    {
        //
        // Dynamic Feature Sets
        //

        public static List<FeatureDefinition> FeatureSetDynamic(FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
        {
            if (featureDefinitionFeatureSet is FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
            {
                return featureDefinitionFeatureSetDynamic.DynamicFeatureSet(featureDefinitionFeatureSetDynamic);
            }

            return featureDefinitionFeatureSet.FeatureSet;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var featureSetMethod = typeof(FeatureDefinitionFeatureSet).GetMethod("get_FeatureSet");
            var featureSetDynamicMethod = typeof(FeatureDescriptionItem_GetCurrentFeature).GetMethod("FeatureSetDynamic");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(featureSetMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, featureSetDynamicMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        //
        // Dynamic Feature Sets
        //

        public static List<FeatureDefinition> FeatureSetDynamic(FeatureDefinitionFeatureSet featureDefinitionFeatureSet)
        {
            if (featureDefinitionFeatureSet is FeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
            {
                return featureDefinitionFeatureSetDynamic.DynamicFeatureSet(featureDefinitionFeatureSetDynamic);
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
                    yield return new CodeInstruction(OpCodes.Call, featureSetDynamicMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        //
        // Unique Feature Set Choices
        //

        // ensures we try to offer unique default options
        internal static void Postfix(FeatureDescriptionItem __instance)
        {
            if (!Main.Settings.EnableEnforceUniqueFeatureSetChoices)
            {
                return;
            }

            CharacterStageClassSelectionPanel_Refresh.FeatureDescriptionItems.Add(__instance);
            __instance.ValueChanged += ValueChanged;

            ValueChanged(__instance);
        }

        // ensures we change any other drop down equals to this one
        private static void ValueChanged(FeatureDescriptionItem __instance)
        {
            var ___choiceDropdown = __instance.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

            foreach (var featureDescriptionItem in CharacterStageClassSelectionPanel_Refresh.FeatureDescriptionItems
                .Where(x => x != __instance && x.Feature == __instance.Feature))
            {
                var choiceDropDown = featureDescriptionItem.GetField<FeatureDescriptionItem, GuiDropdown>("choiceDropdown");

                if (choiceDropDown.value == ___choiceDropdown.value)
                {
                    choiceDropDown.value = (choiceDropDown.value + 1) % choiceDropDown.options.Count;
                }
            }
        }
    }
}
