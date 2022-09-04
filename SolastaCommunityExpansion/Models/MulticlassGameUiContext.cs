using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Models;

public static class MulticlassGameUiContext
{
     private static readonly Color LightGreenSlot = new(0f, 1f, 0f, 1f);
     private static readonly Color WhiteSlot = new(1f, 1f, 1f, 1f);
     private static readonly float[] FontSizes = {17f, 17f, 16f, 14.75f, 13.5f, 13.5f, 13.5f};

     public static float GetFontSize(int classesCount)
     {
         return FontSizes[classesCount % (MulticlassContext.MaxClasses + 1)];
     }

     public static void PaintPactSlots(
         [NotNull] RulesetCharacterHero heroWithSpellRepertoire,
         int totalSlotsCount,
         int totalSlotsRemainingCount,
         int slotLevel,
         [NotNull] RectTransform rectTransform,
         bool hasTooltip = false)
     {
         var warlockSpellRepertoire = SharedSpellsContext.GetWarlockSpellRepertoire(heroWithSpellRepertoire);
         var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

         var pactSlotsCount = 0;
         var pactSlotsRemainingCount = 0;
         var pactSlotsUsedCount = 0;

         if (warlockSpellRepertoire != null)
         {
             pactSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(heroWithSpellRepertoire);
             pactSlotsUsedCount = SharedSpellsContext.GetWarlockUsedSlots(heroWithSpellRepertoire);
             pactSlotsRemainingCount = pactSlotsCount - pactSlotsUsedCount;
         }

         var spellSlotsCount = totalSlotsCount - pactSlotsCount;
         var spellSlotsRemainingCount = totalSlotsRemainingCount - pactSlotsRemainingCount;
         var spellSlotsUsedCount = spellSlotsCount - spellSlotsRemainingCount;

         for (var index = 0; index < rectTransform.childCount; ++index)
         {
             var component = rectTransform.GetChild(index).GetComponent<SlotStatus>();

             if (slotLevel <= warlockSpellLevel)
             {
                 if (index < spellSlotsCount)
                 {
                     component.Used.gameObject.SetActive(index >=
                                                         totalSlotsRemainingCount - pactSlotsRemainingCount);
                     component.Available.gameObject.SetActive(index < totalSlotsRemainingCount -
                         pactSlotsRemainingCount);
                 }
                 else if (slotLevel == warlockSpellLevel)
                 {
                     component.Used.gameObject.SetActive(index >= spellSlotsCount + pactSlotsRemainingCount);
                     component.Available.gameObject.SetActive(index < spellSlotsCount + pactSlotsRemainingCount);
                 }
                 else
                 {
                     component.Used.gameObject.SetActive(false);
                     component.Available.gameObject.SetActive(false);
                 }
             }

             if (index >= spellSlotsCount && slotLevel <= warlockSpellLevel)
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
         else if (pactSlotsRemainingCount == pactSlotsCount)
         {
             str = Gui.Format("Screen/&SpellSlotsUsedLongDescription", spellSlotsUsedCount.ToString());
         }
         else if (spellSlotsRemainingCount == spellSlotsCount)
         {
             str = Gui.Format("Screen/&SpellSlotsUsedShortDescription", pactSlotsUsedCount.ToString());
         }
         else
         {
             str = Gui.Format("Screen/&SpellSlotsUsedShortLongDescription", pactSlotsUsedCount.ToString(),
                 spellSlotsUsedCount.ToString());
         }

         rectTransform.GetComponent<GuiTooltip>().Content = str;
     }

     public static void PaintSlotsWhite([NotNull] RectTransform rectTransform)
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
         [NotNull] RulesetSpellRepertoire spellRepertoire, int minSpellLevel = 1, int maxSpellLevel = 0)
     {
         var selectedSlot = -1;

         var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
         var pactMagicSlotsCount = SharedSpellsContext.GetWarlockMaxSlots(hero);
         var isMulticaster = SharedSpellsContext.IsMulticaster(hero);
         var hasPactMagic = warlockSpellLevel > 0;

         var maxRepertoireLevel = spellRepertoire.MaxSpellLevelOfSpellCastingLevel;
         var selected = false;

         if (maxSpellLevel == 0)
         {
             maxSpellLevel = Math.Max(maxRepertoireLevel, warlockSpellLevel);
         }

         for (var level = minSpellLevel; level <= maxSpellLevel; ++level)
         {
             spellRepertoire.GetSlotsNumber(level, out var remaining, out var max);

             if (hasPactMagic && level != warlockSpellLevel)
             {
                 max -= pactMagicSlotsCount;
             }

             if (max <= 0 || ((level > maxRepertoireLevel || (!isMulticaster && hasPactMagic)) &&
                              level != warlockSpellLevel))
             {
                 continue;
             }

             optionsAvailability.Add(level, remaining > 0);

             if (selected || remaining <= 0)
             {
                 continue;
             }

             selected = true;
             selectedSlot = level - minSpellLevel;
         }

         return selectedSlot;
     }

     [CanBeNull]
     public static string GetAllClassesLabel([CanBeNull] GuiCharacter character, char separator)
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
                 orderby entry.Value descending, entry.Key.FormatTitle()
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

     [NotNull]
     public static string GetAllClassesHitDiceLabel([NotNull] GuiCharacter character, out int dieTypeCount)
     {
         Assert.IsNotNull(character, nameof(character));

         var builder = new StringBuilder();
         var hero = character.RulesetCharacterHero;
         var dieTypesCount = new Dictionary<RuleDefinitions.DieType, int>();
         const char SEPARATOR = ' ';

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
                 .Append(SEPARATOR);
         }

         dieTypeCount = dieTypesCount.Count;

         return builder.Remove(builder.Length - 1, 1).ToString();
     }

    public static void SpellsByLevelGroupBindLearning(
        [NotNull] SpellsByLevelGroup group,
        [NotNull] ICharacterBuildingService characterBuildingService,
        [NotNull] SpellListDefinition spellListDefinition,
        List<string> restrictedSchools,
        bool ritualOnly,
        int spellLevel,
        SpellBox.SpellBoxChangedHandler spellBoxChanged,
        List<SpellDefinition> knownSpells,
        List<SpellDefinition> unlearnedSpells,
        [NotNull] string spellTag,
        bool canAcquireSpells,
        bool unlearn,
        RectTransform tooltipAnchor,
        TooltipDefinitions.AnchorMode anchorMode,
        CharacterStageSpellSelectionPanel panel)
    {
        var localHeroCharacter = characterBuildingService.CurrentLocalHeroCharacter;
        var heroBuildingData = localHeroCharacter.GetHeroBuildingData();
        var pointPool = GetCurrentPool(panel, heroBuildingData);

        group.extraSpellsMap.Clear();
        group.spellsTable.gameObject.SetActive(true);
        group.slotStatusTable.gameObject.SetActive(true);
        group.SpellLevel = spellLevel;

        var allSpells = spellListDefinition.SpellsByLevel[spellListDefinition.HasCantrips ? spellLevel : spellLevel - 1]
            .Spells
            .Where(spell => restrictedSchools.Count == 0 || restrictedSchools.Contains(spell.SchoolOfMagic))
            .Where(spell => !ritualOnly || spell.Ritual)
            .ToList();

        allSpells.AddRange(characterBuildingService
            .EnumerateKnownAndAcquiredSpells(heroBuildingData, string.Empty)
            .Where(s => s.SpellLevel == spellLevel && !allSpells.Contains(s))
        );

        if (!spellTag.Contains(AttributeDefinitions.TagRace)) // this is a patch over original TA code
        {
            localHeroCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(group.features);

            foreach (var spell in from FeatureDefinitionMagicAffinity feature in @group.features
                     where feature.ExtendedSpellList != null
                     from spell in feature.ExtendedSpellList
                         .SpellsByLevel[spellListDefinition.HasCantrips ? spellLevel : spellLevel - 1].Spells
                     where !allSpells.Contains(spell) &&
                           (restrictedSchools.Count == 0 || restrictedSchools.Contains(spell.SchoolOfMagic))
                     select spell)
            {
                allSpells.Add(spell);
            }
        }

        var autoPrepareTag = string.Empty;

        group.autoPreparedSpells.Clear();

        if (group.SpellLevel > 0)
        {
            localHeroCharacter
                .EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(group.features);

            foreach (var feature in group.features.OfType<FeatureDefinitionAutoPreparedSpells>())
            {
                autoPrepareTag = feature.AutoPreparedTag;

                foreach (var spells in from preparedSpellsGroup in feature.AutoPreparedSpellsGroups
                         from spells in preparedSpellsGroup.SpellsList
                         where spells.SpellLevel == @group.SpellLevel
                         select spells)
                {
                    group.autoPreparedSpells.Add(spells);
                }

                foreach (var autoPreparedSpell in group.autoPreparedSpells.Where(autoPreparedSpell =>
                             !allSpells.Contains(autoPreparedSpell)))
                {
                    allSpells.Add(autoPreparedSpell);
                }
            }
        }

        var service = ServiceRepository.GetService<IGamingPlatformService>();

        for (var index = allSpells.Count - 1; index >= 0; --index)
        {
            if (!service.IsContentPackAvailable(allSpells[index].ContentPack))
            {
                allSpells.RemoveAt(index);
            }
        }

        if (!spellTag.Contains(AttributeDefinitions.TagRace)) // this is a patch over original TA code
        {
            CollectAllAutoPreparedSpells(group, localHeroCharacter, allSpells, group.autoPreparedSpells);

            FilterMulticlassBleeding(group, localHeroCharacter, allSpells, group.autoPreparedSpells, pointPool,
                group.extraSpellsMap);
        }

        group.CommonBind(null, unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning, spellBoxChanged,
            allSpells, null, null, group.autoPreparedSpells, unlearnedSpells, autoPrepareTag,
            group.extraSpellsMap, tooltipAnchor, anchorMode);

        if (unlearn)
        {
            group.RefreshUnlearning(characterBuildingService, knownSpells, unlearnedSpells, spellTag,
                canAcquireSpells && spellLevel > 0);
        }
        else
        {
            group.RefreshLearning(characterBuildingService, knownSpells, unlearnedSpells, spellTag,
                canAcquireSpells);
        }

        group.slotStatusTable.Bind(null, spellLevel, false, null, false);
    }

    private static PointPool GetCurrentPool(CharacterStageSpellSelectionPanel panel,
        CharacterHeroBuildingData heroBuildingData)
    {
        var tag = string.Empty;
        for (var index = 0; index < panel.learnStepsTable.childCount; ++index)
        {
            var child = panel.learnStepsTable.GetChild(index);
            if (child.gameObject.activeSelf)
            {
                var component = child.GetComponent<LearnStepItem>();
                var status = index != panel.currentLearnStep
                    ? (index != panel.currentLearnStep - 1
                        ? LearnStepItem.Status.Locked
                        : LearnStepItem.Status.Previous)
                    : LearnStepItem.Status.InProgress;
                if (status == LearnStepItem.Status.InProgress)
                    tag = component.Tag;
            }
        }

        HeroDefinitions.PointsPoolType poolType;
        if (panel.IsFinalStep)
        {
            tag = panel.allTags[panel.allTags.Count - 1];
            poolType = panel.GetPoolTypeOfIndex(panel.currentLearnStep - 1);
        }
        else
        {
            poolType = panel.GetPoolTypeOfIndex(panel.currentLearnStep);
        }

        return ServiceRepository.GetService<ICharacterBuildingService>()
            .GetPointPoolOfTypeAndTag(heroBuildingData, poolType, tag);
    }

    private static void FilterMulticlassBleeding(
        [NotNull] SpellsByLevelGroup __instance,
        [NotNull] RulesetCharacterHero caster,
        [NotNull] List<SpellDefinition> allSpells,
        [NotNull] List<SpellDefinition> autoPreparedSpells,
        PointPool pointPool,
        Dictionary<SpellDefinition, string> extraSpellsMap)
    {
        var spellsOverriden = pointPool.spellListOverride != null;
        var spellLevel = __instance.SpellLevel;

        // avoids auto prepared spells from other classes to bleed in
        var allowedAutoPreparedSpells = LevelUpContext.GetAllowedAutoPreparedSpells(caster)
            .Where(x => x.SpellLevel == spellLevel).ToList();

        autoPreparedSpells.SetRange(allowedAutoPreparedSpells);

        //Select allowed spells - all spells if list is overriden by the pool, or all allowed spells of current level
        var allowedSpells = spellsOverriden
            ? new List<SpellDefinition>(allSpells)
            : LevelUpContext.GetAllowedSpells(caster).Where(x => x.SpellLevel == spellLevel).ToList();

        // displays known spells from other classes
        if (Main.Settings.DisplayAllKnownSpellsDuringLevelUp)
        {
            var otherClassesKnownSpells = LevelUpContext.GetOtherClassesKnownSpells(caster)
                .Where(x => x.SpellLevel == spellLevel).ToList();

            allSpells.RemoveAll(x => !allowedSpells.Contains(x) && !otherClassesKnownSpells.Contains(x));

            foreach (var spell in otherClassesKnownSpells)
            {
                if (!allSpells.Contains(spell))
                {
                    allSpells.Add(spell);
                    if (!extraSpellsMap.ContainsKey(spell))
                    {
                        extraSpellsMap.Add(spell, "Multiclass");
                    }
                }

                if (!Main.Settings.EnableRelearnSpells || !allowedSpells.Contains(spell))
                {
                    autoPreparedSpells.TryAdd(spell);
                }
            }
        }
        // remove spells bleed from other classes
        else
        {
            allSpells.RemoveAll(x => !allowedSpells.Contains(x));
        }
    }

    private static void CollectAllAutoPreparedSpells(
        [NotNull] SpellsByLevelGroup __instance,
        [NotNull] RulesetActor hero,
        [NotNull] List<SpellDefinition> allSpells,
        [NotNull] ICollection<SpellDefinition> auToPreparedSpells)
    {
        // Collect all the auto prepared spells.
        // Also filter the prepped spells by level this group is displaying.
        hero.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(hero.FeaturesToBrowse);

        foreach (var autoPreparedSpells in hero.FeaturesToBrowse.OfType<FeatureDefinitionAutoPreparedSpells>())
        {
            foreach (var spell in from preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups
                     from spell in preparedSpellsGroup.SpellsList
                     let flag = !auToPreparedSpells.Contains(spell) && __instance.SpellLevel == spell.SpellLevel
                     where flag
                     select spell)
            {
                auToPreparedSpells.Add(spell);

                // If a spell is not in all spells it won't be shown in the UI.
                // Add the auto prepared spells here to make sure the user sees them.
                allSpells.TryAdd(spell);
            }
        }
    }

     [CanBeNull]
     public static string GetLevelAndExperienceTooltip([NotNull] GuiCharacter character)
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
             builder.Append(Gui.Format("Format/&LevelAndExperienceMaxedFormat", characterLevel.ToString("N0"),
                 experience.ToString("N0")));
         }
         else
         {
             var num = Mathf.Max(0.0f, RuleDefinitions.ExperienceThresholds[characterLevel] - experience);

             builder.Append(Gui.Format("Format/&LevelAndExperienceFormat", characterLevel.ToString("N0"),
                 experience.ToString("N0"), num.ToString("N0"), (characterLevel + 1).ToString("N0")));
         }

         if (hero.ClassesAndLevels.Count <= 1)
         {
             return builder.ToString();
         }

         builder.Append('\n');

         for (var i = 0; i < hero.ClassesHistory.Count; i++)
         {
             var characterClassDefinition = hero.ClassesHistory[i];

             hero.ClassesAndSubclasses.TryGetValue(characterClassDefinition,
                 out var characterSubclassDefinition);

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

         return builder.ToString();
     }
}
