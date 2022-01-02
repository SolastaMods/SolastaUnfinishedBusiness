using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Helpers;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(GameGadget), "CheckIsEnabled")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_CheckIsEnabled
    {
        public static void Postfix(GameGadget __instance, ref bool __result)
        {
            if (!Main.Settings.BugFixGameGadgetCheckIsEnabled)
            {
                return;
            }

            var original = __result;

            // To agree with the game code, if neither Enabled or Param_Enabled are present then return true.
            // This ensures CheckIsEnabled returns true for gadgets without an Enabled state, which is expected in GameLocationScreenMap.BindGadgets().
            __result = __instance.IsEnabled(true);

            // The bug being fixed is that if:
            // 'Param_Enabled' is present and set to 'false' and 'Enabled' is not present, then the game code returns 'true'.
            // The fix changes that to 'false'.

            Main.Log($"CheckIsEnabled: {__instance.UniqueNameId}, original={original}, new={__result}");
        }
    }

    [HarmonyPatch(typeof(GameGadget), "SetCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_SetCondition
    {
        /// <summary>
        /// Fix issue where a button activator fires Triggered event with state=true first time and correctly activates attached gadget,
        /// but fires Triggered event with state=false second time and doesn't activate attached gadget.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="conditionIndex"></param>
        /// <param name="state"></param>
        /// <param name="___conditionNames"></param>
        public static void Postfix(GameGadget __instance, int conditionIndex, bool state, List<string> ___conditionNames)
        {
            if (!Main.Settings.BugFixButtonActivatorTriggerIssue)
            {
                return;
            }

            if (conditionIndex >= 0 && conditionIndex < ___conditionNames.Count)
            {
                var param = ___conditionNames[conditionIndex];

                // NOTE: only handling 'button activator'
                // TODO: check other activators for same issue
                if (param == GameGadgetExtensions.Triggered && !state && __instance.UniqueNameId.StartsWith("ActivatorButton"))
                {
                    Main.Log($"GameGadget_SetCondition: Resetting '{param}' to true.");

                    // Reset 'Triggered' to true otherwise we have to press the activator twice
                    __instance.SetCondition(conditionIndex, true, new List<GameLocationCharacter>());
                }
            }
        }
    }
}
