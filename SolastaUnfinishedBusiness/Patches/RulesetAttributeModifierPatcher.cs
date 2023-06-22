using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetAttributeModifierPatcher
{
    [HarmonyPatch(typeof(RulesetAttributeModifier), nameof(RulesetAttributeModifier.CompareTo))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CompareTo_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetAttributeModifier __instance, ref int __result,
            RulesetAttributeModifier other)
        {
            //PATCH: fixes Critical Threshold SET operations to apply lowest value
            if (other == null)
            {
                return;
            }

            if (__instance.Operation != FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set)
            {
                return;
            }

            var priorityCache = RulesetAttributeModifier.AttributeModifierOperationPriorityCache;
            if (priorityCache[(int)__instance.Operation] != priorityCache[(int)other.Operation])
            {
                return;
            }

            if (__instance.Tags.Contains("SetLowest"))
            {
                __result = -__instance.Value.CompareTo(other.Value);
            }
        }
    }
}
