using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using UnityEngine;

namespace SolastaCommunityExpansion.PatchCode.SrdAndHouseRules;

internal static class ArmorClassStacking
{
    
    //replaces call to `RulesetAttributeModifier.BuildAttributeModifier` with custom method that calls base on e and adds extra tags when necessary
    public static void AddCustomTagsToModifierBuilder(List<CodeInstruction> codes)
    {
        var method = new Func<FeatureDefinitionAttributeModifier.AttributeModifierOperation, float, string, string, RulesetAttributeModifier>(RulesetAttributeModifier.BuildAttributeModifier).Method;

        var index = codes.FindIndex(c => c.Calls(method));

        if (index <= 0)
        {
            return;
        }

        var custom = new Func<FeatureDefinitionAttributeModifier.AttributeModifierOperation, float, string, string, FeatureDefinitionAttributeModifier, RulesetAttributeModifier>(CustomBuildAttributeModifier).Method;

        codes[index] = new CodeInstruction(OpCodes.Call, custom); //replace call with custom method
        codes.Insert(index, new CodeInstruction(OpCodes.Ldloc_1)); // load 'feature' as last argument
    }

    private static RulesetAttributeModifier CustomBuildAttributeModifier(
        FeatureDefinitionAttributeModifier.AttributeModifierOperation operationType,
        float modifierValue,
        string tag,
        string sourceAbility,
        FeatureDefinitionAttributeModifier feature)
    {
        var modifier = RulesetAttributeModifier.BuildAttributeModifier(operationType, modifierValue, tag);
        if (feature.HasSubFeatureOfType<ExclusiveArmorClassBonus>())
        {
            modifier.Tags.Add(ExclusiveArmorClassBonus.Tag);
        }

        return modifier;
    }

    public static bool UnstackAC(RulesetAttribute attribute)
    {
        if (attribute.Name != AttributeDefinitions.ArmorClass)
        {
            return true;
        }

        var currentValue = attribute.BaseValue;
        var activeModifiers = attribute.ActiveModifiers;
        var minModValue = int.MinValue;
        var setValue = 10;

        var exclusives = new List<RulesetAttributeModifier>();

        foreach (var modifier in activeModifiers)
        {
            switch (modifier.Operation)
            {
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceAnyway:
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
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddConditionAmount:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddSurroundingEnemies:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfBetter:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceIfWorse:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus:
                case FeatureDefinitionAttributeModifier.AttributeModifierOperation
                    .MultiplyByClassLevelBeforeAdditions:
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

        var realMaxValue = attribute.MaxEditableValue > 0
            ? attribute.MaxEditableValue
            : attribute.MaxValue;

        currentValue = minModValue <= currentValue
            ? Mathf.Clamp(currentValue, attribute.MinValue, realMaxValue)
            : minModValue;

        attribute.currentValue = currentValue;
        attribute.upToDate = true;
        attribute.AttributeRefreshed?.Invoke();

        return false;
    }
}