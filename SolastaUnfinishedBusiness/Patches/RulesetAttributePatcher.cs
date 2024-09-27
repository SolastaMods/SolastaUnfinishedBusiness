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
            switch (__instance.Name)
            {
                //PATCH: fixes Critical Threshold SET operations to apply the lowest value
                case AttributeDefinitions.CriticalThreshold
                    when modifier.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set:

                    modifier.Tags.TryAdd("SetLowest");
                    break;

                //PATCH: fix a RESPEC issue until we cannot down to the source cause
                case AttributeDefinitions.ArmorClass
                    when modifier.tags.Contains(AttributeDefinitions.TagHealth):
                    Main.Error(
                        $"[{__instance.Name}] <{modifier.Operation}> v:{modifier.Value} source:'{modifier.sourceAbility}' tags:[{string.Join(", ", modifier.Tags)}]",
                        true);
                    break;
            }
        }
    }
}
