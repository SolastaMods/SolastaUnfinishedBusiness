using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

public static class InvocationActivationBoxPatcher
{
    [HarmonyPatch(typeof(InvocationActivationBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(InvocationActivationBox __instance)
        {
            //PATCH: make sure hidden invocations are indeed hidden and not interactable
            if (__instance.Invocation.invocationDefinition.HasSubFeatureOfType<Hidden>())
            {
                __instance.gameObject.SetActive(false);
                __instance.button.interactable = false;
            }
            else
            {
                __instance.gameObject.SetActive(true);
            }
        }
    }
}
