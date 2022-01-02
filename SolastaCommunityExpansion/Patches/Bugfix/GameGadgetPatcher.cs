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
}
