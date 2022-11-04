using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterInformationPanelPatcher
{
    [HarmonyPatch(typeof(CharacterInformationPanel), "EnumerateFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EnumerateFeatures_Patch
    {
        public static bool Prefix(
            CharacterInformationPanel __instance,
            RectTransform table,
            List<FeatureUnlockByLevel> features,
            string insufficientLevelFormat,
            TooltipDefinitions.AnchorMode tooltipAnchorMode)
        {
            //PATCH: enhances class feature list tooltip and feature name for feature sets
            return CharacterInspectionScreenEnhancement.EnhanceFeatureList(__instance, table, features,
                insufficientLevelFormat, tooltipAnchorMode);
        }
    }

    [HarmonyPatch(typeof(CharacterInformationPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(CharacterInformationPanel __instance)
        {
            //PATCH: Switches positions of Class and Background descriptions, and class selector for MC characters
            CharacterInspectionScreenEnhancement.SwapClassAndBackground(__instance);
        }
    }

    [HarmonyPatch(typeof(CharacterInformationPanel), "Refresh")]
    public static class Refresh_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: overrides the selected class search term with the one determined by the hotkeys / enumerate class badges logic
            return CharacterInspectionScreenEnhancement.EnableClassSelector(instructions);
        }
    }
}
