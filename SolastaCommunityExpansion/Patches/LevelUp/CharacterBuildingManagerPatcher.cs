using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Patches.LevelUp
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

            LevelUpContext.RegisterHero(hero, lastClass, lastSubclass, true);
        }
    }

    // sort spell repertoires, add all known spells to wholelist casters and unregister the hero leveling up
    [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_FinalizeCharacter
    {
        internal static void Postfix(RulesetCharacterHero hero)
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

                return title1.CompareTo(title2);
            });

            //
            // Add wholelist caster spells to KnownSpells collection to improve the MC spell selection UI
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

    // captures the desired class and ensures this doesn't get executed in the class panel level up screen
    [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_AssignClassLevel
    {
        internal static bool Prefix(RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
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

    // ensures this doesn't get executed under a specific MC scenario and only recursive grant features if not in that scenario
    [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_GrantFeatures
    {
        internal static bool Prefix(RulesetCharacterHero hero)
        {
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            return !(isLevelingUp && isClassSelectionStage);
        }

        internal static void Postfix(RulesetCharacterHero hero, List<FeatureDefinition> grantedFeatures, string tag)
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

    [HarmonyPatch(typeof(CharacterBuildingManager), "ClearPrevious")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_ClearPrevious
    {
        private static readonly List<FeatureDefinition> ToRemove = new();

        internal static void Prefix(RulesetCharacterHero hero, string tag)
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

            //TODO: check if other places where this is called require same prefx/postfix treatment
            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, ToRemove);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "UnacquireBonusCantrips")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_UnacquireBonusCantrips
    {
        internal static void Prefix(CharacterHeroBuildingData heroBuildingData, ref string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            tag = CustomFeaturesContext.UnCustomizeTag(tag);
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

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
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

            CustomFeaturesContext.RecursiveRemoveCustomFeatures(hero, tag, hero.ActiveFeatures[tag]);
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

            if (isLevelingUp && !isClassSelectionStage)
            {
                LevelUpContext.UngrantItemsIfRequired(hero);
            }

            return !(isLevelingUp && isClassSelectionStage);
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

    // ensures the level up process only presents / offers spells from current class
    [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_EnumerateKnownAndAcquiredSpells
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

                if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
                    CastingOrigin.Class)
                {
                    // PATCH: short circuit if the feature is for another class (change from native code)
                    if (spellRepertoire.SpellCastingClass != selectedClass)
                    {
                        continue;
                    }

                    poolName = AttributeDefinitions.GetClassTag(selectedClass, selectedClassLevel);
                }
                else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
                         CastingOrigin.Subclass)
                {
                    // PATCH: short circuit if the feature is for another subclass (change from native code)
                    if (spellRepertoire.SpellCastingSubclass != selectedSubclass)
                    {
                        continue;
                    }

                    poolName = AttributeDefinitions.GetSubclassTag(selectedClass, selectedClassLevel, selectedSubclass);
                }
                else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin ==
                         CastingOrigin.Race)
                {
                    poolName = AttributeDefinitions.TagRace;
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

    [HarmonyPatch(typeof(CharacterBuildingManager), "RegisterPoolStack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_RegisterPoolStack
    {
        internal static void Postfix(
            CharacterHeroBuildingData heroBuildingData,
            List<FeatureDefinition> features,
            string tag)
        {
            if (!Main.Settings.EnableCustomSpellsPatch)
            {
                return;
            }

            var hero = heroBuildingData.HeroCharacter;
            var poolMods = hero.GetFeaturesByType<IPointPoolMaxBonus>();
            var spellMods = new List<IPointPoolMaxBonus>();

            poolMods.RemoveAll(CustomFeaturesContext.IsSpellBonus);

            heroBuildingData.HeroCharacter.BrowseFeaturesOfType<FeatureDefinition>(features, (feature, _) =>
            {
                if (feature is IPointPoolMaxBonus bonus)
                {
                    poolMods.Remove(bonus);
                    if (CustomFeaturesContext.IsSpellBonus(bonus))
                    {
                        spellMods.Add(bonus);
                    }
                }
            }, tag);

            var values = new Dictionary<HeroDefinitions.PointsPoolType, int>();

            foreach (var mod in poolMods)
            {
                values.AddOrReplace(mod.PoolType, values.GetValueOrDefault(mod.PoolType) + mod.MaxPointsBonus);
            }

            //Remove spell/cantrip pool modifiers gained on this level
            foreach (var mod in spellMods)
            {
                values.AddOrReplace(mod.PoolType, values.GetValueOrDefault(mod.PoolType) - mod.MaxPointsBonus);
            }

            foreach (var mod in values)
            {
                var poolType = mod.Key;
                var value = mod.Value;

                var poolStack = heroBuildingData.PointPoolStacks[poolType] ?? new PointPoolStack(poolType);

                var pool = poolStack.ActivePools.GetValueOrDefault(tag);

                if (pool != null)
                {
                    pool.MaxPoints += value;
                    pool.RemainingPoints += value;
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), "ApplyFeatureCastSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_ApplyFeatureCastSpell
    {
        internal static void Postfix(CharacterHeroBuildingData heroBuildingData,
            FeatureDefinition feature)
        {
            if (feature is not FeatureDefinitionCastSpell spellCasting) { return; }

            var castingOrigin = spellCasting.SpellCastingOrigin;
            if (castingOrigin != CastingOrigin.Class && castingOrigin != CastingOrigin.Subclass)
            {
                return;
            }

            var hero = heroBuildingData.HeroCharacter;

            ServiceRepository.GetService<ICharacterBuildingService>()
                .GetLastAssignedClassAndLevel(hero, out var gainedClass, out _);

            var poolMods = hero.GetFeaturesByTypeAndTag<IPointPoolMaxBonus>(gainedClass.Name);

            poolMods.RemoveAll(p => !CustomFeaturesContext.IsSpellBonus(p));

            foreach (var mod in poolMods)
            {
                if (mod.PoolType == HeroDefinitions.PointsPoolType.Cantrip)
                {
                    heroBuildingData.TempAcquiredCantripsNumber += mod.MaxPointsBonus;
                }
                else if (mod.PoolType == HeroDefinitions.PointsPoolType.Spell)
                {
                    heroBuildingData.TempAcquiredSpellsNumber += mod.MaxPointsBonus;
                }
            }

            //
            // FIX an original TA bug not considering bonus cantrips from subclasses on this calculation
            //
            if (Main.Settings.BugFixCorrectlyAssignBonusCantrips)
            {
                for (var i = 0; i < hero.ActiveFeatures.Count - 1; i++)
                {
                    var freeBonusCantripsFromSubclasses = hero.ActiveFeatures.ElementAt(i).Value
                        .OfType<FeatureDefinitionPointPool>()
                        .Where(x => x.PoolType == HeroDefinitions.PointsPoolType.Cantrip)
                        .Sum(x => x.PoolAmount);

                    var fixedBonusCantripsFromSubclasses = hero.ActiveFeatures.ElementAt(i).Value
                        .OfType<FeatureDefinitionBonusCantrips>()
                        .Sum(x => x.BonusCantrips.Count);

                    heroBuildingData.TempAcquiredCantripsNumber +=
                        freeBonusCantripsFromSubclasses + fixedBonusCantripsFromSubclasses;
                }
            }
        }
    }
}
