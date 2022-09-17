using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaUnfinishedBusiness.Models;

public static class LevelUpContext
{
    // keeps a tab on all heroes leveling up
    private static readonly Dictionary<RulesetCharacterHero, LevelUpData> LevelUpTab = new();

    internal static void RegisterHero(
        [NotNull] RulesetCharacterHero rulesetCharacterHero,
        bool levelingUp)
    {
        CharacterClassDefinition lastClass = null;
        CharacterSubclassDefinition lastSubclass = null;

        if (levelingUp)
        {
            lastClass = rulesetCharacterHero.ClassesHistory.Last();
            rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(lastClass, out lastSubclass);
        }

        LevelUpTab.TryAdd(rulesetCharacterHero,
            new LevelUpData { SelectedClass = lastClass, SelectedSubclass = lastSubclass, IsLevelingUp = levelingUp });

        // fixes max level and exp in case level 20 gets enabled after a campaign starts
        var characterLevelAttribute = rulesetCharacterHero.GetAttribute(AttributeDefinitions.CharacterLevel);

        characterLevelAttribute.MaxValue = Main.Settings.EnableLevel20
            ? Level20Context.ModMaxLevel
            : Level20Context.GameMaxLevel;
        characterLevelAttribute.Refresh();

        var experienceAttribute = rulesetCharacterHero.GetAttribute(AttributeDefinitions.Experience);

        experienceAttribute.MaxValue = Main.Settings.EnableLevel20
            ? Level20Context.ModMaxExperience
            : Level20Context.GameMaxExperience;
        experienceAttribute.Refresh();
    }

    internal static void UnregisterHero([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        LevelUpTab.Remove(rulesetCharacterHero);
    }

    [CanBeNull]
    internal static CharacterClassDefinition GetSelectedClass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedClass
            : null;
    }

    internal static void SetSelectedClass([NotNull] RulesetCharacterHero rulesetCharacterHero,
        CharacterClassDefinition characterClassDefinition)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.SelectedClass = characterClassDefinition;

        var classesAndLevels = rulesetCharacterHero.ClassesAndLevels;

        rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(levelUpData.SelectedClass, out var subclass);
        levelUpData.SelectedSubclass = subclass;

        levelUpData.RequiresDeity =
            (levelUpData.SelectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric))
            || (levelUpData.SelectedClass == Paladin && rulesetCharacterHero.DeityDefinition == null);

        levelUpData.GrantedItems = new HashSet<ItemDefinition>();

        // Holy Symbol
        var required = (
                           levelUpData.SelectedClass == Cleric ||
                           levelUpData.SelectedClass == Paladin
                       ) &&
                       !(
                           classesAndLevels.ContainsKey(Cleric) ||
                           classesAndLevels.ContainsKey(Paladin)
                       );

        if (required)
        {
            levelUpData.GrantedItems.Add(HolySymbolAmulet);
        }

        // Clothes Wizard
        required =
            !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;

        if (required)
        {
            levelUpData.GrantedItems.Add(ClothesWizard);
        }

        // Component Pouch
        required =
            (
                levelUpData.SelectedClass == Ranger ||
                levelUpData.SelectedClass == Sorcerer ||
                levelUpData.SelectedClass == Warlock ||
                levelUpData.SelectedClass == Wizard //||
                // levelUpData.SelectedClass == TinkererClass
            ) &&
            !(
                classesAndLevels.ContainsKey(Ranger) ||
                classesAndLevels.ContainsKey(Sorcerer) ||
                classesAndLevels.ContainsKey(Warlock) ||
                classesAndLevels.ContainsKey(Wizard) //||
                // classesAndLevels.ContainsKey(TinkererClass)
            );

        if (required)
        {
            levelUpData.GrantedItems.Add(ComponentPouch);
        }

        // Bardic Flute
        required =
            levelUpData.SelectedClass == Bard && !classesAndLevels.ContainsKey(Bard);

        if (required)
        {
            levelUpData.GrantedItems.Add(Flute);
        }

        // Druidic Focus
        required =
            levelUpData.SelectedClass == Druid && !classesAndLevels.ContainsKey(Druid);

        if (required)
        {
            levelUpData.GrantedItems.Add(DruidicFocus);
        }

        // Spellbook
        required =
            !classesAndLevels.ContainsKey(Wizard) && levelUpData.SelectedClass == Wizard;

        if (required)
        {
            levelUpData.GrantedItems.Add(Spellbook);
        }
    }

    [CanBeNull]
    internal static CharacterSubclassDefinition GetSelectedSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedSubclass
            : null;
    }

    internal static void SetSelectedSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero,
        CharacterSubclassDefinition characterSubclassDefinition)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.SelectedSubclass = characterSubclassDefinition;
    }

    [CanBeNull]
    private static RulesetSpellRepertoire GetSelectedClassOrSubclassRepertoire(
        [NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x =>
            (x.SpellCastingClass != null && x.SpellCastingClass == GetSelectedClass(rulesetCharacterHero))
            || (x.SpellCastingSubclass != null &&
                x.SpellCastingSubclass == GetSelectedSubclass(rulesetCharacterHero)));
    }

    internal static void SetIsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero, bool isClassSelectionStage)
    {
        if (rulesetCharacterHero == null || !LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.IsClassSelectionStage = isClassSelectionStage;
    }

    internal static bool RequiresDeity([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.RequiresDeity;
    }

    // also referenced by 4 transpiler in PatchingContext (KEEP PUBLIC)
    public static int GetSelectedClassLevel([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        var selectedClass = GetSelectedClass(rulesetCharacterHero);

        if (selectedClass != null &&
            rulesetCharacterHero.ClassesAndLevels.TryGetValue(selectedClass, out var classLevel))
        {
            return classLevel;
        }

        // first time hero is getting this class
        return 1;
    }

    internal static bool IsClassSelectionStage([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) &&
               levelUpData.IsClassSelectionStage;
    }

    internal static bool IsLevelingUp([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsLevelingUp;
    }

    internal static bool IsMulticlass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.SelectedClass != null
               && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                   || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));
    }

    internal static bool IsRepertoireFromSelectedClassSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero,
        [NotNull] RulesetSpellRepertoire rulesetSpellRepertoire)
    {
        var selectedClass = GetSelectedClass(rulesetCharacterHero);
        var selectedSubclass = GetSelectedSubclass(rulesetCharacterHero);

        return
            (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
             FeatureDefinitionCastSpell.CastingOrigin.Class
             && rulesetSpellRepertoire.SpellCastingClass == selectedClass) ||
            (rulesetSpellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
             FeatureDefinitionCastSpell.CastingOrigin.Subclass
             && rulesetSpellRepertoire.SpellCastingSubclass == selectedSubclass);
    }

    [NotNull]
    private static HashSet<SpellDefinition> CacheAllowedAutoPreparedSpells(
        [NotNull] IEnumerable<FeatureDefinition> featureDefinitions)
    {
        var allowedAutoPreparedSpells = new List<SpellDefinition>();

        foreach (var featureDefinition in featureDefinitions)
        {
            switch (featureDefinition)
            {
                case FeatureDefinitionAutoPreparedSpells
                {
                    AutoPreparedSpellsGroups: { }
                } featureDefinitionAutoPreparedSpells:
                    allowedAutoPreparedSpells.AddRange(
                        featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups.SelectMany(x => x.SpellsList));
                    break;
                case FeatureDefinitionFeatureSet { uniqueChoices: false } featureDefinitionFeatureSet:
                    allowedAutoPreparedSpells.AddRange(
                        CacheAllowedAutoPreparedSpells(featureDefinitionFeatureSet.FeatureSet));
                    break;
            }
        }

        return allowedAutoPreparedSpells.ToHashSet();
    }

    [NotNull]
    private static HashSet<SpellDefinition> CacheAllowedSpells(
        [NotNull] IEnumerable<FeatureDefinition> featureDefinitions)
    {
        var allowedSpells = new List<SpellDefinition>();

        foreach (var featureDefinition in featureDefinitions)
        {
            switch (featureDefinition)
            {
                case FeatureDefinitionFeatureSet { uniqueChoices: false } featureDefinitionFeatureSet:
                    allowedSpells.AddRange(
                        CacheAllowedSpells(featureDefinitionFeatureSet.FeatureSet));
                    break;
                case FeatureDefinitionCastSpell featureDefinitionCastSpell
                    when featureDefinitionCastSpell.SpellListDefinition != null:
                    allowedSpells.AddRange(
                        featureDefinitionCastSpell.SpellListDefinition.SpellsByLevel.SelectMany(x => x.Spells));
                    break;

                case FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity
                    when featureDefinitionMagicAffinity.ExtendedSpellList != null:
                    allowedSpells.AddRange(
                        featureDefinitionMagicAffinity.ExtendedSpellList.SpellsByLevel.SelectMany(x => x.Spells));
                    break;

                case FeatureDefinitionBonusCantrips { BonusCantrips: { } } featureDefinitionBonusCantrips:
                    allowedSpells.AddRange(featureDefinitionBonusCantrips.BonusCantrips);
                    break;

                case FeatureDefinitionAutoPreparedSpells
                {
                    AutoPreparedSpellsGroups: { }
                } featureDefinitionAutoPreparedSpells:
                    allowedSpells.AddRange(
                        featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups.SelectMany(x => x.SpellsList));
                    break;
            }
        }

        return allowedSpells.ToHashSet();
    }

    [NotNull]
    private static HashSet<SpellDefinition> CacheOtherClassesKnownSpells([NotNull] RulesetCharacterHero hero)
    {
        var selectedRepertoire = GetSelectedClassOrSubclassRepertoire(hero);
        var knownSpells = new List<SpellDefinition>();

        foreach (var spellRepertoire in hero.SpellRepertoires
                     .Where(x => x != selectedRepertoire))
        {
            var castingFeature = spellRepertoire.SpellCastingFeature;

            switch (castingFeature.spellKnowledge)
            {
                case RuleDefinitions.SpellKnowledge.WholeList:
                    knownSpells.AddRange(castingFeature.SpellListDefinition.SpellsByLevel.SelectMany(s => s.Spells));
                    break;
                case RuleDefinitions.SpellKnowledge.Selection:
                    knownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                    knownSpells.AddRange(spellRepertoire.KnownCantrips);
                    knownSpells.AddRange(spellRepertoire.KnownSpells);
                    break;
                case RuleDefinitions.SpellKnowledge.Spellbook:
                    knownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                    knownSpells.AddRange(spellRepertoire.KnownCantrips);
                    knownSpells.AddRange(spellRepertoire.KnownSpells);
                    knownSpells.AddRange(spellRepertoire.EnumerateAvailableScribedSpells());
                    break;
            }
        }

        return knownSpells.ToHashSet();
    }

    internal static void CacheSpells([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        var selectedClassName = levelUpData.SelectedClass.Name;
        var thisClassCastingFeatures = rulesetCharacterHero.ActiveFeatures
            .Where(x => x.Key.Contains(selectedClassName))
            .SelectMany(x => x.Value);

        var classCastingFeatures =
            thisClassCastingFeatures as FeatureDefinition[] ?? thisClassCastingFeatures.ToArray();
        levelUpData.AllowedSpells = CacheAllowedSpells(classCastingFeatures);
        levelUpData.AllowedAutoPreparedSpells = CacheAllowedAutoPreparedSpells(classCastingFeatures);
        levelUpData.OtherClassesKnownSpells = CacheOtherClassesKnownSpells(rulesetCharacterHero);
    }

    // supports character creation during boot up
    private static void RecacheSpells([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        var selectedClassName = levelUpData.SelectedClass.Name;
        var thisClassCastingFeatures = rulesetCharacterHero.ActiveFeatures
            .Where(x => x.Key.Contains(selectedClassName))
            .SelectMany(x => x.Value);

        levelUpData.AllowedSpells = CacheAllowedSpells(thisClassCastingFeatures);
    }

    internal static HashSet<SpellDefinition> GetAllowedSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedSpells;
    }

    internal static IEnumerable<SpellDefinition> GetAllowedAutoPreparedSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedAutoPreparedSpells;
    }

    internal static HashSet<SpellDefinition> GetOtherClassesKnownSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.OtherClassesKnownSpells;
    }

    internal static void GrantItemsIfRequired([NotNull] RulesetCharacterHero hero)
    {
        if (!LevelUpTab.TryGetValue(hero, out var levelUpData) || !levelUpData.IsLevelingUp)
        {
            return;
        }

        foreach (var grantedItem in levelUpData.GrantedItems)
        {
            hero.GrantItem(grantedItem, false);
        }
    }

    internal static void GrantRaceFeatures(
        CharacterBuildingManager characterBuildingManager,
        RulesetCharacterHero hero)
    {
        var characterLevel = hero.ClassesHistory.Count;

        // game correctly handles level 1
        if (characterLevel <= 1)
        {
            return;
        }

        var raceDefinition = hero.RaceDefinition;
        var subRaceDefinition = hero.SubRaceDefinition;
        var grantedFeatures = new List<FeatureDefinition>();

        raceDefinition.FeatureUnlocks
            .Where(x => x.Level == characterLevel)
            .Do(x => grantedFeatures.Add(x.FeatureDefinition));

        if (subRaceDefinition != null)
        {
            subRaceDefinition.FeatureUnlocks
                .Where(x => x.Level == characterLevel)
                .Do(x => grantedFeatures.Add(x.FeatureDefinition));
        }

        characterBuildingManager.GrantFeatures(hero, grantedFeatures, $"02Race{characterLevel}", false);
    }

    internal static void SortHeroRepertoires(RulesetCharacterHero hero)
    {
        if (hero.SpellRepertoires.Count <= 2)
        {
            return;
        }

        hero.SpellRepertoires.Sort((a, b) =>
        {
            if (a.SpellCastingRace != null)
            {
                return -1;
            }

            if (b.SpellCastingRace != null)
            {
                return 1;
            }

            var title1 = a.SpellCastingClass != null
                ? a.SpellCastingClass.FormatTitle()
                : a.SpellCastingSubclass.FormatTitle();

            var title2 = b.SpellCastingClass != null
                ? b.SpellCastingClass.FormatTitle()
                : b.SpellCastingSubclass.FormatTitle();

            return String.Compare(title1, title2, StringComparison.CurrentCultureIgnoreCase);
        });
    }

    internal static void UpdateKnownSpellsForWholeCasters(RulesetCharacterHero hero)
    {
        var spellRepertoire = GetSelectedClassOrSubclassRepertoire(hero);

        // only whole list casters
        if (spellRepertoire == null
            || spellRepertoire.SpellCastingFeature.SpellKnowledge !=
            RuleDefinitions.SpellKnowledge.WholeList)
        {
            return;
        }

        // only repertoires with a casting class
        var spellCastingClass = spellRepertoire.SpellCastingClass;

        if (spellCastingClass == null)
        {
            return;
        }

        // add all known spells up to that level
        var castingLevel = SharedSpellsContext.GetClassSpellLevel(spellRepertoire);
        var knownSpells = GetAllowedSpells(hero);

        if (knownSpells == null)
        {
            RecacheSpells(hero);

            knownSpells = GetAllowedSpells(hero);
        }

        foreach (var spell in knownSpells
                     .Where(x => x.SpellLevel == castingLevel))
        {
            spellRepertoire.KnownSpells.TryAdd(spell);
        }
    }

    internal static void GrantCustomFeatures(RulesetCharacterHero hero)
    {
        var buildingData = hero.GetHeroBuildingData();
        var level = hero.ClassesHistory.Count;
        var selectedClass = GetSelectedClass(hero);
        var selectedSubclass = GetSelectedSubclass(hero);

        foreach (var kvp in buildingData.LevelupTrainedFeats)
        {
            foreach (var feat in kvp.Value)
            {
                CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, kvp.Key, feat.Features);
            }
        }

        var classTag = AttributeDefinitions.GetClassTag(selectedClass, level);

        if (hero.ActiveFeatures.TryGetValue(classTag, out var classFeatures))
        {
            CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, classTag, classFeatures);
        }

        if (selectedSubclass == null)
        {
            return;
        }

        var subclassTag = AttributeDefinitions.GetSubclassTag(selectedClass, level, selectedSubclass);

        if (hero.ActiveFeatures.TryGetValue(subclassTag, out var subclassFeatures))
        {
            CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, classTag, subclassFeatures);
        }
    }

    internal static void EnumerateKnownAndAcquiredSpells(
        [NotNull] CharacterHeroBuildingData heroBuildingData,
        List<SpellDefinition> __result)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isMulticlass = IsMulticlass(hero);

        if (!isMulticlass)
        {
            return;
        }

        if (Main.Settings.EnableRelearnSpells)
        {
            var otherClassesKnownSpells = GetOtherClassesKnownSpells(hero);

            __result.RemoveAll(x => otherClassesKnownSpells.Contains(x));
        }
        else
        {
            var allowedSpells = GetAllowedSpells(hero);

            __result.RemoveAll(x => !allowedSpells.Contains(x));
        }
    }

    [NotNull]
    internal static CharacterClassDefinition GetClassForSubclass(CharacterSubclassDefinition subclass)
    {
        return DatabaseRepository.GetDatabase<CharacterClassDefinition>().FirstOrDefault(klass =>
        {
            return klass.FeatureUnlocks.Any(unlock =>
            {
                if (unlock.FeatureDefinition is FeatureDefinitionSubclassChoice subclassChoice)
                {
                    return subclassChoice.Subclasses.Contains(subclass.Name);
                }

                return false;
            });
        })!;
    }

    // keeps the multiclass level up context
    private sealed class LevelUpData
    {
        public CharacterClassDefinition SelectedClass;
        public CharacterSubclassDefinition SelectedSubclass;

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public bool IsClassSelectionStage { get; set; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public bool IsLevelingUp { get; set; }

        // ReSharper disable once MemberHidesStaticFromOuterClass
        public bool RequiresDeity { get; set; }
        public HashSet<ItemDefinition> GrantedItems { get; set; }
        public HashSet<SpellDefinition> AllowedSpells { get; set; }
        public HashSet<SpellDefinition> AllowedAutoPreparedSpells { get; set; }
        public HashSet<SpellDefinition> OtherClassesKnownSpells { get; set; }
    }
}
