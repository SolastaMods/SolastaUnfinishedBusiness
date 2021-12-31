using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Helpers;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(GameGadget), "CheckIsEnabled")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_CheckIsEnabled
    {
        public static bool Prefix(GameGadget __instance, List<string> ___conditionNames, ref bool __result)
        {
            if (!Main.Settings.BugFixGameGadgetCheckIsEnabled)
            {
                return true;
            }

            var result = __result;

            // If neither Enabled or Param_Enabled exist then return 'true'
            __result = !___conditionNames.Any(c => c == GameGadgetExtensions.Enabled || c == GameGadgetExtensions.ParamEnabled);

#if DEBUG
            if(___conditionNames.Count(c => c == GameGadgetExtensions.Enabled || c == GameGadgetExtensions.ParamEnabled) > 1) 
            {
                Main.Log("GameGadget_CheckIsEnabled: More than one enable parameter");
            }
#endif

            if (!__result)
            {
                // If at least one param exists return the truest
                __result = __instance.CheckConditionName(GameGadgetExtensions.ParamEnabled, true, false)
                        || __instance.CheckConditionName(GameGadgetExtensions.Enabled, true, false);
            }

            Main.Log($"CheckIsEnabled: {__instance.UniqueNameId}, orig={result}, new={__result}");

            return false;
        }
    }
}
