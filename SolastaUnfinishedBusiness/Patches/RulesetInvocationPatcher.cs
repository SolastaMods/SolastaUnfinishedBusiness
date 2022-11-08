using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class RulesetInvocationPatcher
{
    [HarmonyPatch(typeof(RulesetInvocation), "IsPermanent")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class InvocationCastEngaged_Patch
    {
        public static bool Prefix(RulesetInvocation __instance, out bool __result)
        {
            //PATCH: do not count power granting invocations as permenent, so we can activate them
            var grantedFeature = __instance.InvocationDefinition.GrantedFeature;
            __result = grantedFeature != null && grantedFeature is not FeatureDefinitionPower;
            return false;
        }
    }
}
