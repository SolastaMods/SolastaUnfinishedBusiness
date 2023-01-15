using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserLocationViewPanelPatcher
{
    private static bool IsCtrlPressed => Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl);

    [HarmonyPatch(typeof(UserLocationViewPanel), "PropOverlap", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PropOverlap_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result)
        {
            //PATCH: Bypasses prop overlap check if CTRL is pressed (DMP)
            __result = __result && !(Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere && IsCtrlPressed);
        }
    }

    [HarmonyPatch(typeof(UserLocationViewPanel), "GadgetOverlap", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GadgetOverlap_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result)
        {
            //PATCH: Bypasses gadget overlap check if CTRL is pressed (DMP)
            __result = __result && !(Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere && IsCtrlPressed);
        }
    }

    [HarmonyPatch(typeof(UserLocationViewPanel), "PropInvalidPlacement", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PropInvalidPlacement_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result)
        {
            //PATCH: Bypasses prop invalid check if CTRL is pressed (DMP)
            __result = __result && !(Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere && IsCtrlPressed);
        }
    }

    [HarmonyPatch(typeof(UserLocationViewPanel), "GadgetInvalidPlacement", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GadgetInvalidPlacement_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result)
        {
            //PATCH: Bypasses gadget invalid check if CTRL is pressed (DMP)
            __result = __result && !(Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere && IsCtrlPressed);
        }
    }
}
