using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.Location
{
    // this patch prevents game from receive input if Mod UI is open
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsAnyMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManagerLocation_IsAnyMetamagicOptionAvailable
    {
        internal static void Postfix(ref bool __result)
        {
            var isCtrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

            if (Main.Settings.EnableCtrlClickBypassMetamagicPanel && isCtrlPressed)
            {
                __result = false;
            }
        }
    }
}
