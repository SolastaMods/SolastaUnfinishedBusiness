using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ConditionDefinitionPatcher
{
    //PATCH: mainly supports lighting and obscurement rules use case
    [HarmonyPatch(typeof(ConditionDefinition), nameof(ConditionDefinition.IsSubtypeOf))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsSubtypeOf_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ConditionDefinition __instance, ref bool __result, string parentConditionName)
        {
            if (__result)
            {
                return;
            }

            var modifier = __instance.GetFirstSubFeatureOfType<IForceConditionParent>();

            if (modifier != null && modifier.Parent(__instance) == parentConditionName)
            {
                __result = true;
            }
        }
    }

    //PATCH: mainly supports lighting and obscurement rules use case
    [HarmonyPatch(typeof(ConditionDefinition), nameof(ConditionDefinition.HasParentCondition))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HasParentCondition_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ConditionDefinition __instance, ref bool __result)
        {
            if (__result)
            {
                return;
            }

            var modifier = __instance.HasSubFeatureOfType<IForceConditionParent>();

            if (modifier)
            {
                __result = true;
            }
        }
    }
}
