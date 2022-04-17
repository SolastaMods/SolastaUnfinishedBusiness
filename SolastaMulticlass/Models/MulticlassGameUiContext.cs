using System.Collections.Generic;
using System.Text;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaMulticlass.Models
{
    internal static class MulticlassGameUiContext
    {
        private static Color LightGreenSlot = new(0f, 1f, 0f, 1f);
        private static Color DarkGreenSlot = new(0f, 0.5f, 0f, 1f);
        private static Color WhiteSlot = new(1f, 1f, 1f, 1f);
        private static readonly float[] fontSizes = new float[] { 17f, 17f, 16f, 15f, 12.5f };

        internal static float GetFontSize(int classesCount)
        {
            return fontSizes[classesCount % 5];
        }

        internal static void PaintSlotsLightOrDarkGreen(
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

                usedSpellsSlots.TryGetValue(SharedSpellsContext.PACT_MAGIC_SLOT_TAB_INDEX, out shortRestSlotsUsedCount);
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
                    else
                    {
                        component.Used.gameObject.SetActive(index >= longRestSlotsCount + shortRestSlotsRemainingCount);
                        component.Available.gameObject.SetActive(index < longRestSlotsCount + shortRestSlotsRemainingCount);
                    }
                }

                if (index >= longRestSlotsCount && slotLevel <= warlockSpellLevel)
                {
                    component.Available.GetComponent<Image>().color = LightGreenSlot;
                }
                else
                {
                    component.Available.GetComponent<Image>().color = DarkGreenSlot;
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

        internal static void PaintSlotsWhite(RectTransform rectTransform)
        {
            for (var index = 0; index < rectTransform.childCount; ++index)
            {
                var child = rectTransform.GetChild(index);
                var component = child.GetComponent<SlotStatus>();

                component.Available.GetComponent<Image>().color = WhiteSlot;
            }
        }

        internal static void RebuildSlotsTable(
            GameObject ___levelButtonPrefab,
            RectTransform ___levelButtonsTable,
            RectTransform ___spellsByLevelTable,
            SpellLevelButton.LevelSelectedHandler levelSelected,
            int accountForCantrips,
            int classSpellLevel,
            int slotLevel,
            bool useClassSpellLevelOnButtons = true)
        {
            var level = useClassSpellLevelOnButtons ? classSpellLevel : slotLevel;

            while (___levelButtonsTable.childCount < level + accountForCantrips)
            {
                Gui.GetPrefabFromPool(___levelButtonPrefab, ___levelButtonsTable);

                var index = ___levelButtonsTable.childCount - 1;
                var child = ___levelButtonsTable.GetChild(index);

                child.GetComponent<SpellLevelButton>().Bind(index, new SpellLevelButton.LevelSelectedHandler(levelSelected));
            }

            while (___levelButtonsTable.childCount > level + accountForCantrips)
            {
                Gui.ReleaseInstanceToPool(___levelButtonsTable.GetChild(___levelButtonsTable.childCount - 1).gameObject);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(___levelButtonsTable);

            // patches the panel to display higher level spell slots from shared slots table but hide the spell panels if class level not there yet
            for (var i = 0; i < ___spellsByLevelTable.childCount; i++)
            {
                var spellsByLevel = ___spellsByLevelTable.GetChild(i);

                for (var j = 0; j < spellsByLevel.childCount; j++)
                {
                    var transform = spellsByLevel.GetChild(j);

                    if (transform.TryGetComponent(typeof(SlotStatusTable), out var _))
                    {
                        transform.gameObject.SetActive(i < slotLevel + accountForCantrips); // table header (with slots)
                    }
                    else
                    {
                        transform.gameObject.SetActive(i < classSpellLevel + accountForCantrips); // table content
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(___spellsByLevelTable);
        }

        internal static string GetAllClassesLabel(GuiCharacter character, char separator = '\n')
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

                    builder
                        .Append(classTitle)
                        .Append(separator);
                }
            }
            else if (hero != null && hero.ClassesAndLevels.Count > 1)
            {
                foreach (var characterClassDefinition in hero.ClassesAndLevels.Keys)
                {
                    builder
                        .Append(characterClassDefinition.FormatTitle())
                        .Append('/')
                        .Append(hero.ClassesAndLevels[characterClassDefinition])
                        .Append(separator);
                }
            }
            else
            {
                return null;
            }

            return builder.ToString().Remove(builder.Length - 1, 1);
        }

        internal static string GetAllClassesHitDiceLabel(GuiCharacter character, out int dieTypeCount)
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
