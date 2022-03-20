using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.AtttributeModifierBonus
{
    [HarmonyPatch(typeof(RulesetAttribute), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetAttribute_Refresh
    {
        internal static void Postfix(RulesetAttribute __instance, ref int ___currentValue)
        {

            var activeModifiers = __instance.ActiveModifiers;

            foreach (var activeModifier in activeModifiers
                .Where(x => x.Operation == (FeatureDefinitionAttributeModifier.AttributeModifierOperation)ExtraAttributeModifierOperation.AdditiveAtEnd))
            {
                ___currentValue += (int)activeModifier.Value;
            }
        }
    }
}
