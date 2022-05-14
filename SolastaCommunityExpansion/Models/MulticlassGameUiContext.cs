using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Models
{
    public static class MulticlassGameUiContext
    {
        private static Color LightGreenSlot = new(0f, 1f, 0f, 1f);
        private static Color WhiteSlot = new(1f, 1f, 1f, 1f);
        private static readonly float[] fontSizes = new float[] { 17f, 17f, 16f, 14.75f, 13.5f, 13.5f, 13.5f };

        public static float GetFontSize(int classesCount)
        {
            return fontSizes[classesCount % (MulticlassContext.MAX_CLASSES + 1)];
        }

        public static void PaintPactSlots(
            RulesetCharacterHero heroWithSpellRepertoire,
            int totalSlotsCount,
            int totalSlotsRemainingCount,
            int slotLevel,
            RectTransform rectTransform,
            bool hasTooltip = false)
        {
            var shortRestSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
            var longRestSlotsCount = totalSlotsCount - shortRestSlotsCount;

            var shortRestSlotsUsedCount = 0;

            var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            if (warlockSpellRepertoire != null)
            {
                var usedSpellsSlots = warlockSpellRepertoire.GetField<RulesetSpellRepertoire, Dictionary<int, int>>("usedSpellsSlots");

                usedSpellsSlots.TryGetValue(SharedSpellsContext.MC_PACT_MAGIC_SLOT_TAB_INDEX, out shortRestSlotsUsedCount);
            }

            var shortRestSlotsRemainingCount = shortRestSlotsCount - shortRestSlotsUsedCount;
            var longRestSlotsRemainingCount = totalSlotsRemainingCount - shortRestSlotsRemainingCount;

            var longRestSlotsUsedCount = longRestSlotsCount - longRestSlotsRemainingCount;

            for (var index = 0; index < rectTransform.childCount; ++index)
            {
                var component = rectTransform.GetChild(index).GetComponent<SlotStatus>();

                if (slotLevel <= warlockSpellLevel)
                {
                    if (index < longRestSlotsCount)
                    {
                        component.Used.gameObject.SetActive(index >= totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                        component.Available.gameObject.SetActive(index < totalSlotsRemainingCount - shortRestSlotsRemainingCount);
                    }
                    else if (slotLevel == warlockSpellLevel)
                    {
                        component.Used.gameObject.SetActive(index >= longRestSlotsCount + shortRestSlotsRemainingCount);
                        component.Available.gameObject.SetActive(index < longRestSlotsCount + shortRestSlotsRemainingCount);
                    }
                    else
                    {
                        component.Used.gameObject.SetActive(false);
                        component.Available.gameObject.SetActive(false);
                    }
                }

                if (index >= longRestSlotsCount && slotLevel <= warlockSpellLevel)
                {
                    component.Available.GetComponent<Image>().color = LightGreenSlot;
                }
                else
                {
                    component.Available.GetComponent<Image>().color = WhiteSlot;
                }
            }

            if (!hasTooltip)
            {
                return;
            }

            string str;

            if (totalSlotsRemainingCount == 0)
            {
                str = "Screen/&SpellSlotsUsedAllDescription";
            }
            else if (totalSlotsRemainingCount == totalSlotsCount)
            {
                str = "Screen/&SpellSlotsUsedNoneDescription";
            }
            else if (shortRestSlotsRemainingCount == shortRestSlotsCount)
            {
                str = Gui.Format("Screen/&SpellSlotsUsedLongDescription", longRestSlotsUsedCount.ToString());
            }
            else if (longRestSlotsRemainingCount == longRestSlotsCount)
            {
                str = Gui.Format("Screen/&SpellSlotsUsedShortDescription", shortRestSlotsUsedCount.ToString());
            }
            else
            {
                str = Gui.Format("Screen/&SpellSlotsUsedShortLongDescription", shortRestSlotsUsedCount.ToString(), longRestSlotsUsedCount.ToString());
            }

            rectTransform.GetComponent<GuiTooltip>().Content = str;
        }

        public static void PaintSlotsWhite(RectTransform rectTransform)
        {
            for (var index = 0; index < rectTransform.childCount; ++index)
            {
                var child = rectTransform.GetChild(index);
                var component = child.GetComponent<SlotStatus>();

                component.Available.GetComponent<Image>().color = WhiteSlot;
            }
        }

        /**Adds available slot level options to optionsAvailability and returns index of pre-picked option, or -1*/
        public static int AddAvailableSubLevels(Dictionary<int, bool> optionsAvailability, RulesetCharacterHero hero,
            RulesetSpellRepertoire spellRepertoire, int minSpellLebvel = 1)
        {
            var selectedSlot = -1;

            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
            var shortRestSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var isMulticaster = SharedSpellsContext.IsMulticaster(hero);
            var hasPactMagic = warlockSpellLevel > 0;

            var maxRepertoireLevel = spellRepertoire.MaxSpellLevelOfSpellCastingLevel;
            var maxSpellLevel = Math.Max(maxRepertoireLevel, warlockSpellLevel);
            var selected = false;

            for (int level = minSpellLebvel; level <= maxSpellLevel; ++level)
            {
                spellRepertoire.GetSlotsNumber(level, out var remaining, out var max);
                if (hasPactMagic && level != warlockSpellLevel)
                {
                    max -= shortRestSlotsCount;
                }

                if (max > 0 && (
                        level <= maxRepertoireLevel
                        && (isMulticaster || !hasPactMagic)
                        || level == warlockSpellLevel
                    ))
                {
                    optionsAvailability.Add(level, remaining > 0);
                    if (!selected && remaining > 0)
                    {
                        selected = true;
                        selectedSlot = level - minSpellLebvel;
                    }
                }
            }

            return selectedSlot;
        }

        public static string GetAllClassesLabel(GuiCharacter character, char separator)
        {
            var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
            var builder = new StringBuilder();
            var snapshot = character?.Snapshot;
            var hero = character?.RulesetCharacterHero;

            if (snapshot != null && snapshot.Classes.Length > 1)
            {
                foreach (var className in snapshot.Classes)
                {
                    var classTitle = dbCharacterClassDefinition.GetElement(className).FormatTitle();

                    builder
                        .Append(classTitle)
                        .Append(separator);
                }
            }
            else if (hero != null && hero.ClassesAndLevels.Count > 1)
            {
                var i = 0;
                var classesCount = hero.ClassesAndLevels.Count;
                var newLine = separator == '\n' || classesCount <= 4 ? 2 : 3;
                var sortedClasses = from entry in hero.ClassesAndLevels
                                    orderby entry.Value descending, entry.Key.FormatTitle() ascending
                                    select entry;

                foreach (var kvp in sortedClasses)
                {
                    builder
                        .Append(kvp.Key.FormatTitle())
                        .Append('/')
                        .Append(kvp.Value);

                    if (classesCount <= 3)
                    {
                        builder
                            .Append(separator);
                    }
                    else
                    {
                        builder
                            .Append(' ');

                        i++;

                        if (i % newLine == 0)
                        {
                            builder
                                .Append('\n');
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return builder.ToString().Remove(builder.Length - 1, 1);
        }

        public static string GetAllClassesHitDiceLabel(GuiCharacter character, out int dieTypeCount)
        {
            Assert.IsNotNull(character, nameof(character));

            var builder = new StringBuilder();
            var hero = character.RulesetCharacterHero;
            var dieTypesCount = new Dictionary<RuleDefinitions.DieType, int>();
            const char separator = ' ';

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
                builder
                    .Append(dieTypesCount[dieType])
                    .Append(Gui.GetDieSymbol(dieType))
                    .Append(separator);
            }

            dieTypeCount = dieTypesCount.Count;

            return builder.Remove(builder.Length - 1, 1).ToString();
        }

        public static string GetLevelAndExperienceTooltip(GuiCharacter character)
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

                    builder
                        .AppendFormat("\n{0:00} - ", i + 1)
                        .Append(characterClassDefinition.FormatTitle());

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
