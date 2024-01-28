using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionPatcher
{
    [HarmonyPatch(typeof(FeatureDefinition), nameof(FeatureDefinition.AllowsDuplicate), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AllowsDuplicate_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(FeatureDefinition __instance, out bool __result)
        {
            __result = __instance.HasSubFeatureOfType<AllowConditionDuplicates>();
        }
    }
}
