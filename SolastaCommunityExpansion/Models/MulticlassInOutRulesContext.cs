using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace SolastaCommunityExpansion.Models;

public static class MulticlassInOutRulesContext
{
    public static void EnumerateHeroAllowedClassDefinitions(RulesetCharacterHero hero,
        List<CharacterClassDefinition> allowedClasses, ref int selectedClass)
    {
        var currentClass = hero.ClassesHistory[hero.ClassesHistory.Count - 1];

        allowedClasses.Clear();

        // only allows to leave a class if it is a supported one with required In/Out attributes
        if (!IsSupported(currentClass)
            || (Main.Settings.EnableMinInOutAttributes && !ApproveMultiClassInOut(hero, currentClass)))
        {
            allowedClasses.Add(currentClass);
        }

        // only allows existing classes with required In/Out attributes
        else if (hero.ClassesAndLevels.Count >= Main.Settings.MaxAllowedClasses)
        {
            allowedClasses.AddRange(hero.ClassesAndLevels.Keys.Where(characterClassDefinition =>
                !Main.Settings.EnableMinInOutAttributes || ApproveMultiClassInOut(hero, characterClassDefinition)));
        }

        // only allows supported classes with required In/Out attributes
        else
        {
            allowedClasses.AddRange(DatabaseRepository.GetDatabase<CharacterClassDefinition>().Where(classDefinition =>
                IsSupported(classDefinition) && (!Main.Settings.EnableMinInOutAttributes ||
                                                 ApproveMultiClassInOut(hero, classDefinition))));
        }

        allowedClasses.Sort((a, b) =>
        {
            hero.ClassesAndLevels.TryGetValue(a, out var aLevels);
            hero.ClassesAndLevels.TryGetValue(b, out var bLevels);

            return aLevels == bLevels ? a.FormatTitle().CompareTo(b.FormatTitle()) : bLevels.CompareTo(aLevels);
        });

        selectedClass = allowedClasses.IndexOf(hero.ClassesHistory[hero.ClassesHistory.Count - 1]);
    }

    private static int MyGetAttribute(RulesetCharacterHero hero, string attributeName)
    {
        var attribute = hero.GetAttribute(attributeName);
        var activeModifiers = attribute.ActiveModifiers;
        var currentValue = attribute.BaseValue + activeModifiers
            .Where(x => x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
            .Sum(activeModifier => Mathf.FloorToInt(activeModifier.Value));

        return Mathf.Clamp(currentValue, int.MinValue,
            attribute.MaxEditableValue > 0 ? attribute.MaxEditableValue : attribute.MaxValue);
    }

    private static Dictionary<string, int> GetItemsAttributeModifiers(RulesetCharacterHero hero)
    {
        var items = new List<RulesetItem>();

        hero.CharacterInventory.EnumerateAllItems(items, false);

        var attributeModifiers =
            AttributeDefinitions.AbilityScoreNames.ToDictionary(attributeName => attributeName, attributeName => 0);

        foreach (var featureDefinitionAttributeModifier in items
                     .SelectMany(x => x.ItemDefinition.StaticProperties
                         .Select(y => y.FeatureDefinition)
                         .OfType<FeatureDefinitionAttributeModifier>()
                         .Where(z => AttributeDefinitions.AbilityScoreNames.Contains(z.ModifiedAttribute) &&
                                     z.ModifierType == FeatureDefinitionAttributeModifier.AttributeModifierOperation
                                         .Additive)))
        {
            attributeModifiers[featureDefinitionAttributeModifier.ModifiedAttribute] +=
                featureDefinitionAttributeModifier.ModifierValue;
        }

        return attributeModifiers;
    }

    [SuppressMessage("Convert switch statement to expression", "IDE0066")]
    private static bool ApproveMultiClassInOut(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
    {
        if (classDefinition.GuiPresentation.Hidden)
        {
            return false;
        }

        var itemsAttributeModifiers = GetItemsAttributeModifiers(hero);
        var strength = MyGetAttribute(hero, AttributeDefinitions.Strength) -
                       itemsAttributeModifiers[AttributeDefinitions.Strength];
        var dexterity = MyGetAttribute(hero, AttributeDefinitions.Dexterity) -
                        itemsAttributeModifiers[AttributeDefinitions.Dexterity];
        var intelligence = MyGetAttribute(hero, AttributeDefinitions.Intelligence) -
                           itemsAttributeModifiers[AttributeDefinitions.Intelligence];
        var wisdom = MyGetAttribute(hero, AttributeDefinitions.Wisdom) -
                     itemsAttributeModifiers[AttributeDefinitions.Wisdom];
        var charisma = MyGetAttribute(hero, AttributeDefinitions.Charisma) -
                       itemsAttributeModifiers[AttributeDefinitions.Charisma];

        switch (classDefinition.Name)
        {
            case RuleDefinitions.BarbarianClass:
                //case IntegrationContext.CLASS_WARDEN:
                return strength >= 13;

            case RuleDefinitions.SorcererClass:
            case RuleDefinitions.WarlockClass:
            case IntegrationContext.CLASS_WARLOCK:
            case IntegrationContext.CLASS_WITCH:
                return charisma >= 13;

            case RuleDefinitions.ClericClass:
            case RuleDefinitions.DruidClass:
                return wisdom >= 13;

            case RuleDefinitions.FighterClass:
                return strength >= 13 || dexterity >= 13;

            case RuleDefinitions.MonkClass:
            case IntegrationContext.CLASS_MONK:
                return wisdom >= 13 && dexterity >= 13;

            case RuleDefinitions.RangerClass:
                return dexterity >= 13 && wisdom >= 13;

            case RuleDefinitions.PaladinClass:
                return strength >= 13 && charisma >= 13;

            case RuleDefinitions.RogueClass:
                return dexterity >= 13;

            case RuleDefinitions.WizardClass:
            case IntegrationContext.CLASS_TINKERER:
                return intelligence >= 13;

            // case IntegrationContext.CLASS_MAGUS:
            //     return intelligence >= 13 && (strength >= 13 || dexterity >= 13);

            default:
                return false;
        }
    }

    private static bool IsSupported(CharacterClassDefinition classDefinition)
    {
        return !classDefinition.GuiPresentation.Hidden;
    }
}
