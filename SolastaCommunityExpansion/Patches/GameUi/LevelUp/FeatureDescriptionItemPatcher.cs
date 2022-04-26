using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        //
        // Features With Pre Requisites
        //

        public static void FilterAdd(List<FeatureDefinition> availableFeatures, FeatureDefinition featureDefinition)
        {
            if (featureDefinition is IFeatureDefinitionWithPrerequisites featureDefinitionWithPrerequisites
                && !featureDefinitionWithPrerequisites.Validators.All(x => x.Invoke()))
            {
                return;
            }

            availableFeatures.Add(featureDefinition);
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var availableFeaturesField = typeof(FeatureDescriptionItem).GetField("availableFeatures", BindingFlags.NonPublic | BindingFlags.Instance);
            var filterAddMethod = typeof(FeatureDescriptionItem_Bind).GetMethod("FilterAdd");

            var found = 0;
            var code = new List<CodeInstruction>(instructions);

            for (var i = 0; i < code.Count; i++)
            {
                var instruction = code[i];

                if (instruction.LoadsField(availableFeaturesField) && ++found == 2)
                {
                    code[i + 2] = new CodeInstruction(OpCodes.Call, filterAddMethod);
                }
            }

            return code;
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
