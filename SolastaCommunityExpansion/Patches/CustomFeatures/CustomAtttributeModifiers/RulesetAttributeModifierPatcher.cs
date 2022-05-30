using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAtttributeModifiers;

// allows the metamagic feats to work correctly with a Sorcerer
[HarmonyPatch(typeof(RulesetAttributeModifier), "ApplyOnValue")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetAttributeModifier_ApplyOnValue
{
    internal static void Postfix(RulesetAttributeModifier __instance, ref int __result, int originValue)
    {
        if (__instance.Operation ==
            (FeatureDefinitionAttributeModifier.AttributeModifierOperation)ExtraAttributeModifierOperation
                .AdditiveAtEnd)
        {
            __result = Mathf.FloorToInt(__instance.Value + originValue);
        }
    }
}
