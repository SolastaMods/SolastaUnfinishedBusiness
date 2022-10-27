using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public static class InvocationSubPanelPatcher
{
    [HarmonyPatch(typeof(InvocationSubPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Prefix(InvocationSubPanel __instance)
        {
            //PATCH: support for custom invocations and separate sub-panels for them
            //filters only invocations that fit this sub-panel
            CustomInvocationSubPanel.UpdateRelevantInvocations(__instance);
        }
    }
}
