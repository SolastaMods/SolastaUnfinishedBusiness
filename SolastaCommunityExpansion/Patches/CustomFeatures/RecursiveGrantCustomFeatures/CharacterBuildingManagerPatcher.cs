using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.RecursiveGrantCustomFeatures
{
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        /**
         * When a character is being granted features, this patch will apply the effect of custom features.
         */
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
