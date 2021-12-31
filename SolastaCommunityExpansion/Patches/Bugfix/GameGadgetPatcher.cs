using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Helpers;

namespace SolastaCommunityExpansion.Patches.Bugfix
{

    [HarmonyPatch(typeof(GameGadget), "CheckIsEnabled")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameGadget_CheckIsEnabled
    {
        public static bool Prefix(GameGadget __instance, ref bool __result)
        {
            if (!Main.Settings.BugFixGameGadgetCheckIsEnabled)
            {
                return true;
            }

            var result = __result;
            __result = __instance.IsEnabled();
            Main.Log($"CheckIsEnabled: {__instance.UniqueNameId}, {result}, {__result}");
            return false;
        }
    }
}
