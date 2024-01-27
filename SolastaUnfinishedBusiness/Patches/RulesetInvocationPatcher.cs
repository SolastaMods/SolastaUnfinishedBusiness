using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetInvocationPatcher
{
    [HarmonyPatch(typeof(RulesetInvocation), nameof(RulesetInvocation.IsPermanent))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InvocationCastEngaged_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(RulesetInvocation __instance, out bool __result)
        {
            //PATCH: do not count power granting invocations as permanent, so we can activate them
            __result = __instance.InvocationDefinition.IsPermanent();
            return false;
        }
    }

    [HarmonyPatch(typeof(RulesetInvocation), nameof(RulesetInvocation.Consume))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Consume_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetInvocation __instance)
        {
            //PATCH: support for invocations that recharge on short rest (like Fey Teleportation feat)
            if (__instance.invocationDefinition.HasSubFeatureOfType<RechargeInvocationOnShortRest>())
            {
                __instance.used = true;
            }
        }
    }
}
