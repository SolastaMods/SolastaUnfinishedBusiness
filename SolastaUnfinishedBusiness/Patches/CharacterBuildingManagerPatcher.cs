using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionCastSpell;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterBuildingManagerPatcher
{
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.CreateNewCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CreateNewCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] CharacterBuildingManager __instance)
        {
            //PATCH: registers the hero getting created
            LevelUpContext.RegisterHero(__instance.CurrentLocalHeroCharacter, false);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.TrainInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TrainInvocation_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            InvocationDefinition invocation,
            ref bool checkPool)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is InvocationDefinitionCustom)
            {
                checkPool = false;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UnlearnInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnlearnInvocation_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            InvocationDefinition invocation,
            ref bool checkPool)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is InvocationDefinitionCustom)
            {
                checkPool = false;
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UntrainInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UntrainInvocation_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CharacterHeroBuildingData heroBuildingData,
            InvocationDefinition invocation,
            string tag)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is not InvocationDefinitionCustom)
            {
                return true;
            }

            if (heroBuildingData.LevelupTrainedInvocations.TryGetValue(tag, out var value))
            {
                value.Remove(invocation);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UndoUnlearnInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UndoUnlearnInvocation_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CharacterHeroBuildingData heroBuildingData,
            InvocationDefinition invocation,
            string tag)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is not InvocationDefinitionCustom)
            {
                return true;
            }

            if (heroBuildingData.UnlearnedInvocations.TryGetValue(tag, out var value))
            {
                value.Remove(invocation);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.LevelUpCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LevelUpCharacter_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] RulesetCharacterHero hero, ref bool force)
        {
            //PATCH: forces no experience on level up setting
            if (Main.Settings.NoExperienceOnLevelUp)
            {
                force = true;
            }

            //PATCH: registers the hero leveling up
            LevelUpContext.RegisterHero(hero, true);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.FinalizeCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FinalizeCharacter_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
        {
            //PATCH: grants race features
            LevelUpContext.GrantRaceFeatures(__instance, hero);

            //PATCH: grants repertoires and cantrips from backgrounds
            if (hero.ClassesHistory.Count == 1)
            {
                foreach (var featureDefinitionCastSpell in hero.BackgroundDefinition.Features
                             .OfType<FeatureDefinitionCastSpell>())
                {
                    hero.GrantSpellRepertoire(featureDefinitionCastSpell, null, null, hero.RaceDefinition);

                    var buildingData = hero.GetHeroBuildingData();

                    __instance.GrantCantripsAndSpellsByTag(buildingData, AttributeDefinitions.TagBackground,
                        featureDefinitionCastSpell);
                }

                //PATCH: grants the power spell points to any created hero including pre-gen ones (SPELL_POINTS)
                SpellPointsContext.GrantPowerSpellPoints(hero);
            }

            //PATCH: grants custom features
            LevelUpContext.GrantCustomFeaturesFromFeats(hero);
            LevelUpContext.GrantCustomFeatures(hero);
        }

        [UsedImplicitly]
        public static void Postfix(CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
        {
            //PATCH: grants cantrip selected by a Domain Nature on level 1
            DomainNature.GrantCantrip(hero);

            //PATCH: grants spell repertoires and respective selected spells from feats
            LevelUpContext.GrantSpellsOrCantripsFromFeatCastSpell(__instance, hero);

            //PATCH: keeps spell repertoires sorted by class title but ancestry one is always kept first
            LevelUpContext.SortHeroRepertoires(hero);

            //PATCH: adds whole list caster spells to KnownSpells collection to improve the MC spell selection UI
            // LevelUpContext.UpdateKnownSpellsForWholeCasters(hero);

            //PATCH: unregisters the hero leveling up
            LevelUpContext.UnregisterHero(hero);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.AssignClassLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AssignClassLevel_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero, CharacterClassDefinition classDefinition)
        {
            //PATCH: captures the desired class
            LevelUpContext.SetSelectedClass(hero, classDefinition);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            if (result)
            {
                //PATCH: grants items for new class if required
                LevelUpContext.GrantItemsIfRequired(hero);
            }

            return !result;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.AssignSubclass))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AssignSubclass_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] RulesetCharacterHero hero, CharacterSubclassDefinition subclassDefinition)
        {
            //PATCH: captures the desired sub class
            LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.GrantFeatures))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GrantFeatures_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero)
        {
            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

            return !(isLevelingUp && isClassSelectionStage);
        }

        [UsedImplicitly]
        public static void Postfix(
            RulesetCharacterHero hero,
            List<FeatureDefinition> grantedFeatures,
            string tag)
        {
            //PATCH: support for `FeatureDefinitionGrantCustomInvocations`
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            //PATCH: fixes the particular case where we get double invocation pools if hero is MC
            var heroBuildingData = hero.GetHeroBuildingData();

            if (heroBuildingData.PointPoolStacks
                    .TryGetValue(HeroDefinitions.PointsPoolType.Invocation, out var pointPoolStack) &&
                hero.ClassesAndLevels
                    .TryGetValue(DatabaseHelper.CharacterClassDefinitions.Warlock, out var levels))
            {
                var goodTag =
                    AttributeDefinitions.GetClassTag(DatabaseHelper.CharacterClassDefinitions.Warlock, levels);

                foreach (var badKey in pointPoolStack.ActivePools.Keys.Where(x => x != goodTag).ToList())
                {
                    pointPoolStack.ActivePools.Remove(badKey);
                }
            }

            FeatureDefinitionGrantInvocations.GrantInvocations(hero, tag, grantedFeatures);
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var getInvocationProficiencies = typeof(RulesetCharacterHero).GetMethod("get_InvocationProficiencies");
            var customInvocationsProficiencies =
                new Func<RulesetCharacterHero, List<string>>(CustomInvocationSubPanel
                        .OnlyStandardInvocationProficiencies)
                    .Method;

            return instructions
                //PATCH: don't offer invocations unlearn on non Warlock classes (MULTICLASS)
                .ReplaceCalls(getInvocationProficiencies,
                    "CharacterBuildingManager.GrantFeatures",
                    new CodeInstruction(OpCodes.Call, customInvocationsProficiencies));
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.RemoveActiveFeaturesFromHeroByTag))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveActiveFeaturesFromHeroByTag_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacterHero hero, string tag)
        {
            //PATCH: support for `FeatureDefinitionGrantCustomInvocations`
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            if (hero.ActiveFeatures.TryGetValue(tag, out var features))
            {
                FeatureDefinitionGrantInvocations.RemoveInvocations(hero, tag, features);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.ClearPrevious))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ClearPrevious_Patch
    {
        [UsedImplicitly]
        public static void Prefix(RulesetCharacterHero hero, string tag)
        {
            //PATCH: support for `FeatureDefinitionGrantCustomInvocations`
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            if (hero.ActiveFeatures.TryGetValue(tag, out var features))
            {
                FeatureDefinitionGrantInvocations.RemoveInvocations(hero, tag, features);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UnassignLastClassLevel))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnassignLastClassLevel_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero)
        {
            //PATCH: un-captures the desired class
            LevelUpContext.SetSelectedClass(hero, null);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            if (result)
            {
                //PATCH: removes items from new class if required
                LevelUpContext.RemoveItemsIfRequired(hero);
            }

            return !result;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UnassignLastSubclass))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnassignLastSubclass_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero)
        {
            //PATCH: un-captures the desired subclass
            LevelUpContext.SetSelectedSubclass(hero, null);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            return !result;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UntrainLastFightingStyle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UntrainLastFightingStyle_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero)
        {
            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            return !result;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.EnumerateKnownAndAcquiredSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumerateKnownAndAcquiredSpells_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] CharacterHeroBuildingData heroBuildingData,
            List<SpellDefinition> __result)
        {
            //PATCH: ensures the level up process only presents / offers spells from current class
            LevelUpContext.EnumerateKnownAndAcquiredSpells(heroBuildingData, __result);
        }
    }

    //PATCH: gets the correct spell feature for the selected class
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.GetSpellFeature))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetSpellFeature_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            [NotNull] CharacterHeroBuildingData heroBuildingData,
            string tag,
            ref FeatureDefinitionCastSpell __result)
        {
            var hero = heroBuildingData.HeroCharacter;

            //PATCH: support cast spell granted from feat
            foreach (var featureDefinitionCastSpell in heroBuildingData.levelupTrainedFeats.SelectMany(x =>
                         x.Value.SelectMany(y => y.Features).OfType<FeatureDefinitionCastSpell>()))
            {
                var spellTag = featureDefinitionCastSpell.GetFirstSubFeatureOfType<FeatHelpers.SpellTag>();

                if (spellTag == null || !tag.EndsWith(spellTag.Name))
                {
                    continue;
                }

                __result = featureDefinitionCastSpell;

                return false;
            }

            var isMulticlass = LevelUpContext.IsMulticlass(hero);
            if (!isMulticlass)
            {
                return true;
            }

            var selectedClass = LevelUpContext.GetSelectedClass(hero);

            if (!selectedClass)
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

    //PATCH: ensures the level up process don't get stuck if race uses fixed list and hero is a caster on level 1
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.SetupSpellPointPools))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetupSpellPointPools_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            FeatureDefinitionCastSpell featureDefinitionCastSpell,
            string tag)
        {
            heroBuildingData.TempAcquiredCantripsNumber = 0;
            heroBuildingData.TempAcquiredSpellsNumber = 0;
            heroBuildingData.TempUnlearnedSpellsNumber = 0;
            heroBuildingData.TempAcquiredAnyCantripOrSpellNumber = 0;

            __instance.ApplyFeatureCastSpell(heroBuildingData, featureDefinitionCastSpell);

            // this IF is only difference from original game code (in original block is always executed)
            if (tag != AttributeDefinitions.TagRace ||
                featureDefinitionCastSpell.SpellKnowledge != SpellKnowledge.FixedList)
            {
                __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip, tag,
                    heroBuildingData.TempAcquiredCantripsNumber);
                __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.Spell, tag,
                    heroBuildingData.TempAcquiredSpellsNumber);
                __instance.SetPointPool(heroBuildingData, HeroDefinitions.PointsPoolType.CantripOrSpell, tag,
                    heroBuildingData.TempAcquiredAnyCantripOrSpellNumber);
            }

            if (heroBuildingData.HeroCharacter.ActiveFeatures.TryGetValue(tag, out var value))
            {
                heroBuildingData.HeroCharacter.BrowseFeaturesOfType<FeatureDefinitionCastSpell>(
                    value,
                    (feature, s) => __instance.LearnFixedSpells(heroBuildingData, feature, s), tag);
            }

            return false;
        }
    }

    //PATCH: ensures the level up process only offers slots from the leveling up class
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UpgradeSpellPointPools))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpgradeSpellPointPools_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            CharacterBuildingManager __instance,
            [NotNull] CharacterHeroBuildingData heroBuildingData)
        {
            var hero = heroBuildingData.HeroCharacter;
            var selectedClass = LevelUpContext.GetSelectedClass(hero);
            var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);
            var selectedClassLevel = LevelUpContext.GetSelectedClassLevel(hero);

            // we filter out any repertoire that was granted from feats
            foreach (var spellRepertoire in hero.SpellRepertoires
                         .Where(x => !x.SpellCastingFeature.HasSubFeatureOfType<FeatHelpers.SpellTag>()))
            {
                var poolName = string.Empty;
                var maxPoints = 0;

                switch (spellRepertoire.SpellCastingFeature.SpellCastingOrigin)
                {
                    // short circuit if the feature is for another class (change from native code)
                    case CastingOrigin.Class when spellRepertoire.SpellCastingClass != selectedClass:
                        continue;
                    case CastingOrigin.Class:
                        poolName = AttributeDefinitions.GetClassTag(selectedClass, selectedClassLevel);
                        break;
                    // short circuit if the feature is for another subclass (change from native code)
                    case CastingOrigin.Subclass when spellRepertoire.SpellCastingSubclass != selectedSubclass:
                        continue;
                    case CastingOrigin.Subclass:
                        poolName = AttributeDefinitions.GetSubclassTag(
                            selectedClass, selectedClassLevel, selectedSubclass);
                        break;
                    case CastingOrigin.Race:
                        poolName = AttributeDefinitions.TagRace;
                        break;
                    case CastingOrigin.Monster:
                        break;
                    default:
                        throw new ArgumentException("spellRepertoire.SpellCastingFeature.SpellCastingOrigin");
                }

                if (__instance.HasAnyActivePoolOfType(heroBuildingData, HeroDefinitions.PointsPoolType.Cantrip)
                    && heroBuildingData.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools
                        .TryGetValue(poolName, out var pointPool))
                {
                    maxPoints = pointPool.MaxPoints;
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

                if (heroBuildingData.HeroCharacter.ActiveFeatures.TryGetValue(poolName, out var value))
                {
                    heroBuildingData.HeroCharacter.BrowseFeaturesOfType<FeatureDefinitionCastSpell>(
                        value,
                        (feature, s) => __instance.LearnFixedSpells(heroBuildingData, feature, s), poolName);
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.IsFeatMatchingPrerequisites))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsFeatMatchingPrerequisites_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            CharacterHeroBuildingData heroBuildingData,
            FeatDefinition feat,
            ref bool isSameFamilyPrerequisite)
        {
            //PATCH: fixes being able to select feats from same family when more than 1 feat selection is possible aat same time
            //vanilla code doesn't check if we already have selected feats from same family
            if (!__result || !feat.HasFamilyTag || string.IsNullOrEmpty(feat.FamilyTag))
            {
                return;
            }

            if (!heroBuildingData.levelupTrainedFeats.Any(pair =>
                    pair.Value.Any(f => f.HasFamilyTag && f.FamilyTag == feat.FamilyTag)))
            {
                return;
            }

            __result = false;
            isSameFamilyPrerequisite = true;
        }
    }

    //PATCH: considers subclass morphotype preferences
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.AssignDefaultMorphotypes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class AssignDefaultMorphotypes_Patch
    {
        private static RangedInt PreferedSkinColors(
            RacePresentation racePresentation,
            [NotNull] CharacterHeroBuildingData heroBuildingData)
        {
            var subRaceDefinition = heroBuildingData.HeroCharacter.SubRaceDefinition;

            return subRaceDefinition
                ? subRaceDefinition.RacePresentation.PreferedSkinColors
                : racePresentation.PreferedSkinColors;
        }

        private static RangedInt PreferedHairColors(
            RacePresentation racePresentation,
            [NotNull] CharacterHeroBuildingData heroBuildingData)
        {
            var subRaceDefinition = heroBuildingData.HeroCharacter.SubRaceDefinition;

            return subRaceDefinition
                ? subRaceDefinition.RacePresentation.PreferedHairColors
                : racePresentation.PreferedHairColors;
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var preferedSkinColorsMethod = typeof(RacePresentation).GetMethod("get_PreferedSkinColors");
            var preferedHairColorsColorsMethod = typeof(RacePresentation).GetMethod("get_PreferedHairColors");
            var myPreferedSkinColorsMethod =
                new Func<RacePresentation, CharacterHeroBuildingData, RangedInt>(PreferedSkinColors).Method;
            var myPreferedHairColorsColorsMethod =
                new Func<RacePresentation, CharacterHeroBuildingData, RangedInt>(PreferedHairColors).Method;

            return instructions
                .ReplaceCalls(preferedSkinColorsMethod,
                    "CharacterBuildingManager.AssignDefaultMorphotypes.PreferedSkinColors",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, myPreferedSkinColorsMethod))
                .ReplaceCalls(preferedHairColorsColorsMethod,
                    "CharacterBuildingManager.AssignDefaultMorphotypes.PreferedHairColors",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, myPreferedHairColorsColorsMethod));
        }
    }

    //PATCH: apply point pools assigned from feats
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.TrainFeat))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TrainFeat_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            FeatDefinition feat)
        {
            foreach (var featureDefinitionPointPool in feat.Features.OfType<FeatureDefinitionPointPool>())
            {
                if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                        out var pointPoolStack))
                {
                    continue;
                }

                var hero = __instance.CurrentLocalHeroCharacter;

                __instance.GetLastAssignedClassAndLevel(hero, out var classDefinition, out var level);

                var finaTag = AttributeDefinitions.GetClassTag(classDefinition, level) +
                              featureDefinitionPointPool.ExtraSpellsTag;

                if (pointPoolStack.ActivePools
                    .TryGetValue(finaTag + featureDefinitionPointPool.ExtraSpellsTag, out var pool))
                {
                    pool.maxPoints += featureDefinitionPointPool.poolAmount;
                }
                else
                {
                    __instance.ApplyFeatureDefinitionPointPool(heroBuildingData, featureDefinitionPointPool, finaTag);
                }
            }

            LevelUpContext.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
        }
    }

    //PATCH: remove point pools assigned from feats
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UntrainFeat))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UntrainFeat_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            FeatDefinition feat)
        {
            foreach (var featureDefinitionPointPool in feat.Features.OfType<FeatureDefinitionPointPool>())
            {
                if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                        out var pointPoolStack))
                {
                    continue;
                }

                var hero = __instance.CurrentLocalHeroCharacter;

                __instance.GetLastAssignedClassAndLevel(hero, out var classDefinition, out var level);

                var finaTag = AttributeDefinitions.GetClassTag(classDefinition, level) +
                              featureDefinitionPointPool.ExtraSpellsTag + featureDefinitionPointPool.ExtraSpellsTag;

                if (!pointPoolStack.ActivePools.TryGetValue(finaTag, out var pool))
                {
                    continue;
                }

                pool.maxPoints -= featureDefinitionPointPool.poolAmount;

                if (pool.maxPoints == 0)
                {
                    pointPoolStack.ActivePools.Remove(finaTag);
                }
            }

            LevelUpContext.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
        }
    }

    //PATCH: remove point pools assigned from feats
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UntrainFeats))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UntrainFeats_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData)
        {
            foreach (var featureDefinitionPointPool in heroBuildingData.LevelupTrainedFeats
                         .SelectMany(x => x.Value
                             .SelectMany(y => y.Features))
                         .OfType<FeatureDefinitionPointPool>())
            {
                if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                        out var pointPoolStack))
                {
                    continue;
                }

                var hero = __instance.CurrentLocalHeroCharacter;

                __instance.GetLastAssignedClassAndLevel(hero, out var classDefinition, out var level);

                var finaTag = AttributeDefinitions.GetClassTag(classDefinition, level) +
                              featureDefinitionPointPool.ExtraSpellsTag + featureDefinitionPointPool.ExtraSpellsTag;

                if (!pointPoolStack.ActivePools.TryGetValue(finaTag, out var pool))
                {
                    continue;
                }

                pool.maxPoints -= featureDefinitionPointPool.poolAmount;

                if (pool.maxPoints == 0)
                {
                    pointPoolStack.ActivePools.Remove(finaTag);
                }
            }

            LevelUpContext.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
        }
    }
}
