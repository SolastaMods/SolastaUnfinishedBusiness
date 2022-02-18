using System.Collections.Generic;
using System.Text;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class GameUiContext
    {
        private static readonly float[] fontSizes = new float[] { 17f, 17f, 16f, 15f, 12.5f };

        internal static float GetFontSize(int classesCount)
        {
            return fontSizes[classesCount % 5];
        }

        internal static string GetAllClassesLabel(GuiCharacter character, string separator = "\n")
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
            var builder = new StringBuilder();
            var snapshot = character?.Snapshot;
            var hero = character?.RulesetCharacterHero;

            if (snapshot != null)
            {
                foreach (var className in snapshot.Classes)
                {
                    var classTitle = dbCharacterClassDefinition.GetElement(className).FormatTitle();

                    builder.Append(classTitle);
                    builder.Append(separator);
                }
            }
            else if (hero != null && hero.ClassesAndLevels.Count > 1)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    builder.Append(characterClassDefinition.FormatTitle());
                    builder.Append('/');
                    builder.Append(hero.ClassesAndLevels[characterClassDefinition]);
                    builder.Append(separator);
                }
            }
            else
            {
                return null;
            }

            return builder.ToString().Remove(builder.Length - separator.Length, separator.Length);
        }

        internal static string GetAllClassesHitDiceLabel(GuiCharacter character, out int dieTypeCount)
        {
            Assert.IsNotNull(character, nameof(character));

            var builder = new StringBuilder();
            var hero = character.RulesetCharacterHero;
            var dieTypesCount = new Dictionary<RuleDefinitions.DieType, int>();
            const string separator = " ";

            foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
            {
                if (!dieTypesCount.ContainsKey(characterClassDefinition.HitDice))
                {
                    dieTypesCount.Add(characterClassDefinition.HitDice, 0);
                }

                dieTypesCount[characterClassDefinition.HitDice] += hero.ClassesAndLevels[characterClassDefinition];
            }

            foreach (var dieType in dieTypesCount.Keys)
            {
                builder.Append(dieTypesCount[dieType]);
                builder.Append(Gui.GetDieSymbol(dieType));
#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
                builder.Append(separator);
#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
            }

            dieTypeCount = dieTypesCount.Count;

            return builder.Remove(builder.Length - separator.Length, separator.Length).ToString();
        }

        internal static string GetLevelAndExperienceTooltip(GuiCharacter character)
        {
            var builder = new StringBuilder();
            var hero = character.RulesetCharacterHero;

            if (hero == null)
            {
                return null;
            }

            var characterLevelAttribute = hero.GetAttribute(AttributeDefinitions.CharacterLevel);
            var characterLevel = characterLevelAttribute.CurrentValue;
            var experience = hero.GetAttribute(AttributeDefinitions.Experience).CurrentValue;

            if (characterLevel == characterLevelAttribute.MaxValue)
            {
                builder.Append(Gui.Format("Format/&LevelAndExperienceMaxedFormat", characterLevel.ToString("N0"), experience.ToString("N0")));
            }
            else
            {
                var num = Mathf.Max(0.0f, RuleDefinitions.ExperienceThresholds[characterLevel] - experience);

                builder.Append(Gui.Format("Format/&LevelAndExperienceFormat", characterLevel.ToString("N0"), experience.ToString("N0"), num.ToString("N0"), (characterLevel + 1).ToString("N0")));
            }

            if (hero.ClassesAndLevels.Count > 1) // cannot use InspectionPanelContext here as this method happens before that context is set
            {
                builder.Append('\n');

                for (var i = 0; i < hero.ClassesHistory.Count; i++)
                {
                    var characterClassDefinition = hero.ClassesHistory[i];

                    hero.ClassesAndSubclasses.TryGetValue(characterClassDefinition, out var characterSubclassDefinition);

#pragma warning disable JSON002 // Probable JSON string detected
                    builder.Append('\n')
                        .AppendFormat("{0:00}", i + 1)
                        .Append(" - ")
                        .Append(characterClassDefinition.FormatTitle());
#pragma warning restore JSON002 // Probable JSON string detected

                    // NOTE: don't use characterSubclassDefinition?. which bypasses Unity object lifetime check
                    if (characterSubclassDefinition)
                    {
                        builder
                            .Append(' ')
                            .Append(characterSubclassDefinition.FormatTitle());
                    }
                }
            }

            return builder.ToString();
        }
    }
}
