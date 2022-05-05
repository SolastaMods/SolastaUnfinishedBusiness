using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    // register the hero getting created
    [HarmonyPatch(typeof(CharacterBuildingManager), "CreateNewCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_CreateCharacter
    {
        internal static void Postfix(CharacterBuildingManager __instance)
        {
            LevelUpContext.RegisterHero(__instance.CurrentLocalHeroCharacter, null, null);
        }
    }

    // register the hero leveling up
    [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_LevelUpCharacter
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var lastClass = hero.ClassesHistory.Last();

            hero.ClassesAndSubclasses.TryGetValue(lastClass, out var lastSubclass);

            LevelUpContext.RegisterHero(hero, lastClass, lastSubclass, levelingUp: true);
        }
    }

    // unregister the hero leveling up
    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_FinalizeCharacter
    {
        internal static void Postfix(RulesetCharacterHero hero)
        {
            LevelUpContext.UnregisterHero(hero);
        }
    }

    // captures the desired class
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignClassLevel
    {
        internal static void Prefix(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            LevelUpContext.SetSelectedClass(hero, classDefinition);
        }
    }

    // captures the desired sub class
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignSubclass
    {
        internal static void Prefix(RulesetCharacterHero hero, CharacterSubclassDefinition subclassDefinition)
        {
            LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static void Postfix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures, string tag)
        {
            CustomFeaturesContext.RecursiveGrantCustomFeatures(hero, grantedFeatures, tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "ClearPrevious")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_ClearPrevious
    {
        internal static void Prefix(RulesetCharacterHero hero, string tag)
        {
            if (string.IsNullOrEmpty(tag) || !hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag], tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignRace")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignRace
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData)
        {
            var hero = heroBuildingData.HeroCharacter;
            var tag = AttributeDefinitions.TagRace;

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag], tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastClassLevel
    {
        internal static void Prefix(RulesetCharacterHero hero)
        {
            var heroBuildingData = hero.GetOrCreateHeroBuildingData();
            var classDefinition = hero.ClassesHistory[heroBuildingData.HeroCharacter.ClassesHistory.Count - 1];
            var classesAndLevel = hero.ClassesAndLevels[classDefinition];
            var tag = AttributeDefinitions.GetClassTag(classDefinition, classesAndLevel);

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag], tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignBackground")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignBackground
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData)
        {
            var hero = heroBuildingData.HeroCharacter;
            var tag = AttributeDefinitions.TagBackground;

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag], tag);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastSubclass")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastSubclass
    {
        internal static void Prefix(RulesetCharacterHero hero, bool onlyIfCurrentLevel = false)
        {
            var classDefinition = hero.ClassesHistory[hero.ClassesHistory.Count - 1];
            var classLevel = hero.ClassesAndLevels[classDefinition];
            var level = 0;

            if (onlyIfCurrentLevel)
            {
                classDefinition.TryGetSubclassFeature(out _, out level);
            }

            if (!hero.ClassesAndSubclasses.ContainsKey(classDefinition) || onlyIfCurrentLevel && classLevel > level)
            {
                return;
            }

            var classesAndSubclass = hero.ClassesAndSubclasses[classDefinition];
            var tag = AttributeDefinitions.GetSubclassTag(classDefinition, classLevel, classesAndSubclass);

            if (!hero.ActiveFeatures.ContainsKey(tag))
            {
                return;
            }

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, hero.ActiveFeatures[tag], tag);
        }
    }
}
