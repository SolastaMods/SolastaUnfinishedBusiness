using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttributeModifiers;

// non stacked AC
[HarmonyPatch(typeof(RulesetAttribute), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetAttribute_Refresh
{
    internal static bool Prefix([NotNull] RulesetAttribute __instance)
    {
        if (__instance.Name != AttributeDefinitions.ArmorClass)
        {
            return true;
        }

        var currentValue = __instance.BaseValue;
        var activeModifiers = __instance.ActiveModifiers;
        var minModValue = int.MinValue;
        var setValue = 10;

        var exclusives = new List<RulesetAttributeModifier>();

        foreach (var modifier in activeModifiers)
        {
            switch (modifier.Operation)
            {
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.Force:
                    minModValue = Mathf.RoundToInt(modifier.Value);
                    break;

                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.Set:
                    setValue = Main.Settings.UseMoreRestrictiveAcStacking
                               || modifier.Tags.Contains(ExclusiveArmorClassBonus.Tag)
                        ? Mathf.RoundToInt(modifier.Value)
                        : 10;
                    currentValue = modifier.ApplyOnValue(currentValue);

                    break;

                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.Multiplicative:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.ConditionAmount:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.SurroundingEnemies:
                default:
                {
                    if (modifier.Tags.Contains(ExclusiveArmorClassBonus.Tag))
                    {
                        exclusives.Add(modifier);
                    }
                    else
                    {
                        currentValue = modifier.ApplyOnValue(currentValue);
                    }

                    break;
                }
            }
        }

        if (!exclusives.Empty())
        {
            var exclusiveAc = exclusives
                .Select(modifier => modifier.ApplyOnValue(currentValue)).Prepend(setValue).Max() - currentValue;

            currentValue = currentValue + (10 - setValue) + exclusiveAc;
        }

        var realMaxValue = __instance.MaxEditableValue > 0
            ? __instance.MaxEditableValue
            : __instance.MaxValue;

        currentValue = minModValue <= currentValue
            ? Mathf.Clamp(currentValue, __instance.MinValue, realMaxValue)
            : minModValue;

        __instance.currentValue = currentValue;
        __instance.upToDate = true;
        __instance.AttributeRefreshed?.Invoke();

        return false;
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "RefreshArmorClassInFeatures")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_RefreshArmorClassInFeatures
{
    [NotNull]
    public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();
        var method = new Func<FeatureDefinitionAttributeModifier.AttributeModifierOperation,
            float, string, RulesetAttributeModifier>(RulesetAttributeModifier.BuildAttributeModifier).Method;

        var index = codes.FindIndex(c => c.Calls(method));

        if (index <= 0)
        {
            return codes.AsEnumerable();
        }

        var custom = new Func<FeatureDefinitionAttributeModifier.AttributeModifierOperation,
            float, string, FeatureDefinitionAttributeModifier, RulesetAttributeModifier>(CustomBuild).Method;

        codes[index] = new CodeInstruction(OpCodes.Call, custom);
        codes.Insert(index, new CodeInstruction(OpCodes.Ldloc_2));

        return codes.AsEnumerable();
    }

    private static RulesetAttributeModifier CustomBuild(
        FeatureDefinitionAttributeModifier.AttributeModifierOperation operationType,
        float modifierValue,
        string tag,
        FeatureDefinitionAttributeModifier feature)
    {
        var modifier = RulesetAttributeModifier.BuildAttributeModifier(operationType, modifierValue, tag);
        if (feature.HasSubFeatureOfType<ExclusiveArmorClassBonus>())
        {
            modifier.Tags.Add(ExclusiveArmorClassBonus.Tag);
        }

        return modifier;
    }
}
