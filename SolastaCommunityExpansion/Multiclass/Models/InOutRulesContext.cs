using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class InOutRulesContext
    {
        internal static void EnumerateHeroAllowedClassDefinitions(RulesetCharacterHero hero, List<CharacterClassDefinition> allowedClasses, ref int selectedClass)
        {
            var currentClass = hero.ClassesHistory[hero.ClassesHistory.Count - 1];

            allowedClasses.Clear();

            // only allows to leave a class if it is a supported one with required In/Out attributes
            if (!IsSupported(currentClass) || (Main.Settings.EnableMinInOutAttributes && !ApproveMultiClassInOut(hero, currentClass)))
            {
                allowedClasses.Add(currentClass);
            }

            // only allows existing classes with required In/Out attributes
            else if (hero.ClassesAndLevels.Count >= Main.Settings.MaxAllowedClasses)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    if (!Main.Settings.EnableMinInOutAttributes || ApproveMultiClassInOut(hero, characterClassDefinition))
                    {
                        allowedClasses.Add(characterClassDefinition);
                    }
                }
            }

            // only allows supported classes with required In/Out attributes
            else
            {
                foreach (var classDefinition in DatabaseRepository.GetDatabase<CharacterClassDefinition>())
                {
                    if (IsSupported(classDefinition) && (!Main.Settings.EnableMinInOutAttributes || ApproveMultiClassInOut(hero, classDefinition)))
                    {
                        allowedClasses.Add(classDefinition);
                    }
                }
            }

            allowedClasses.Sort((a, b) => a.FormatTitle().CompareTo(b.FormatTitle()));
            selectedClass = allowedClasses.IndexOf(hero.ClassesHistory[hero.ClassesHistory.Count - 1]);
        }

        [SuppressMessage("Convert switch statement to expression", "IDE0066")]
        internal static bool ApproveMultiClassInOut(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            var strength = hero.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
            var dexterity = hero.GetAttribute(AttributeDefinitions.Dexterity).CurrentValue;
            var intelligence = hero.GetAttribute(AttributeDefinitions.Intelligence).CurrentValue;
            var wisdom = hero.GetAttribute(AttributeDefinitions.Wisdom).CurrentValue;
            var charisma = hero.GetAttribute(AttributeDefinitions.Charisma).CurrentValue;

            if (classDefinition.GuiPresentation.Hidden)
            {
                return false;
            }

            switch (classDefinition.Name)
            {
                case RuleDefinitions.BarbarianClass:
                case IntegrationContext.CLASS_WARDEN:
                    return strength >= 13;

                case IntegrationContext.CLASS_BARD:
                case RuleDefinitions.SorcererClass:
                case IntegrationContext.CLASS_WARLOCK:
                case IntegrationContext.CLASS_WITCH:
                    return charisma >= 13;

                case RuleDefinitions.ClericClass:
                case RuleDefinitions.DruidClass:
                    return wisdom >= 13;

                case RuleDefinitions.FighterClass:
                    return strength >= 13 || dexterity >= 13;

                case IntegrationContext.CLASS_MONK:
                case RuleDefinitions.RangerClass:
                    return dexterity >= 13 && wisdom >= 13;

                case RuleDefinitions.PaladinClass:
                    return strength >= 13 && charisma >= 13;

                case RuleDefinitions.RogueClass:
                    return dexterity >= 13;

                case RuleDefinitions.WizardClass:
                case IntegrationContext.CLASS_TINKERER:
                case IntegrationContext.CLASS_ALCHEMIST:
                    return intelligence >= 13;

                default:
                    return false;
            }
        }

        [SuppressMessage("Convert switch statement to expression", "IDE0066")]
        internal static bool IsSupported(CharacterClassDefinition classDefinition)
        {
            if (classDefinition.GuiPresentation.Hidden)
            {
                return false;
            }

            switch (classDefinition.Name)
            {
                case RuleDefinitions.BarbarianClass:
                case RuleDefinitions.ClericClass:
                case RuleDefinitions.DruidClass:
                case RuleDefinitions.FighterClass:
                case RuleDefinitions.PaladinClass:
                case RuleDefinitions.RangerClass:
                case RuleDefinitions.RogueClass:
                case RuleDefinitions.SorcererClass:
                case RuleDefinitions.WizardClass:
                case IntegrationContext.CLASS_TINKERER:
                case IntegrationContext.CLASS_WARDEN:
                case IntegrationContext.CLASS_WITCH:
                case IntegrationContext.CLASS_ALCHEMIST:
                case IntegrationContext.CLASS_BARD:
                case IntegrationContext.CLASS_MONK:
                case IntegrationContext.CLASS_WARLOCK:
                    return true;

                default:
                    return false;
            }
        }
    }
}
