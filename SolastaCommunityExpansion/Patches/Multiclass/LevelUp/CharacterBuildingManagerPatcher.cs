using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // captures the desired class and ensures this doesn't get executed in the class panel level up screen
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignClassLevel
    {
        internal static bool Prefix(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            LevelUpContext.SetSelectedClass(hero, classDefinition);

            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            if (isLevelingUp && !isClassSelectionStage)
            {
                LevelUpContext.GrantItemsIfRequired(hero);
            }

            return !(isLevelingUp && isClassSelectionStage);
        }
    }

    // ensures this doesn't get executed in the class panel level up screen
    [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnassignLastClassLevel
    {
        internal static bool Prefix(RulesetCharacterHero hero)
        {
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

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

            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

            if (isLevelingUp && !isClassSelectionStage)
            {
                LevelUpContext.UngrantItemsIfRequired(hero);
            }

            return !(isLevelingUp && isClassSelectionStage);
        }
    }

    // ensures this doesn't get executed in the class panel level up screen
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static bool Prefix(RulesetCharacterHero hero)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            return !(isLevelingUp && isClassSelectionStage);
        }
    }

    // ensures the level up process only presents / offers spells from current class
    [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_EnumerateKnownAndAcquiredSpells
    {
        internal static void Postfix(
            CharacterHeroBuildingData heroBuildingData,
            List<SpellDefinition> __result)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return;
            }

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

    // get the correct spell feature for the selected class
    [HarmonyPatch(typeof(CharacterBuildingManager), "GetSpellFeature")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GetSpellFeature
    {
        internal static bool Prefix(
            CharacterHeroBuildingData heroBuildingData,
            string tag,
            ref FeatureDefinitionCastSpell __result)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

            var hero = heroBuildingData.HeroCharacter;
            var isMulticlass = LevelUpContext.IsMulticlass(hero);

            if (!isMulticlass)
            {
                return true;
            }

            var selectedClass = LevelUpContext.GetSelectedClass(hero);
            var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);

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

    // ensure the level up process only offers slots from the leveling up class
    [HarmonyPatch(typeof(CharacterBuildingManager), "UpgradeSpellPointPools")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UpgradeSpellPointPools
    {
        internal static bool Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                return true;
            }

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

                if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                {
                    // PATCH: short circuit if the feature is for another class (change from native code)
                    if (spellRepertoire.SpellCastingClass != selectedClass)
                    {
                        continue;
                    }

                    poolName = AttributeDefinitions.GetClassTag(selectedClass, selectedClassLevel);
                }
                else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass)
                {
                    // PATCH: short circuit if the feature is for another subclass (change from native code)
                    if (spellRepertoire.SpellCastingSubclass != selectedSubclass)
                    {
                        continue;
                    }

                    poolName = AttributeDefinitions.GetSubclassTag(selectedClass, selectedClassLevel, selectedSubclass);
                }
                else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                {
                    poolName = AttributeDefinitions.TagRace;
                }

                if (__instance.HasAnyActivePoolOfType(heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip)
                    && heroBuildingData.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools.ContainsKey(poolName))
                {
                    maxPoints = heroBuildingData.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools[poolName].MaxPoints;
                }

                heroBuildingData.TempAcquiredCantripsNumber = 0;
                heroBuildingData.TempAcquiredSpellsNumber = 0;
                heroBuildingData.TempUnlearnedSpellsNumber = 0;

                var characterBuildingManagerType = typeof(CharacterBuildingManager);
                var applyFeatureCastSpellMethod = characterBuildingManagerType.GetMethod("ApplyFeatureCastSpell", BindingFlags.NonPublic | BindingFlags.Instance);
                var setPointPoolMethod = characterBuildingManagerType.GetMethod("SetPointPool", BindingFlags.NonPublic | BindingFlags.Instance);

                applyFeatureCastSpellMethod.Invoke(__instance, new object[] { heroBuildingData, spellRepertoire.SpellCastingFeature });

                setPointPoolMethod.Invoke(__instance, new object[] { heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip, poolName, heroBuildingData.TempAcquiredCantripsNumber + maxPoints });
                setPointPoolMethod.Invoke(__instance, new object[] { heroBuildingData, HeroDefinitions.PointsPoolType.Spell, poolName, heroBuildingData.TempAcquiredSpellsNumber });
                setPointPoolMethod.Invoke(__instance, new object[] { heroBuildingData, HeroDefinitions.PointsPoolType.SpellUnlearn, poolName, heroBuildingData.TempUnlearnedSpellsNumber });
            }

            return false;
        }
    }
}
