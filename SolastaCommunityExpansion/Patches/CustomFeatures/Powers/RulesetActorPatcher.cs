using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers;

// enforce sorcery points and healing pool to correctly calculate class level
[HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_RefreshAttributes
{
    private static readonly Dictionary<string, CharacterClassDefinition> Rules = new()
    {
        {AttributeDefinitions.HealingPool, Paladin}, {AttributeDefinitions.SorceryPoints, Sorcerer}
    };

    private static int GetClassOrCharacterLevel(int characterLevel, RulesetCharacter rulesetCharacter,
        [NotNull] string attribute)
    {
        if (!Rules.TryGetValue(attribute, out var characterClass))
        {
            return characterLevel;
        }

        var hero = rulesetCharacter as RulesetCharacterHero ??
                   rulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

        if (hero != null && hero.ClassesAndLevels.TryGetValue(characterClass, out var classLevel))
        {
            return classLevel;
        }

        return characterLevel;
    }

    internal static bool Prefix(RulesetActor __instance)
    {
        if (__instance is not RulesetCharacter rulesetCharacter)
        {
            return true;
        }

        var characterLevel = 1;
        var characterLevelAttribute = rulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel, true);

        if (characterLevelAttribute != null)
        {
            characterLevelAttribute.Refresh();
            characterLevel = characterLevelAttribute.CurrentValue;
        }

        foreach (var attribute in __instance.Attributes)
        {
            foreach (var activeModifier in attribute.Value.ActiveModifiers)
            {
                activeModifier.Value = activeModifier.Operation switch
                {
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel =>
                        characterLevel,
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel =>
                        GetClassOrCharacterLevel(characterLevel, rulesetCharacter, attribute.Key),
                    _ => activeModifier.Value
                };
            }

            attribute.Value.Refresh();
        }

        return false;
    }
}
