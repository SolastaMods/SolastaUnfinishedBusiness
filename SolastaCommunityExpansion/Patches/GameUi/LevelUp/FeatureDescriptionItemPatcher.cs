using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        public static void FilterAdd(List<FeatureDefinition> availableFeatures, FeatureDefinition featureDefinition)
        {
            if (featureDefinition is IFeatureDefinitionWithPrerequisites featureDefinitionWithPrerequisites
                && featureDefinitionWithPrerequisites.Validator != null
                && !featureDefinitionWithPrerequisites.Validator())
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
    }
}
