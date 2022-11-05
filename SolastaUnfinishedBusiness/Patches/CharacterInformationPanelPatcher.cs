using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
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
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: overrides the selected class search term with the one determined by the hotkeys / enumerate class badges logic
            var containsMethod = typeof(string).GetMethod("Contains");
            var getSelectedClassSearchTermMethod =
                new Func<string, string>(CharacterInspectionScreenEnhancement.GetSelectedClassSearchTerm).Method;
            var enumerateClassBadgesMethod = typeof(CharacterInformationPanel).GetMethod("EnumerateClassBadges",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var myEnumerateClassBadgesMethod =
                new Action<CharacterInformationPanel>(CharacterInspectionScreenEnhancement.EnumerateClassBadges).Method;

            // need to replace 2nd and 3rd occurrence so I call it twice looking for 2nd ones...
            return instructions
                .ReplaceCalls(enumerateClassBadgesMethod,
                    "CharacterInformationPanel.Refresh_Patch.EnumerateClassBadges",
                    new CodeInstruction(OpCodes.Call, myEnumerateClassBadgesMethod))
                .ReplaceCall(containsMethod,
                    2, "CharacterInformationPanel.Refresh_Patch.Contains.1",
                    new CodeInstruction(OpCodes.Call, getSelectedClassSearchTermMethod),
                    new CodeInstruction(OpCodes.Call, containsMethod))
                .ReplaceCall(containsMethod,
                    2, "CharacterInformationPanel.Refresh_Patch.Contains.2",
                    new CodeInstruction(OpCodes.Call, getSelectedClassSearchTermMethod),
                    new CodeInstruction(OpCodes.Call, containsMethod));
        }
    }
}
