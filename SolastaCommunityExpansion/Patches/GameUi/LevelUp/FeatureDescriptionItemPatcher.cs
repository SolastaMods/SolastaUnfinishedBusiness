using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.LevelUp
{
    // dynamic feature sets
    [HarmonyPatch(typeof(FeatureDescriptionItem), "GetCurrentFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_GetCurrentFeature
    {
        //
        // Dynamic Feature Sets
        //

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

    // dynamic feature sets
    [HarmonyPatch(typeof(FeatureDescriptionItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FeatureDescriptionItem_Bind
    {
        //
        // Dynamic Feature Sets
        //

        public static List<FeatureDefinition> FeatureSetDynamic(FeatureDefinitionFeatureSet featureDefinitionFeatureSet, FeatureDescriptionItem featureDescriptionItem)
        {
            if (featureDefinitionFeatureSet is IFeatureDefinitionFeatureSetDynamic
                && FeatureDescriptionItems.TryGetValue(featureDescriptionItem, out var tab))
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

        //
        // Unique Feature Set Choices / Removal Feature Set Choices
        //

        internal class FeatureDescriptionItemTab
        {
            public Dictionary<FeatureDefinition, string> FeatureSet { get; set; } = new();

            // only used in removal behavior
            public FeatureDefinition SelectedFeature;
        }

        internal static Dictionary<FeatureDescriptionItem, FeatureDescriptionItemTab> FeatureDescriptionItems { get; } = new();

        internal static void Prefix(FeatureDescriptionItem __instance, FeatureDefinition featureDefinition)
        {
            if (featureDefinition is not FeatureDefinitionFeatureSet)
            {
                return;
            }

            var tab = new FeatureDescriptionItemTab();
            FeatureDescriptionItems.Add(__instance, tab);

            if (featureDefinition is IFeatureDefinitionFeatureSetDynamic featureDefinitionFeatureSetDynamic)
            {
                tab.FeatureSet = featureDefinitionFeatureSetDynamic.DynamicFeatureSet.Invoke((FeatureDefinitionFeatureSet)featureDefinition);
            }
        }

        // ensures we try to offer unique default options
        internal static void Postfix(FeatureDescriptionItem __instance)
        {
            if (__instance.Feature is FeatureDefinitionFeatureSetUniqueAcross featureDefinitionFeatureSetUniqueAcross)
            {
                if (featureDefinitionFeatureSetUniqueAcross.BehaviorTags
                    .Contains(FeatureDefinitionFeatureSetUniqueAcross.REMOVE_BEHAVIOR))
                {
                    __instance.ValueChanged += GrantRemoveLogic;
                    GrantRemoveLogic(__instance);
                }

                __instance.ValueChanged += KeepUniqueAcrossLogic;
                KeepUniqueAcrossLogic(__instance);
            }
        }

        private static void GrantRemoveLogic(FeatureDescriptionItem __instance)
        {
            var hero = Global.ActiveLevelUpHero;
            var heroBuildingData = hero?.GetHeroBuildingData();

            if (hero == null || heroBuildingData == null)
            {
                return;
            }

            if (FeatureDescriptionItems.TryGetValue(__instance, out var tab))
            {
                string tag;

                if (tab.SelectedFeature != null)
                {
                    tag = tab.FeatureSet[tab.SelectedFeature];
                    hero.ActiveFeatures[tag].Add(tab.SelectedFeature);
                }

                tab.SelectedFeature = __instance.GetCurrentFeature();

                tag = tab.FeatureSet[tab.SelectedFeature];
                hero.ActiveFeatures[tag].Remove(tab.SelectedFeature);
            }
        }

        // ensures we change any other drop down equals to this one
        private static void KeepUniqueAcrossLogic(FeatureDescriptionItem __instance)
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
    }
}
