#if false
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(GameGadget), "SetCondition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    // Not fixed as of 1.3.17
    internal static class GameGadget_SetCondition
    {
        /// <summary>
        /// Fix issue where a button activator fires Triggered event with state=true first time and correctly activates attached gadget,
        /// but fires Triggered event with state=false second time and doesn't activate attached gadget.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="conditionIndex"></param>
        /// <param name="state"></param>
        /// <param name="__instance.conditionNames"></param>
        public static void Postfix(GameGadget __instance, int conditionIndex, bool state, List<string> __instance.conditionNames)
        {
            if (!Main.Settings.BugFixButtonActivatorTriggerIssue)
            {
                return;
            }

            if (conditionIndex >= 0 && conditionIndex < __instance.conditionNames.Count)
            {
                var param = __instance.conditionNames[conditionIndex];

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
#endif
