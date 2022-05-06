using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterBuildingManagerPatcher
    {
        // captures the desired class and ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
        internal static class CharacterBuildingManagerAssignClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
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
        internal static class CharacterBuildingManagerUnassignLastClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                if (isLevelingUp && !isClassSelectionStage)
                {
                    LevelUpContext.UngrantItemsIfRequired(hero);
                }

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
        internal static class CharacterBuildingManagerGrantFeatures
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures the level up process only presents / offers spells from current class
        [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
        internal static class CharacterBuildingManagerEnumerateKnownAndAcquiredSpells
        {
            internal static void Postfix(
                CharacterHeroBuildingData heroBuildingData,
                List<SpellDefinition> __result)
            {
                var hero = heroBuildingData.HeroCharacter;
                var isMulticlass = LevelUpContext.IsMulticlass(hero);

                if (!isMulticlass)
                {
                    return;
                }

                var allowedSpells = LevelUpContext.GetAllowedSpells(hero);

                if (Main.Settings.EnableRelearnSpells)
                {
                    var selectedRepertoire = LevelUpContext.GetSelectedClassOrSubclassRepertoire(hero);

                    // only happens during character creation
                    if (selectedRepertoire == null)
                    {
                        __result.Clear();
                    }
                    // allow relearn any other spells but the ones learned by this repertoire
                    else
                    {
                        var knownSpells = new List<SpellDefinition>();

                        knownSpells.AddRange(selectedRepertoire.AutoPreparedSpells);
                        knownSpells.AddRange(selectedRepertoire.KnownCantrips);
                        knownSpells.AddRange(selectedRepertoire.KnownSpells);
                        knownSpells.AddRange(selectedRepertoire.EnumerateAvailableScribedSpells());

                        __result.SetRange(knownSpells);
                    }
                }
                else
                {
                    __result.RemoveAll(x => !allowedSpells.Contains(x));
                }
            }
        }

        // get the correct spell feature for the selected class
        [HarmonyPatch(typeof(CharacterBuildingManager), "GetSpellFeature")]
        internal static class CharacterBuildingManagerGetSpellFeature
        {
            internal static bool Prefix(
                CharacterHeroBuildingData heroBuildingData,
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
        internal static class CharacterBuildingManagerUpgradeSpellPointPools
        {
            internal static bool Prefix(
                CharacterBuildingManager __instance,
                CharacterHeroBuildingData heroBuildingData)
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

                foreach (RulesetSpellRepertoire spellRepertoire in hero.SpellRepertoires)
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
}
