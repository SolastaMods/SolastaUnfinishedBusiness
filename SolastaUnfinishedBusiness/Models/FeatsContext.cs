using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using UnityEngine;
using UnityEngine.UI;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaUnfinishedBusiness.Models;

internal static class FeatsContext
{
    private const int Columns = 3;
    internal const int Width = 300;
    internal const int Height = 44;
    internal const int Spacing = 5;
    internal const int MinInitialFeats = 0;
    internal const int MaxInitialFeats = 4; // don't increase this value to avoid issue reports on crazy scenarios

    internal static HashSet<FeatDefinition> Feats { get; private set; } = [];
    internal static HashSet<FeatDefinition> FeatGroups { get; private set; } = [];
    private static int PreviousTotalFeatsGrantedFirstLevel { get; set; } = -1;
    private static bool PreviousAlternateHuman { get; set; }

    internal static void Load()
    {
        LoadFeatsPointPools();
        SwitchAsiAndFeat();
        SwitchFirstLevelTotalFeats();
        SwitchEveryFourLevelsFeats();
        SwitchEveryFourLevelsFeats(true);
    }

    internal static void LateLoad()
    {
        var feats = new List<FeatDefinition>();

        // generate feats here and fill the list
        ArmorFeats.CreateFeats(feats);
        CasterFeats.CreateFeats(feats);
        OtherFeats.CreateFeats(feats); // must come before Class Feats
        ClassFeats.CreateFeats(feats);
        CraftyFeats.CreateFeats(feats);
        CriticalVirtuosoFeats.CreateFeats(feats);
        DefenseExpertFeats.CreateFeats(feats);
        MeleeCombatFeats.CreateFeats(feats);
        PrecisionFocusedFeats.CreateFeats(feats);
        RaceFeats.CreateFeats(feats);
        RangedCombatFeats.CreateFeats(feats);
        TwoWeaponCombatFeats.CreateFeats(feats);

        // load them in mod UI
        feats.ForEach(LoadFeat);
        GroupFeats.Load(LoadFeatGroup);

        // tweak the groups to make display simpler on mod UI
        Feats.RemoveWhere(x => x.FormatTitle().Contains("["));
        Feats.RemoveWhere(x => x.HasSubFeatureOfType<HideFromFeats>());

        foreach (var child in AttributeDefinitions.AbilityScoreNames
                     .Select(attribute => DatabaseRepository.GetDatabase<FeatDefinition>()
                         .GetElement($"FeatGroupHalf{attribute}")))
        {
            FeatGroups.Remove(child);
        }

        foreach (var featGroup in FeatGroups
                     .Where(featGroup =>
                         !string.IsNullOrEmpty(featGroup.FamilyTag) &&
                         featGroup.Name != "FeatGroupElementalTouch")
                     .ToArray())
        {
            FeatGroups.Remove(featGroup);

            if (!CasterFeats.MagicTouchedData.ContainsKey(featGroup.Name.Replace("FeatGroup", string.Empty)))
            {
                LoadFeat(featGroup);
            }
        }

        // sorting
        Feats = [.. Feats.OrderBy(x => x.FormatTitle())];
        FeatGroups = [.. FeatGroups.OrderBy(x => x.FormatTitle())];

        foreach (var groupedFeat in GroupFeats.Groups
                     .Select(groupDefinition => groupDefinition.GetFirstSubFeatureOfType<GroupedFeat>()))
        {
            groupedFeat?.Feats.Sort((a, b) => String.CompareOrdinal(a.FormatTitle(), b.FormatTitle()));
        }

        // settings paring feats
        foreach (var name in Main.Settings.FeatEnabled
                     .Where(name => Feats.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.FeatEnabled.Remove(name);
        }

        // settings paring groups
        foreach (var name in Main.Settings.FeatGroupEnabled
                     .Where(name => FeatGroups.All(x => x.Name != name))
                     .ToArray())
        {
            Main.Settings.FeatGroupEnabled.Remove(name);
        }

        // handle Half Attributes subgroups special case
        SwitchHalfAttributes(Main.Settings.FeatGroupEnabled.Contains("FeatGroupHalfAttributes"));

        // avoids restart on level up UI
        GuiWrapperContext.RecacheFeats();
    }

    private static void LoadFeat([NotNull] FeatDefinition featDefinition)
    {
        Feats.Add(featDefinition);
        UpdateFeatsVisibility(featDefinition, !Main.Settings.FeatEnabled.Contains(featDefinition.Name));
    }

    private static void LoadFeatGroup([NotNull] FeatDefinition featDefinition)
    {
        FeatGroups.Add(featDefinition);
        UpdateFeatGroupsVisibility(featDefinition);
    }

    private static void UpdateFeatsVisibility([NotNull] BaseDefinition featDefinition, bool hidden)
    {
        featDefinition.GuiPresentation.hidden = hidden;

        var groupedFeat = featDefinition.GetFirstSubFeatureOfType<GroupedFeat>();

        groupedFeat?.GetSubFeats(true, true).ForEach(x => UpdateFeatsVisibility(x, hidden));
    }

    private static void UpdateFeatGroupsVisibility([NotNull] BaseDefinition featDefinition)
    {
        featDefinition.GuiPresentation.hidden = !Main.Settings.FeatGroupEnabled.Contains(featDefinition.Name);
    }

    internal static void SwitchFeat(FeatDefinition featDefinition, bool active)
    {
        if (!Feats.Contains(featDefinition))
        {
            return;
        }

        var name = featDefinition.Name;

        if (active)
        {
            Main.Settings.FeatEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FeatEnabled.Remove(name);
        }

        UpdateFeatsVisibility(featDefinition, !Main.Settings.FeatEnabled.Contains(featDefinition.Name));
        GuiWrapperContext.RecacheFeats();
    }

    private static void SwitchHalfAttributes(bool active)
    {
        foreach (var child in AttributeDefinitions.AbilityScoreNames
                     .Select(attribute =>
                         DatabaseRepository.GetDatabase<FeatDefinition>().GetElement($"FeatGroupHalf{attribute}")))
        {
            child.GuiPresentation.hidden = !active;
        }
    }

    internal static void SwitchFeatGroup(FeatDefinition featDefinition, bool active)
    {
        if (!FeatGroups.Contains(featDefinition))
        {
            return;
        }

        var name = featDefinition.Name;

        if (name == "FeatGroupHalfAttributes")
        {
            SwitchHalfAttributes(active);
        }

        if (active)
        {
            Main.Settings.FeatGroupEnabled.TryAdd(name);
        }
        else
        {
            Main.Settings.FeatGroupEnabled.Remove(name);
        }

        UpdateFeatGroupsVisibility(featDefinition);
        GuiWrapperContext.RecacheFeats();
    }

    internal static void UpdatePanelChildren(FeatSubPanel panel)
    {
        // get missing children from pool
        while (panel.table.childCount < panel.relevantFeats.Count)
        {
            Gui.GetPrefabFromPool(panel.itemPrefab, panel.table);
        }

        // release extra children to pool
        while (panel.table.childCount > panel.relevantFeats.Count)
        {
            Gui.ReleaseInstanceToPool(panel.table.GetChild(panel.table.childCount - 1).gameObject);
        }
    }

    // called before sorting feats to hide sub-feats during level up
    private static void ProcessFeatGroups(FeatSubPanel panel, bool active, Transform table)
    {
        //this is not feat learning - skip manipulations
        if (!active)
        {
            return;
        }

        var toRemove = new List<FeatDefinition>();

        foreach (var group in panel.relevantFeats
                     .Select(feat => feat.GetFirstSubFeatureOfType<IGroupedFeat>())
                     .Where(group => group is { HideSubFeats: true }))
        {
            toRemove.AddRange(group.GetSubFeats());
        }

        for (var i = 0; i < table.childCount; i++)
        {
            var child = table.GetChild(i);
            var featItem = child.GetComponent<FeatItem>();

            if (toRemove.Contains(featItem.GuiFeatDefinition.FeatDefinition))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    internal static void SortFeats(FeatSubPanel panel)
    {
        panel.relevantFeats.Sort(CompareFeats);
    }

    internal static int CompareFeats(FeatDefinition a, FeatDefinition b)
    {
        return string.Compare(a.FormatTitle(), b.FormatTitle(),
            StringComparison.CurrentCultureIgnoreCase);
    }

    internal static void UpdateRelevantFeatList(FeatSubPanel panel)
    {
        var dbFeatDefinition = DatabaseRepository.GetDatabase<FeatDefinition>();
        var visibleFeats = dbFeatDefinition
            .Where(x => !x.GuiPresentation.Hidden)
            .ToArray();

        panel.relevantFeats.SetRange(visibleFeats
            .Where(f => f.GetFirstSubFeatureOfType<IGroupedFeat>() is not { } group
                        || group.GetSubFeats().Count(s => visibleFeats.Contains(s)) > 1)
        );
    }

    internal static void ForceSameWidth(RectTransform table, bool active, FeatSubPanel panel)
    {
        ProcessFeatGroups(panel, active, table);

        if (active && Main.Settings.EnableSameWidthFeatSelection)
        {
            var hero = Global.LevelUpHero;
            var buildingData = hero?.GetHeroBuildingData();

            if (buildingData == null)
            {
                return;
            }

            var trainedFeats = buildingData.LevelupTrainedFeats
                .SelectMany(x => x.Value)
                .Union(hero.TrainedFeats)
                .ToArray();

            var j = 0;
            RectTransform rect;

            for (var i = 0; i < table.childCount; i++)
            {
                var child = table.GetChild(i);
                var featItem = child.GetComponent<FeatItem>();

                if (!child.gameObject.activeSelf || trainedFeats.Contains(featItem.GuiFeatDefinition.FeatDefinition))
                {
                    continue;
                }

                var x = j % Columns;
                var y = j / Columns;
                var posX = x * (Width + (Spacing * 2));
                var posY = -y * (Height + Spacing);

                rect = child.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(posX, posY);
                rect.sizeDelta = new Vector2(Width, Height);

                j++;
            }

            rect = table.GetComponent<RectTransform>();
            // ReSharper disable once PossibleLossOfFraction
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((j / Columns) + 1) * (Height + Spacing));
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(table);
    }

    private static void LoadFeatsPointPools()
    {
        // create feats point pools
        // +1 here as need to count the Alternate Human Feat
        for (var i = 1; i <= MaxInitialFeats + 1; i++)
        {
            var s = i.ToString();

            _ = FeatureDefinitionPointPoolBuilder
                .Create($"PointPool{i}BonusFeats")
                .SetGuiPresentation(
                    Gui.Format("Feature/&PointPoolSelectBonusFeatsTitle", s),
                    Gui.Format("Feature/&PointPoolSelectBonusFeatsDescription", s))
                .SetPool(HeroDefinitions.PointsPoolType.Feat, i)
                .AddToDB();
        }
    }
    
    internal static void SwitchFirstLevelTotalFeats()
    {
        if (PreviousTotalFeatsGrantedFirstLevel > -1)
        {
            UnloadRacesLevel1Feats(PreviousTotalFeatsGrantedFirstLevel, PreviousAlternateHuman);
        }

        PreviousTotalFeatsGrantedFirstLevel = Main.Settings.TotalFeatsGrantedFirstLevel;
        PreviousAlternateHuman = Main.Settings.EnableAlternateHuman;
        LoadRacesLevel1Feats(Main.Settings.TotalFeatsGrantedFirstLevel, Main.Settings.EnableAlternateHuman);
    }
    
    private static void LoadRacesLevel1Feats(int initialFeats, bool alternateHuman)
    {
        var human = Human;

        BuildFeatureUnlocks(initialFeats, alternateHuman, out var featureUnlockByLevelNonHuman,
            out var featureUnlockByLevelHuman);

        foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
        {
            if (IsSubRace(characterRaceDefinition))
            {
                continue;
            }

            if (alternateHuman && characterRaceDefinition == human)
            {
                if (featureUnlockByLevelHuman != null)
                {
                    human.FeatureUnlocks.Add(featureUnlockByLevelHuman);
                }

                var pointPoolAbilityScoreImprovement =
                    new FeatureUnlockByLevel(PointPoolAbilityScoreImprovement, 1);
                human.FeatureUnlocks.Add(pointPoolAbilityScoreImprovement);

                var pointPoolHumanSkillPool = new FeatureUnlockByLevel(PointPoolHumanSkillPool, 1);
                human.FeatureUnlocks.Add(pointPoolHumanSkillPool);

                Remove(human,
                    FeatureDefinitionAttributeModifiers
                        .AttributeModifierHumanAbilityScoreIncrease);
            }
            else
            {
                if (featureUnlockByLevelNonHuman != null)
                {
                    characterRaceDefinition.FeatureUnlocks.Add(featureUnlockByLevelNonHuman);
                }
            }
        }
    }

    private static void UnloadRacesLevel1Feats(int initialFeats, bool alternateHuman)
    {
        var human = Human;

        BuildFeatureUnlocks(initialFeats, alternateHuman,
            out var featureUnlockByLevelNonHuman,
            out var featureUnlockByLevelHuman);

        foreach (var characterRaceDefinition in DatabaseRepository.GetDatabase<CharacterRaceDefinition>())
        {
            if (IsSubRace(characterRaceDefinition))
            {
                continue;
            }

            if (alternateHuman && characterRaceDefinition == human)
            {
                if (featureUnlockByLevelHuman != null)
                {
                    Remove(human, featureUnlockByLevelHuman);
                }

                Remove(human, PointPoolAbilityScoreImprovement);
                Remove(human, PointPoolHumanSkillPool);

                var humanAttributeIncrease = new FeatureUnlockByLevel(
                    FeatureDefinitionAttributeModifiers.AttributeModifierHumanAbilityScoreIncrease, 1);

                human.FeatureUnlocks.Add(humanAttributeIncrease);
            }
            else
            {
                if (featureUnlockByLevelNonHuman != null)
                {
                    Remove(characterRaceDefinition, featureUnlockByLevelNonHuman);
                }
            }
        }
    }

    private static void Remove(
        [NotNull] CharacterRaceDefinition characterRaceDefinition,
        BaseDefinition toRemove)
    {
        var ndx = -1;

        for (var i = 0; i < characterRaceDefinition.FeatureUnlocks.Count; i++)
        {
            if (characterRaceDefinition.FeatureUnlocks[i].Level == 1 &&
                characterRaceDefinition.FeatureUnlocks[i].FeatureDefinition == toRemove)
            {
                ndx = i;
            }
        }

        if (ndx >= 0)
        {
            characterRaceDefinition.FeatureUnlocks.RemoveAt(ndx);
        }
    }

    private static void Remove(
        [NotNull] CharacterRaceDefinition characterRaceDefinition,
        [NotNull] FeatureUnlockByLevel featureUnlockByLevel)
    {
        Remove(characterRaceDefinition, featureUnlockByLevel.FeatureDefinition);
    }

    private static bool IsSubRace(CharacterRaceDefinition raceDefinition)
    {
        return DatabaseRepository.GetDatabase<CharacterRaceDefinition>()
            .Any(crd => crd.SubRaces.Contains(raceDefinition));
    }

    internal static void SwitchAsiAndFeat()
    {
        FeatureSetAbilityScoreChoice.mode = Main.Settings.EnablesAsiAndFeat
            ? FeatureDefinitionFeatureSet.FeatureSetMode.Union
            : FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion;
    }


    internal static void SwitchEveryFourLevelsFeats(bool isMiddle = false)
    {
        var levels = isMiddle ? new[] { 6, 14 } : [2, 10, 18];
        var dbCharacterClassDefinition = DatabaseRepository.GetDatabase<CharacterClassDefinition>();
        var pointPool1BonusFeats = GetDefinition<FeatureDefinitionPointPool>("PointPool1BonusFeats");
        var pointPool2BonusFeats = GetDefinition<FeatureDefinitionPointPool>("PointPool2BonusFeats");
        var enable = isMiddle
            ? Main.Settings.EnableFeatsAtEveryFourLevelsMiddle
            : Main.Settings.EnableFeatsAtEveryFourLevels;

        foreach (var characterClassDefinition in dbCharacterClassDefinition)
        {
            foreach (var level in levels)
            {
                var featureUnlockPointPool1 = new FeatureUnlockByLevel(pointPool1BonusFeats, level);
                var featureUnlockPointPool2 = new FeatureUnlockByLevel(pointPool2BonusFeats, level);

                if (enable)
                {
                    characterClassDefinition.FeatureUnlocks.Add(ShouldBe2Points()
                        ? featureUnlockPointPool2
                        : featureUnlockPointPool1);
                }
                else
                {
                    if (ShouldBe2Points())
                    {
                        characterClassDefinition.FeatureUnlocks.RemoveAll(x =>
                            x.FeatureDefinition == pointPool2BonusFeats && x.level == level);
                    }
                    else
                    {
                        characterClassDefinition.FeatureUnlocks.RemoveAll(x =>
                            x.FeatureDefinition == pointPool1BonusFeats && x.level == level);
                    }
                }

                continue;

                bool ShouldBe2Points()
                {
                    return (characterClassDefinition == Rogue && level is 10 && !isMiddle) ||
                           (characterClassDefinition == Fighter && level is 6 or 14 && isMiddle);
                }
            }

            characterClassDefinition.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    private static void BuildFeatureUnlocks(
        int initialFeats,
        bool alternateHuman,
        [CanBeNull] out FeatureUnlockByLevel featureUnlockByLevelNonHuman,
        [CanBeNull] out FeatureUnlockByLevel featureUnlockByLevelHuman)
    {
        string name;

        featureUnlockByLevelNonHuman = null;
        featureUnlockByLevelHuman = null;

        switch (initialFeats)
        {
            case 0:
            {
                if (alternateHuman)
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(PointPoolBonusFeat, 1);
                }

                break;
            }
            case 1:
            {
                featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(PointPoolBonusFeat, 1);

                name = "PointPool2BonusFeats";
                if (alternateHuman && TryGetDefinition<FeatureDefinitionPointPool>(name, out var pointPool2BonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPool2BonusFeats, 1);
                }

                break;
            }
            case > 1:
            {
                name = $"PointPool{initialFeats}BonusFeats";
                if (TryGetDefinition<FeatureDefinitionPointPool>(name, out var featureDefinitionPointPool))
                {
                    featureUnlockByLevelNonHuman = new FeatureUnlockByLevel(featureDefinitionPointPool, 1);
                }

                name = $"PointPool{initialFeats + 1}BonusFeats";
                if (alternateHuman && TryGetDefinition<FeatureDefinitionPointPool>(name, out var pointPoolXBonusFeats))
                {
                    featureUnlockByLevelHuman = new FeatureUnlockByLevel(pointPoolXBonusFeats, 1);
                }

                break;
            }
        }
    }

    internal sealed class HideFromFeats
    {
        internal static readonly HideFromFeats Marker = new();
    }
}
