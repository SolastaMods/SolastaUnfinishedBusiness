using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetItemPropertyPatcher
{
    [HarmonyPatch(typeof(RulesetItemProperty), nameof(RulesetItemProperty.UnicityTag), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnicityTag_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetItemProperty __instance, ref string __result)
        {
            var custom = __instance.featureDefinition.GetFirstSubFeatureOfType<ICustomUnicityTag>();
            if (custom != null)
            {
                __result = custom.UnicityTag;
            }
        }
    }
}
