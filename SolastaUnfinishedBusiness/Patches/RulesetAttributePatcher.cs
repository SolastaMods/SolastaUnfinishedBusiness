using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RulesetAttributePatcher
{
    [HarmonyPatch(typeof(RulesetAttribute), nameof(RulesetAttribute.AddModifier))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AddModifier_Patch
    {
        [UsedImplicitly]
        public static void Postfix(RulesetAttribute __instance, RulesetAttributeModifier modifier)
        {
            //PATCH: fixes Critical Threshold SET operations to apply lowest value
            if (__instance.Name == AttributeDefinitions.CriticalThreshold
                && modifier.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set)
            {
                modifier.Tags.TryAdd("SetLowest");
            }

            if (__instance.Name == AttributeDefinitions.ArmorClass
                && modifier.tags.Contains(AttributeDefinitions.TagHealth))
            {
                //TODO: add popup to urge players to share this log
                Main.Error(
                    $"[{__instance.Name}] <{modifier.Operation}> v:{modifier.Value} source:'{modifier.sourceAbility}' tags:[{string.Join(", ", modifier.Tags)}]",
                    true);
            }
        }
    }
}
