using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class FeatureDefinitionAttributeModifierPatcher
{
    [HarmonyPatch(typeof(FeatureDefinitionAttributeModifier), "ApplyModifiers")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ApplyModifiers_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: support for exclusivity tags in AC modifiers  
            //used to prevent various extra defence feats (like arcane defense or wise defense) from stacking
            //replaces call to `RulesetAttributeModifier.BuildAttributeModifier` with custom method that calls base on e and adds extra tags when necessary
            return ArmorClassStacking.AddCustomTagsToModifierBuilderInFeature(instructions);
        }
    }
}
