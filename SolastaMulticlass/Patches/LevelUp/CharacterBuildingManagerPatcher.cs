using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterBuildingManagerPatcher
    {
        //
        // context registration / unregistration patches
        //

        // register the hero leveling up
        [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
        internal static class CharacterBuildingManagerLevelUpCharacter
        {
            internal static void Prefix(RulesetCharacterHero hero)
            {
                LevelUpContext.RegisterHero(hero);
            }
        }

        // unregister the hero leveling up
        [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
        internal static class CharacterBuildingManagerFinalizeCharacter
        {
            internal static void Postfix(RulesetCharacterHero hero)
            {
                LevelUpContext.UnregisterHero(hero);
            }
        }

        // captures the desired class and ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
        internal static class CharacterBuildingManagerAssignClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
            {
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                if (isLevelingUp)
                {
                    LevelUpContext.SetSelectedClass(hero, classDefinition);

                    if (!isClassSelectionStage)
                    {
                        LevelUpContext.GrantItemsIfRequired(hero);
                    }
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

        // captures the desired sub class
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
        internal static class CharacterBuildingManagerAssignSubclass
        {
            internal static void Prefix(RulesetCharacterHero hero, CharacterSubclassDefinition subclassDefinition)
            {
                LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
            }
        }

        //
        // don't grant features on class selection stage during level up
        //

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

        //
        // SPELLS
        //

        // ensures the level up process only presents / offers spells based on all different mod settings
        [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
        internal static class CharacterBuildingManagerEnumerateKnownAndAcquiredSpells
        {
            internal static bool Prefix(
                CharacterHeroBuildingData heroBuildingData,
                string tagToIgnore,
                ref List<SpellDefinition> __result)
            {
                var hero = heroBuildingData.HeroCharacter;
                var isMulticlass = LevelUpContext.IsMulticlass(hero);

                if (!isMulticlass)
                {
                    return true;
                }

                var selectedClass = LevelUpContext.GetSelectedClass(hero);
                var spellDefinitionList = new List<SpellDefinition>();

                heroBuildingData.MatchingFeatures.Clear();

                foreach (var spellRepertoire in hero.SpellRepertoires)
                {
                    var isRepertoireFromSelectedClassSubclass = LevelUpContext.IsRepertoireFromSelectedClassSubclass(hero, spellRepertoire);

                    // PATCH: don't allow cantrips to be re-learned
                    foreach (var spell in spellRepertoire.KnownCantrips)
                    {
                        if (isRepertoireFromSelectedClassSubclass
                            || (!Main.Settings.EnableRelearnSpells && LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, spell)))
                        {
                            spellDefinitionList.TryAdd(spell);
                        }
                    }

                    // PATCH: don't allow spells to be re-learned
                    foreach (var spell in spellRepertoire.KnownSpells)
                    {
                        if (isRepertoireFromSelectedClassSubclass
                            || (!Main.Settings.EnableRelearnSpells && LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, spell)))
                        {
                            spellDefinitionList.TryAdd(spell);
                        }
                    }

                    //
                    // this if wasn't in original code
                    //

                    // PATCH: don't allow spells from whole lists to be re-learned
                    if (spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.WholeList)
                    {
                        var classSpellLevel = spellRepertoire.MaxSpellLevelOfSpellCastingLevel;

                        for (var spellLevel = 1; spellLevel <= classSpellLevel; spellLevel++)
                        {
                            foreach (var spell in spellRepertoire.SpellCastingFeature.SpellListDefinition.GetSpellsOfLevel(spellLevel))
                            {
                                if (isRepertoireFromSelectedClassSubclass
                                    || (!Main.Settings.EnableRelearnSpells && LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, spell)))
                                {
                                    spellDefinitionList.TryAdd(spell);
                                }
                            }
                        }
                    }
                }

                //
                // this is the modified code from EnumerateAvailableScribedSpells()
                //

                // PATCH: don't allow scribed spells to be re-learned
                var foundSpellbooks = new List<RulesetItemSpellbook>();

                hero.CharacterInventory.BrowseAllCarriedItems(foundSpellbooks);
                foreach (var foundSpellbook in foundSpellbooks)
                {
                    foreach (var spell in foundSpellbook.ScribedSpells)
                    {
                        if (selectedClass == Wizard
                            || (!Main.Settings.EnableRelearnSpells && LevelUpContext.IsSpellOfferedBySelectedClassSubclass(hero, spell)))
                        {
                            spellDefinitionList.TryAdd(spell);
                        }
                    }
                }

                // GAME CODE FROM HERE

                foreach (var bonusCantrip in heroBuildingData.BonusCantrips
                    .Where(x => x.Key != tagToIgnore))
                {
                    foreach (var spellDefinition in bonusCantrip.Value)
                    {
                        spellDefinitionList.TryAdd(spellDefinition);
                    }
                }

                foreach (var acquiredCantrip in heroBuildingData.AcquiredCantrips
                    .Where(x => x.Key != tagToIgnore))
                {
                    foreach (var spellDefinition in acquiredCantrip.Value)
                    {
                        spellDefinitionList.TryAdd(spellDefinition);
                    }
                }

                foreach (var acquiredSpell in heroBuildingData.AcquiredSpells
                    .Where(x => x.Key != tagToIgnore))
                {
                    foreach (var spellDefinition in acquiredSpell.Value)
                    {
                        spellDefinitionList.TryAdd(spellDefinition);
                    }
                }

                __result = spellDefinitionList;

                return false;
            }
        }

        // removes any levels from the tag otherwise the spell offering panel gets lost and produces a null exception
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

        // ensures the level up process only offers slots from the leveling up class
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
