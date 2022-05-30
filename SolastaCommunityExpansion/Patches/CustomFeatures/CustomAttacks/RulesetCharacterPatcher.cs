using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttacks;

[HarmonyPatch(typeof(RulesetCharacter), "GetMovementModifiers")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_GetMovementModifiers
{
    internal static void Postfix(RulesetCharacter __instance, ref List<FeatureDefinition> __result)
    {
        var features = __instance.GetSubFeaturesByType<IConditionalMovementModifier>();
        foreach (var feature in features)
        {
            feature.AddModifiers(__instance, __result);
        }
    }
}
