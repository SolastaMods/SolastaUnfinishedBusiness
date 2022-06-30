using System.Collections.Generic;
using System.Linq;
using static SolastaCommunityExpansion.Models.IntegrationContext;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ItemDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class LevelUpContext
{
    // keeps a tab on all heroes leveling up
    private static readonly Dictionary<RulesetCharacterHero, LevelUpData> LevelUpTab = new();

    public static RulesetCharacterHero GetHero(string name)
    {
        return LevelUpTab.FirstOrDefault(x => x.Key.Name == name).Key;
    }

    public static void RegisterHero(
        RulesetCharacterHero rulesetCharacterHero,
        CharacterClassDefinition lastClass,
        CharacterSubclassDefinition lastSubclass,
        bool levelingUp = false)
    {
        LevelUpTab.TryAdd(rulesetCharacterHero,
            new LevelUpData {SelectedClass = lastClass, SelectedSubclass = lastSubclass, IsLevelingUp = levelingUp});

        // fixes max level and exp in case level 20 gets enabled after a campaign starts
        var characterLevelAttribute = rulesetCharacterHero.GetAttribute(AttributeDefinitions.CharacterLevel);

        characterLevelAttribute.MaxValue = Main.Settings.EnableLevel20
            ? Level20Context.MOD_MAX_LEVEL
            : Level20Context.GAME_MAX_LEVEL;
        characterLevelAttribute.Refresh();

        var experienceAttribute = rulesetCharacterHero.GetAttribute(AttributeDefinitions.Experience);

        experienceAttribute.MaxValue = Main.Settings.EnableLevel20
            ? Level20Context.MOD_MAX_EXPERIENCE
            : Level20Context.GAME_MAX_EXPERIENCE;
        experienceAttribute.Refresh();
    }


    public static void UnregisterHero(RulesetCharacterHero rulesetCharacterHero)
    {
        LevelUpTab.Remove(rulesetCharacterHero);
    }

    public static CharacterClassDefinition GetSelectedClass(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedClass
            : null;
    }

    public static void SetSelectedClass(RulesetCharacterHero rulesetCharacterHero,
        CharacterClassDefinition characterClassDefinition)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.SelectedClass = characterClassDefinition;

        bool required;
        var classesAndLevels = rulesetCharacterHero.ClassesAndLevels;

        rulesetCharacterHero.ClassesAndSubclasses.TryGetValue(levelUpData.SelectedClass, out var subclass);
        levelUpData.SelectedSubclass = subclass;

        levelUpData.RequiresDeity =
            (levelUpData.SelectedClass == Cleric && !classesAndLevels.ContainsKey(Cleric))
            || (levelUpData.SelectedClass == Paladin && rulesetCharacterHero.DeityDefinition == null);

        levelUpData.GrantedItems = new HashSet<ItemDefinition>();

        // Holy Symbol
        required =
            (
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
                levelUpData.SelectedClass == Wizard ||
                levelUpData.SelectedClass == TinkererClass ||
                levelUpData.SelectedClass == WarlockClass ||
                levelUpData.SelectedClass == WitchClass
            ) &&
            !(
                classesAndLevels.ContainsKey(Ranger) ||
                classesAndLevels.ContainsKey(Sorcerer) ||
                classesAndLevels.ContainsKey(Wizard) ||
                classesAndLevels.ContainsKey(TinkererClass) ||
                classesAndLevels.ContainsKey(WarlockClass) ||
                classesAndLevels.ContainsKey(WitchClass)
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

    public static CharacterSubclassDefinition GetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
            ? levelUpData.SelectedSubclass
            : null;
    }

    public static void SetSelectedSubclass(RulesetCharacterHero rulesetCharacterHero,
        CharacterSubclassDefinition characterSubclassDefinition)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.SelectedSubclass = characterSubclassDefinition;
    }

    public static RulesetSpellRepertoire GetSelectedClassOrSubclassRepertoire(
        RulesetCharacterHero rulesetCharacterHero)
    {
        return rulesetCharacterHero.SpellRepertoires.FirstOrDefault(x =>
            (x.SpellCastingClass != null && x.SpellCastingClass == GetSelectedClass(rulesetCharacterHero))
            || (x.SpellCastingSubclass != null &&
                x.SpellCastingSubclass == GetSelectedSubclass(rulesetCharacterHero)));
    }

    public static void SetIsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero,
        bool isClassSelectionStage)
    {
        if (!LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData))
        {
            return;
        }

        levelUpData.IsClassSelectionStage = isClassSelectionStage;
    }

    public static bool RequiresDeity(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.RequiresDeity;
    }

    // also referenced by 4 transpilers in PatchingContext
    public static int GetSelectedClassLevel(RulesetCharacterHero rulesetCharacterHero)
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

    public static bool IsClassSelectionStage(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) &&
               levelUpData.IsClassSelectionStage;
    }

    public static bool IsLevelingUp(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData) && levelUpData.IsLevelingUp;
    }

    public static bool IsMulticlass(RulesetCharacterHero rulesetCharacterHero)
    {
        return LevelUpTab.TryGetValue(rulesetCharacterHero, out var levelUpData)
               && levelUpData.SelectedClass != null
               && (rulesetCharacterHero.ClassesAndLevels.Count > 1
                   || !rulesetCharacterHero.ClassesAndLevels.ContainsKey(levelUpData.SelectedClass));
    }

    public static bool IsRepertoireFromSelectedClassSubclass(RulesetCharacterHero rulesetCharacterHero,
        RulesetSpellRepertoire rulesetSpellRepertoire)
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

    private static HashSet<SpellDefinition> CacheAllowedAutoPreparedSpells(
        IEnumerable<FeatureDefinition> featureDefinitions)
    {
        var allowedAutoPreparedSpells = new List<SpellDefinition>();

        foreach (var featureDefinition in featureDefinitions)
        {
            if (featureDefinition is FeatureDefinitionAutoPreparedSpells featureDefinitionAutoPreparedSpells &&
                featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups != null)
            {
                allowedAutoPreparedSpells.AddRange(
                    featureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroups.SelectMany(x => x.SpellsList));
            }
        }

        return allowedAutoPreparedSpells.ToHashSet();
    }

    public static HashSet<SpellDefinition> CacheAllowedSpells(IEnumerable<FeatureDefinition> featureDefinitions)
    {
        var allowedSpells = new List<SpellDefinition>();

        foreach (var featureDefinition in featureDefinitions)
        {
            switch (featureDefinition)
            {
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

                case FeatureDefinitionBonusCantrips {BonusCantrips: { }} featureDefinitionBonusCantrips:
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

    private static HashSet<SpellDefinition> CacheOtherClassesKnownSpells(RulesetCharacterHero hero)
    {
        var selectedRepertoire = GetSelectedClassOrSubclassRepertoire(hero);
        var otherClassesKnownSpells = new List<SpellDefinition>();

        foreach (var spellRepertoire in hero.SpellRepertoires
                     .Where(x => x != selectedRepertoire))
        {
            otherClassesKnownSpells.AddRange(spellRepertoire.AutoPreparedSpells);
            otherClassesKnownSpells.AddRange(spellRepertoire.KnownCantrips);
            otherClassesKnownSpells.AddRange(spellRepertoire.KnownSpells);

            if (spellRepertoire.SpellCastingClass == Wizard)
            {
                otherClassesKnownSpells.AddRange(spellRepertoire.EnumerateAvailableScribedSpells());
            }
        }

        return otherClassesKnownSpells.ToHashSet();
    }

    public static void CacheSpells(RulesetCharacterHero rulesetCharacterHero)
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
        levelUpData.AllowedAutoPreparedSpells = CacheAllowedAutoPreparedSpells(thisClassCastingFeatures);
        levelUpData.OtherClassesKnownSpells = CacheOtherClassesKnownSpells(rulesetCharacterHero);
    }

    // supports character creation during boot up
    public static void RecacheSpells(RulesetCharacterHero rulesetCharacterHero)
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

    public static HashSet<SpellDefinition> GetAllowedSpells(RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedSpells;
    }

    public static HashSet<SpellDefinition> GetAllowedAutoPreparedSpells(RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.AllowedAutoPreparedSpells;
    }

    public static HashSet<SpellDefinition> GetOtherClassesKnownSpells(RulesetCharacterHero hero)
    {
        return !LevelUpTab.TryGetValue(hero, out var levelUpData)
            ? new HashSet<SpellDefinition>()
            : levelUpData.OtherClassesKnownSpells;
    }

    public static void GrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
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

    public static void UngrantItemsIfRequired(RulesetCharacterHero rulesetCharacterHero)
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
    private class LevelUpData
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
