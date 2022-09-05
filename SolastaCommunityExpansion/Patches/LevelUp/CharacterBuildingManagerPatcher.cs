using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using TA;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Patches.LevelUp;

//PATCH: Replaces this method completely to remove weird 'return' on FeatureDefinitionCastSpell check
[HarmonyPatch(typeof(CharacterBuildingManager), "BrowseGrantedFeaturesHierarchically")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_BrowseGrantedFeaturesHierarchically
{
    internal static bool Prefix(
        [NotNull] CharacterBuildingManager __instance,
        [NotNull] CharacterHeroBuildingData heroBuildingData,
        [NotNull] List<FeatureDefinition> grantedFeatures,
        string tag)
    {
        var spellTag = tag;

        foreach (var grantedFeature in grantedFeatures)
        {
            switch (grantedFeature)
            {
                case FeatureDefinitionCastSpell spell:
                    __instance.SetupSpellPointPools(heroBuildingData, spell, spellTag);

                    break; //PATCH: this was `return` in original code, leading to game skipping granting some features
                case FeatureDefinitionBonusCantrips cantrips:
                    using (var enumerator = cantrips.BonusCantrips.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;
                            if (current != null)
                            {
                                __instance.AcquireBonusCantrip(heroBuildingData, current, spellTag);
                            }
                        }
                    }

                    break;
                case FeatureDefinitionProficiency definitionProficiency:
                    if (definitionProficiency.ProficiencyType == RuleDefinitions.ProficiencyType.FightingStyle)
                    {
                        using var enumerator = definitionProficiency.Proficiencies.GetEnumerator();

                        while (enumerator.MoveNext())
                        {
                            var current = enumerator.Current;

                            var element = DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(current);
                            __instance.AcquireBonusFightingStyle(heroBuildingData, element, spellTag);
                        }
                    }

                    break;
                case FeatureDefinitionFeatureSet definitionFeatureSet:
                    if (definitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                    {
                        __instance.BrowseGrantedFeaturesHierarchically(heroBuildingData,
                            definitionFeatureSet.FeatureSet, spellTag);
                    }

                    break;
            }
        }

        return false;
    }
}

//PATCH: registers the hero getting created
[HarmonyPatch(typeof(CharacterBuildingManager), "CreateNewCharacter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_CreateCharacter
{
    internal static void Postfix([NotNull] CharacterBuildingManager __instance)
    {
        LevelUpContext.RegisterHero(__instance.CurrentLocalHeroCharacter, null, null);
    }
}

//PATCH: registers the hero leveling up
[HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_LevelUpCharacter
{
    internal static void Prefix([NotNull] RulesetCharacterHero hero, ref bool force)
    {
        // PATCH: ensure the hero will get the experience gain
        if (Main.Settings.NoExperienceOnLevelUp)
        {
            force = true;
        }

        var lastClass = hero.ClassesHistory.Last();

        hero.ClassesAndSubclasses.TryGetValue(lastClass, out var lastSubclass);

        LevelUpContext.RegisterHero(hero, lastClass, lastSubclass, true);
    }
}

[HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_FinalizeCharacter
{
    //PATCH: original game code doesn't grant Race features above level 1
    internal static void Prefix([NotNull] CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
    {
        var characterLevelAttribute = hero.GetAttribute(AttributeDefinitions.CharacterLevel);

        characterLevelAttribute.Refresh();

        var characterLevel = characterLevelAttribute.CurrentValue;

        if (characterLevel == 1)
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

        __instance.GrantFeatures(hero, grantedFeatures, $"02Race{characterLevel}", false);
    }

    //PATCH: sorts spell repertoires, adds all known spells to whole list casters and unregisters the hero leveling up
    internal static void Postfix([NotNull] RulesetCharacterHero hero)
    {
        //
        // keep spell repertoires sorted by class title but ancestry one is always kept first
        //
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

        //TODO: Is this still required with new SpellMaps?
        //
        // Add whole list caster spells to KnownSpells collection to improve the MC spell selection UI
        //
        var selectedClassRepertoire = LevelUpContext.GetSelectedClassOrSubclassRepertoire(hero);

        if (selectedClassRepertoire == null
            || selectedClassRepertoire.SpellCastingFeature.SpellKnowledge !=
            RuleDefinitions.SpellKnowledge.WholeList)
        {
            LevelUpContext.UnregisterHero(hero);

            return;
        }

        var spellCastingClass = selectedClassRepertoire.SpellCastingClass;

        if (spellCastingClass == null)
        {
            LevelUpContext.UnregisterHero(hero);

            return;
        }

        var levels = hero.ClassesAndLevels[spellCastingClass];

        if (levels % 2 == 0)
        {
            return;
        }

        var castingLevel = SharedSpellsContext.GetClassSpellLevel(selectedClassRepertoire);
        var knownSpells = LevelUpContext.GetAllowedSpells(hero);

        if (knownSpells == null)
        {
            LevelUpContext.RecacheSpells(hero);

            knownSpells = LevelUpContext.GetAllowedSpells(hero);
        }

        selectedClassRepertoire.KnownSpells.AddRange(knownSpells
            .Where(x => x.SpellLevel == castingLevel));

        //
        // finally get rid of the hero context
        //
        LevelUpContext.UnregisterHero(hero);
    }
}

//PATCH: captures the desired class and ensures this doesn't get executed in the class panel level up screen
[HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_AssignClassLevel
{
    internal static bool Prefix([NotNull] RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
    {
        LevelUpContext.SetSelectedClass(hero, classDefinition);

        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

        if (isLevelingUp && !isClassSelectionStage)
        {
            LevelUpContext.GrantItemsIfRequired(hero);
        }

        return !(isLevelingUp && isClassSelectionStage);
    }
}

//PATCH: captures the desired sub class
[HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_AssignSubclass
{
    internal static void Prefix([NotNull] RulesetCharacterHero hero, CharacterSubclassDefinition subclassDefinition)
    {
        LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
    }
}

//PATCH: ensures this doesn't get executed under a specific MC scenario and only recursive grant features if not in that scenario
[HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_GrantFeatures
{
    internal static bool Prefix([NotNull] RulesetCharacterHero hero)
    {
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

        return !(isLevelingUp && isClassSelectionStage);
    }

    internal static void Postfix([NotNull] RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures,
        string tag)
    {
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

        // this is a MC scenario
        if (isLevelingUp && isClassSelectionStage)
        {
            return;
        }

        CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, tag, grantedFeatures);
    }
}

//
// These patches ensure that any custom features undo any required work
//

//TODO: Still required?
[HarmonyPatch(typeof(CharacterBuildingManager), "ClearPrevious")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_ClearPrevious
{
    private static readonly List<FeatureDefinition> ToRemove = new();

    internal static void Prefix(RulesetCharacterHero hero, [CanBeNull] string tag)
    {
        ToRemove.Clear();

        if (string.IsNullOrEmpty(tag) || !hero.ActiveFeatures.ContainsKey(tag))
        {
            return;
        }

        ToRemove.AddRange(hero.ActiveFeatures[tag]);
    }

    internal static void Postfix(RulesetCharacterHero hero, string tag)
    {
        if (ToRemove.Empty())
        {
            return;
        }

        //TODO: check if other places where this is called require same prefix/postfix treatment
        CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, ToRemove);
    }
}

//TODO: Still required?
[HarmonyPatch(typeof(CharacterBuildingManager), "UnassignRace")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_UnassignRace
{
    internal static void Prefix([NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var hero = heroBuildingData.HeroCharacter;
        const string TAG = AttributeDefinitions.TagRace;

        if (!hero.ActiveFeatures.ContainsKey(TAG))
        {
            return;
        }

        CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, TAG, hero.ActiveFeatures[TAG]);
    }
}

//TODO: Still required?
[HarmonyPatch(typeof(CharacterBuildingManager), "UnassignBackground")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_UnassignBackground
{
    internal static void Prefix([NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var hero = heroBuildingData.HeroCharacter;
        const string TAG = AttributeDefinitions.TagBackground;

        if (!hero.ActiveFeatures.ContainsKey(TAG))
        {
            return;
        }

        CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, TAG, hero.ActiveFeatures[TAG]);
    }
}

//PATCH: ensures this doesn't get executed in the class panel level up screen
[HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_UnassignLastClassLevel
{
    internal static bool Prefix([NotNull] RulesetCharacterHero hero)
    {
        var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
        var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

        //TODO: Still required?
        //
        // CUSTOM FEATURES BEHAVIOR
        //
        if (!isLevelingUp || !isClassSelectionStage)
        {
            var heroBuildingData = hero.GetOrCreateHeroBuildingData();
            var classDefinition = hero.ClassesHistory[heroBuildingData.HeroCharacter.ClassesHistory.Count - 1];
            var classesAndLevel = hero.ClassesAndLevels[classDefinition];
            var tag = AttributeDefinitions.GetClassTag(classDefinition, classesAndLevel);

            if (hero.ActiveFeatures.ContainsKey(tag))
            {
                CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
            }
        }

        if (isLevelingUp && !isClassSelectionStage)
        {
            LevelUpContext.UngrantItemsIfRequired(hero);
        }

        return !(isLevelingUp && isClassSelectionStage);
    }
}

//TODO: Still required?
[HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastSubclass")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_UnassignLastSubclass
{
    internal static void Prefix([NotNull] RulesetCharacterHero hero, bool onlyIfCurrentLevel = false)
    {
        var classDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
        var classLevel = hero.ClassesAndLevels[classDefinition];
        var level = 0;

        if (onlyIfCurrentLevel)
        {
            classDefinition.TryGetSubclassFeature(out _, out level);
        }

        if (!hero.ClassesAndSubclasses.ContainsKey(classDefinition) || (onlyIfCurrentLevel && classLevel > level))
        {
            return;
        }

        var classesAndSubclass = hero.ClassesAndSubclasses[classDefinition];
        var tag = AttributeDefinitions.GetSubclassTag(classDefinition, classLevel, classesAndSubclass);

        if (!hero.ActiveFeatures.ContainsKey(tag))
        {
            return;
        }

        CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
    }
}

//
// these patches support MC shared casters
//

//PATCH: ensures the level up process only presents / offers spells from current class
[HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_EnumerateKnownAndAcquiredSpells
{
    internal static void Postfix(
        [NotNull] CharacterHeroBuildingData heroBuildingData,
        List<SpellDefinition> __result)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isMulticlass = LevelUpContext.IsMulticlass(hero);

        if (!isMulticlass)
        {
            return;
        }

        if (Main.Settings.EnableRelearnSpells)
        {
            var otherClassesKnownSpells = LevelUpContext.GetOtherClassesKnownSpells(hero);

            __result.RemoveAll(x => otherClassesKnownSpells.Contains(x));
        }
        else
        {
            var allowedSpells = LevelUpContext.GetAllowedSpells(hero);

            __result.RemoveAll(x => !allowedSpells.Contains(x));
        }
    }
}

//PATCH: gets the correct spell feature for the selected class
[HarmonyPatch(typeof(CharacterBuildingManager), "GetSpellFeature")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_GetSpellFeature
{
    internal static bool Prefix(
        [NotNull] CharacterHeroBuildingData heroBuildingData,
        string tag,
        ref FeatureDefinitionCastSpell __result)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isMulticlass = LevelUpContext.IsMulticlass(hero);

        if (!isMulticlass)
        {
            return true;
        }

        var selectedClass = LevelUpContext.GetSelectedClass(hero);

        if (selectedClass == null)
        {
            return true;
        }

        var localTag = tag;

        __result = null;

        if (localTag.StartsWith(AttributeDefinitions.TagClass))
        {
            localTag = AttributeDefinitions.TagClass + selectedClass.Name;
        }
        else if (localTag.StartsWith(AttributeDefinitions.TagSubclass))
        {
            localTag = AttributeDefinitions.TagSubclass + selectedClass.Name;
        }

        // PATCH
        foreach (var activeFeature in hero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
        {
            foreach (var featureDefinition in activeFeature.Value
                         .OfType<FeatureDefinitionCastSpell>())
            {
                __result = featureDefinition;

                return false;
            }
        }

        if (!localTag.StartsWith(AttributeDefinitions.TagSubclass))
        {
            return false;
        }

        localTag = AttributeDefinitions.TagClass + selectedClass.Name;

        // PATCH
        foreach (var activeFeature in hero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
        {
            foreach (var featureDefinition in activeFeature.Value
                         .OfType<FeatureDefinitionCastSpell>())
            {
                __result = featureDefinition;

                return false;
            }
        }

        return false;
    }
}

//PATCH: ensures the level up process only offers slots from the leveling up class
[HarmonyPatch(typeof(CharacterBuildingManager), "UpgradeSpellPointPools")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterBuildingManager_UpgradeSpellPointPools
{
    internal static bool Prefix(
        CharacterBuildingManager __instance,
        [NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var hero = heroBuildingData.HeroCharacter;
        var isMulticlass = LevelUpContext.IsMulticlass(hero);

        if (!isMulticlass)
        {
            return true;
        }

        var selectedClass = LevelUpContext.GetSelectedClass(hero);
        var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);
        var selectedClassLevel = LevelUpContext.GetSelectedClassLevel(hero);

        foreach (var spellRepertoire in hero.SpellRepertoires)
        {
            var poolName = string.Empty;
            var maxPoints = 0;

            switch (spellRepertoire.SpellCastingFeature.SpellCastingOrigin)
            {
                // PATCH: short circuit if the feature is for another class (change from native code)
                case CastingOrigin.Class when spellRepertoire.SpellCastingClass != selectedClass:
                    continue;
                case CastingOrigin.Class:
                    poolName = AttributeDefinitions.GetClassTag(selectedClass, selectedClassLevel);
                    break;
                // PATCH: short circuit if the feature is for another subclass (change from native code)
                case CastingOrigin.Subclass when spellRepertoire.SpellCastingSubclass != selectedSubclass:
                    continue;
                case CastingOrigin.Subclass:
                    poolName = AttributeDefinitions.GetSubclassTag(selectedClass, selectedClassLevel, selectedSubclass);
                    break;
                case CastingOrigin.Race:
                    poolName = AttributeDefinitions.TagRace;
                    break;
            }

            if (__instance.HasAnyActivePoolOfType(heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip)
                && heroBuildingData.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools
                    .ContainsKey(poolName))
            {
                maxPoints = heroBuildingData.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip]
                    .ActivePools[poolName].MaxPoints;
            }

            heroBuildingData.TempAcquiredCantripsNumber = 0;
            heroBuildingData.TempAcquiredSpellsNumber = 0;
            heroBuildingData.TempUnlearnedSpellsNumber = 0;

            __instance.ApplyFeatureCastSpell(heroBuildingData, spellRepertoire.SpellCastingFeature);
            __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip, poolName,
                heroBuildingData.TempAcquiredCantripsNumber + maxPoints);
            __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.Spell, poolName,
                heroBuildingData.TempAcquiredSpellsNumber);
            __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.SpellUnlearn, poolName,
                heroBuildingData.TempUnlearnedSpellsNumber);
        }

        return false;
    }
}

//PATCH: fix a TA issue that not consider subclass morphotype preferences
[HarmonyPatch(typeof(CharacterBuildingManager), "AssignDefaultMorphotypes")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal class CharacterBuildingManager_AssignDefaultMorphotypes
{
    public static RangedInt PreferedSkinColors(
        RacePresentation racePresentation,
        [NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var subRaceDefinition = heroBuildingData.HeroCharacter.SubRaceDefinition;

        return subRaceDefinition != null
            ? subRaceDefinition.RacePresentation.PreferedSkinColors
            : racePresentation.PreferedSkinColors;
    }

    public static RangedInt PreferedHairColors(
        RacePresentation racePresentation,
        [NotNull] CharacterHeroBuildingData heroBuildingData)
    {
        var subRaceDefinition = heroBuildingData.HeroCharacter.SubRaceDefinition;

        return subRaceDefinition != null
            ? subRaceDefinition.RacePresentation.PreferedHairColors
            : racePresentation.PreferedHairColors;
    }

    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var preferedSkinColorsMethod = typeof(RacePresentation).GetMethod("get_PreferedSkinColors");
        var preferedHairColorsColorsMethod = typeof(RacePresentation).GetMethod("get_PreferedHairColors");
        var myPreferedSkinColorsMethod =
            typeof(CharacterBuildingManager_AssignDefaultMorphotypes).GetMethod("PreferedSkinColors");
        var myPreferedHairColorsColorsMethod =
            typeof(CharacterBuildingManager_AssignDefaultMorphotypes).GetMethod("PreferedHairColors");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(preferedSkinColorsMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg, 1); // heroBuildingData
                yield return new CodeInstruction(OpCodes.Call, myPreferedSkinColorsMethod);
            }
            else if (instruction.Calls(preferedHairColorsColorsMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg, 1); // heroBuildingData
                yield return new CodeInstruction(OpCodes.Call, myPreferedHairColorsColorsMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
