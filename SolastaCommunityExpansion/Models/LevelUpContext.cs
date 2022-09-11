using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class LevelUpContext
{
    // keeps a tab on all heroes leveling up
    private static readonly Dictionary<RulesetCharacterHero, LevelUpData> LevelUpTab = new();

    public static void RegisterHero(
        [NotNull] RulesetCharacterHero rulesetCharacterHero,
        CharacterClassDefinition lastClass,
        CharacterSubclassDefinition lastSubclass,
        bool levelingUp = false)
    {
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

    public static void UnregisterHero([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        LevelUpTab.Remove(rulesetCharacterHero);
    }

    [CanBeNull]
    public static CharacterClassDefinition GetSelectedClass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedClass
            : null;
    }

    public static void SetSelectedClass([NotNull] RulesetCharacterHero rulesetCharacterHero,
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
                levelUpData.SelectedClass == Bard ||
                levelUpData.SelectedClass == Ranger ||
                levelUpData.SelectedClass == Sorcerer ||
                levelUpData.SelectedClass == Warlock ||
                levelUpData.SelectedClass == Wizard //||
                // levelUpData.SelectedClass == TinkererClass
            ) &&
            !(
                classesAndLevels.ContainsKey(Bard) ||
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
    public static CharacterSubclassDefinition GetSelectedSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedSubclass
            : null;
    }

    public static void SetSelectedSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero,
        CharacterSubclassDefinition characterSubclassDefinition)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.SelectedSubclass = characterSubclassDefinition;
    }

    [CanBeNull]
    public static RulesetSpellRepertoire GetSelectedClassOrSubclassRepertoire(
        [NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x =>
            (x.SpellCastingClass != null && x.SpellCastingClass == GetSelectedClass(rulesetCharacterHero))
            || (x.SpellCastingSubclass != null &&
                x.SpellCastingSubclass == GetSelectedSubclass(rulesetCharacterHero)));
    }

    public static void SetIsClassSelectionStage([NotNull] RulesetCharacterHero rulesetCharacterHero,
        bool isClassSelectionStage)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.IsClassSelectionStage = isClassSelectionStage;
    }

    public static bool RequiresDeity([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.RequiresDeity;
    }

    // also referenced by 4 transpiler in PatchingContext
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

    public static bool IsClassSelectionStage([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) &&
               levelUpData.IsClassSelectionStage;
    }

    public static bool IsLevelingUp([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsLevelingUp;
    }

    public static bool IsMulticlass([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.SelectedClass != null
               && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                   || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));
    }

    public static bool IsRepertoireFromSelectedClassSubclass([NotNull] RulesetCharacterHero rulesetCharacterHero,
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
                    //TODO: check if we need to get known spells, or they all are in the spellbook
                    knownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
                    knownSpells.AddRange(spellRepertoire.KnownCantrips);
                    knownSpells.AddRange(spellRepertoire.KnownSpells);
                    knownSpells.AddRange(spellRepertoire.EnumerateAvailableScribedSpells());
                    break;
            }
        }

        return knownSpells.ToHashSet();
    }

    public static void CacheSpells([NotNull] RulesetCharacterHero rulesetCharacterHero)
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
    public static void RecacheSpells([NotNull] RulesetCharacterHero rulesetCharacterHero)
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

    public static HashSet<SpellDefinition> GetAllowedSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedSpells;
    }

    public static HashSet<SpellDefinition> GetAllowedAutoPreparedSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedAutoPreparedSpells;
    }

    public static HashSet<SpellDefinition> GetOtherClassesKnownSpells([NotNull] RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.OtherClassesKnownSpells;
    }

    public static void GrantItemsIfRequired([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        foreach (var grantedItem in levelUpData.GrantedItems)
        {
            rulesetCharacterHero.GrantItem(grantedItem, false);
        }
    }

    public static void UngrantItemsIfRequired([NotNull] RulesetCharacterHero rulesetCharacterHero)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        foreach (var grantedItem in levelUpData.GrantedItems)
        {
            rulesetCharacterHero.LoseItem(grantedItem, false);
        }
    }

    // keeps the multiclass level up context
    private sealed class LevelUpData
    {
        public CharacterClassDefinition SelectedClass;
        public CharacterSubclassDefinition SelectedSubclass;
        public bool IsClassSelectionStage { get; set; }
        public bool IsLevelingUp { get; set; }
        public bool RequiresDeity { get; set; }
        public HashSet<ItemDefinition> GrantedItems { get; set; }
        public HashSet<SpellDefinition> AllowedSpells { get; set; }
        public HashSet<SpellDefinition> AllowedAutoPreparedSpells { get; set; }
        public HashSet<SpellDefinition> OtherClassesKnownSpells { get; set; }
    }
}
