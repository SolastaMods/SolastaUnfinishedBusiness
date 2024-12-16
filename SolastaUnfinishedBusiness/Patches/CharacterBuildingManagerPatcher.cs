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
using SolastaUnfinishedBusiness.FightingStyles;
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
            LevelUpHelper.RegisterHero(__instance.CurrentLocalHeroCharacter, false);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.TrainInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TrainInvocation_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            InvocationDefinition invocation,
            ref bool checkPool)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is InvocationDefinitionCustom)
            {
                checkPool = false;
            }

            if (invocation.GrantedFeature is not FeatureDefinitionPointPool featureDefinitionPointPool)
            {
                return;
            }

            if (!heroBuildingData.PointPoolStacks.TryGetValue(
                    featureDefinitionPointPool.PoolType, out var pointPoolStack))
            {
                return;
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
        private static void UndoGrantPool(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            InvocationDefinition invocation)
        {
            if (invocation.GrantedFeature is not FeatureDefinitionPointPool featureDefinitionPointPool)
            {
                return;
            }

            if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                    out var pointPoolStack))
            {
                return;
            }

            var hero = __instance.CurrentLocalHeroCharacter;

            __instance.GetLastAssignedClassAndLevel(hero, out var classDefinition, out var level);

            var finaTag = AttributeDefinitions.GetClassTag(classDefinition, level) +
                          featureDefinitionPointPool.ExtraSpellsTag + featureDefinitionPointPool.ExtraSpellsTag;

            if (!pointPoolStack.ActivePools.TryGetValue(finaTag, out var pool))
            {
                return;
            }

            pool.maxPoints -= featureDefinitionPointPool.poolAmount;

            if (pool.maxPoints == 0)
            {
                pointPoolStack.ActivePools.Remove(finaTag);
            }
        }

        [UsedImplicitly]
        public static bool Prefix(
            CharacterBuildingManager __instance,
            CharacterHeroBuildingData heroBuildingData,
            InvocationDefinition invocation,
            string tag)
        {
            //PATCH: do not check or modify point pools when dealing with custom invocations
            if (invocation is not InvocationDefinitionCustom)
            {
                UndoGrantPool(__instance, heroBuildingData, invocation);

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
            LevelUpHelper.RegisterHero(hero, true);
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.FinalizeCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FinalizeCharacter_Patch
    {
        private static void GrantCantripFromFightingStyle(
            CharacterBuildingManager characterBuildingManager,
            RulesetCharacterHero hero,
            FeatureDefinitionCastSpell featureDefinitionCastSpell)
        {
            var spellTag = featureDefinitionCastSpell.GetFirstSubFeatureOfType<FeatHelpers.SpellTag>();

            if (spellTag == null)
            {
                return;
            }

            characterBuildingManager.GetLastAssignedClassAndLevel(hero, out var classDefinition, out var level);

            var classTag = AttributeDefinitions.GetClassTag(classDefinition, level);
            var tag = spellTag.Name;
            var finalTag = classTag + tag + tag;
            var heroBuildingData = hero.GetHeroBuildingData();

            // grant cantrips from selection or fixed list
            if (!heroBuildingData.AcquiredCantrips.TryGetValue(finalTag, out var cantrips))
            {
                return;
            }

            foreach (var cantrip in cantrips)
            {
                hero.GrantCantrip(cantrip, featureDefinitionCastSpell);
            }
        }

        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
        {
            var buildingData = hero.GetHeroBuildingData();

            //PATCH: grants race features
            LevelUpHelper.GrantRaceFeatures(__instance, hero);

            //PATCH: grants repertoires and cantrips from backgrounds
            if (hero.ClassesHistory.Count == 1)
            {
                foreach (var featureDefinitionCastSpell in hero.BackgroundDefinition.Features
                             .OfType<FeatureDefinitionCastSpell>())
                {
                    hero.GrantSpellRepertoire(featureDefinitionCastSpell, null, null, hero.RaceDefinition);

                    __instance.GrantCantripsAndSpellsByTag(
                        buildingData, AttributeDefinitions.TagBackground, featureDefinitionCastSpell);
                }

                //PATCH: grants the power spell points to any created hero including pre-gen ones (SPELL_POINTS)
                SpellPointsContext.GrantPowerSpellPoints(hero);
            }

            //PATCH: grants repertoire and selected cantrips from Blessed Warrior if not there yet
            if (hero.TrainedFightingStyles.Any(x => x.Name == BlessedWarrior.Name) &&
                hero.SpellRepertoires.All(x => x.spellCastingFeature != BlessedWarrior.CastSpellBlessedWarrior))
            {
                hero.GrantSpellRepertoire(BlessedWarrior.CastSpellBlessedWarrior, null, null, null);
                GrantCantripFromFightingStyle(__instance, hero, BlessedWarrior.CastSpellBlessedWarrior);
            }

            //PATCH: grants repertoire and selected cantrips from Blessed Warrior if not there yet
            if (hero.TrainedFightingStyles.Any(x => x.Name == DruidicWarrior.Name) &&
                hero.SpellRepertoires.All(x => x.spellCastingFeature != DruidicWarrior.CastSpellDruidicWarrior))
            {
                hero.GrantSpellRepertoire(DruidicWarrior.CastSpellDruidicWarrior, null, null, null);
                GrantCantripFromFightingStyle(__instance, hero, DruidicWarrior.CastSpellDruidicWarrior);
            }

            //PATCH: grants custom features
            LevelUpHelper.GrantCustomFeaturesFromFeats(hero);
            LevelUpHelper.GrantCustomFeatures(hero);
        }

        [UsedImplicitly]
        public static void Postfix(CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
        {
            //PATCH: grants cantrip that for whatever reason vanilla has a hard time granting ;-)
            GrantCantripFromCustomAcquiredPool(hero, "Thaumaturge");
            GrantCantripFromCustomAcquiredPool(hero, "DomainNature");
            GrantCantripFromCustomAcquiredPool(hero, "PactTome");
            GrantCantripFromCustomAcquiredPool(hero, "PrimalOrder");

            //PATCH: grant spells for these 2 subs as pools with tags aren't granted from subs if not at sub 1st level
            var selectedClass = LevelUpHelper.GetSelectedClass(hero);

            if (selectedClass == DatabaseHelper.CharacterClassDefinitions.Wizard)
            {
                hero.GrantAcquiredSpellWithTagFromSubclassPool(WizardAbjuration.SpellTag);
                hero.GrantAcquiredSpellWithTagFromSubclassPool(WizardEvocation.SpellTag);
            }

            //PATCH: grants spell repertoires and respective selected spells from feats
            LevelUpHelper.GrantSpellsOrCantripsFromFeatCastSpell(__instance, hero);

            //PATCH: keeps spell repertoires sorted by class title but ancestry one is always kept first
            LevelUpHelper.SortHeroRepertoires(hero);

            //PATCH: adds whole list caster spells to KnownSpells collection to improve the MC spell selection UI
            // LevelUpContext.UpdateKnownSpellsForWholeCasters(hero);

            //PATCH: unregisters the hero leveling up
            LevelUpHelper.UnregisterHero(hero);
        }

        private static void GrantCantripFromCustomAcquiredPool(RulesetCharacterHero hero, string name)
        {
            var repertoire = hero.SpellRepertoires
                .FirstOrDefault(x => LevelUpHelper.IsRepertoireFromSelectedClassSubclass(hero, x));

            if (repertoire == null)
            {
                return;
            }

            var heroBuildingData = hero.GetHeroBuildingData();
            var selectedClassLevel = LevelUpHelper.GetSelectedClassLevel(hero);

            var selectedClass = LevelUpHelper.GetSelectedClass(hero);
            var classTag = AttributeDefinitions.GetClassTag(selectedClass, selectedClassLevel);
            var classPoolName = $"{classTag}{name}";

            // consider cantrips from classes
            if (heroBuildingData.AcquiredCantrips.TryGetValue(classPoolName, out var cantrips1))
            {
                foreach (var cantrip in cantrips1)
                {
                    hero.GrantCantrip(cantrip, repertoire.SpellCastingFeature, name);
                }
            }

            // consider cantrips from feats / invocations / etc.
            classPoolName = $"{classTag}{name}{name}";

            if (heroBuildingData.AcquiredCantrips.TryGetValue(classPoolName, out var cantrips2))
            {
                foreach (var cantrip in cantrips2)
                {
                    hero.GrantCantrip(cantrip, repertoire.SpellCastingFeature, name);
                }
            }

            var selectedSubclass = LevelUpHelper.GetSelectedSubclass(hero);

            if (!selectedSubclass)
            {
                return;
            }

            // consider cantrips from subclasses
            var subclassTag = AttributeDefinitions.GetSubclassTag(selectedClass, 1, selectedSubclass);
            var subclassPoolName = $"{subclassTag}{name}";

            if (!heroBuildingData.AcquiredCantrips.TryGetValue(subclassPoolName, out var cantrips3))
            {
                return;
            }

            foreach (var cantrip in cantrips3)
            {
                hero.GrantCantrip(cantrip, repertoire.SpellCastingFeature, name);
            }
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
            LevelUpHelper.SetSelectedClass(hero, classDefinition);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpHelper.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpHelper.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            if (result)
            {
                //PATCH: grants items for new class if required
                LevelUpHelper.GrantItemsIfRequired(hero);
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
            LevelUpHelper.SetSelectedSubclass(hero, subclassDefinition);
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
            var isLevelingUp = LevelUpHelper.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpHelper.IsClassSelectionStage(hero);

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

                foreach (var badKey in pointPoolStack.ActivePools.Keys.Where(x => x != goodTag).ToArray())
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
            LevelUpHelper.SetSelectedClass(hero, null);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpHelper.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpHelper.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            if (result)
            {
                //PATCH: removes items from new class if required
                LevelUpHelper.RemoveItemsIfRequired(hero);
            }

            return !result;
        }
    }

    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.UnassignLastSubclass))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnassignLastSubclass_Patch
    {
        private static void ResetCantripsPool(RulesetCharacterHero hero, string poolName)
        {
            var buildingData = hero.GetHeroBuildingData();

            if (buildingData.PointPoolStacks.TryGetValue(HeroDefinitions.PointsPoolType.Cantrip, out var pointPool))
            {
                pointPool.ActivePools.Remove(poolName);
            }
        }

        [UsedImplicitly]
        public static bool Prefix([NotNull] RulesetCharacterHero hero)
        {
            //PATCH: avoid Domain Nature to break level up with the cantrip pool it gets
            ResetCantripsPool(hero, $"{AttributeDefinitions.TagSubclass}Cleric1DomainNatureDomainNature");

            //PATCH: un-captures the desired subclass
            LevelUpHelper.SetSelectedSubclass(hero, null);

            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpHelper.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpHelper.IsClassSelectionStage(hero);
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
        public static bool Prefix(CharacterBuildingManager __instance, [NotNull] RulesetCharacterHero hero)
        {
            //PATCH: ensures this doesn't get executed in the class panel level up screen
            var isLevelingUp = LevelUpHelper.IsLevelingUp(hero);
            var isClassSelectionStage = LevelUpHelper.IsClassSelectionStage(hero);
            var result = isLevelingUp && isClassSelectionStage;

            if (result ||
                hero.TrainedFightingStyles.Count <= 0)
            {
                return !result;
            }

            //PATCH: remove point pools assigned from fighting styles
            var heroBuildingData = hero.GetHeroBuildingData();
            var fightingStyle = hero.TrainedFightingStyles[hero.TrainedFightingStyles.Count - 1];

            foreach (var featureDefinitionPointPool in fightingStyle.Features.OfType<FeatureDefinitionPointPool>())
            {
                if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                        out var pointPoolStack))
                {
                    continue;
                }

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

            return true;
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
            LevelUpHelper.EnumerateKnownAndAcquiredSpells(heroBuildingData, __result);
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

            //PATCH: support cast spell granted from fighting style
            if (tag.EndsWith(BlessedWarrior.Name) || tag.EndsWith(DruidicWarrior.Name))
            {
                var castSpell = hero.TrainedFightingStyles[hero.TrainedFightingStyles.Count - 1].Features
                    .OfType<FeatureDefinitionCastSpell>().First();

                if (castSpell)
                {
                    var spellTag = castSpell.GetFirstSubFeatureOfType<FeatHelpers.SpellTag>();

                    if (spellTag != null && tag.EndsWith(spellTag.Name))
                    {
                        __result = castSpell;

                        return false;
                    }
                }
            }

            var isMulticlass = LevelUpHelper.IsMulticlass(hero);
            if (!isMulticlass)
            {
                return true;
            }

            var selectedClass = LevelUpHelper.GetSelectedClass(hero);

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
            var selectedClass = LevelUpHelper.GetSelectedClass(hero);
            var selectedSubclass = LevelUpHelper.GetSelectedSubclass(hero);
            var selectedClassLevel = LevelUpHelper.GetSelectedClassLevel(hero);

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

    //PATCH: apply point pools assigned from fighting styles
    [HarmonyPatch(typeof(CharacterBuildingManager), nameof(CharacterBuildingManager.TrainFightingStyle))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TrainFightingStyle_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterBuildingManager __instance,
            RulesetCharacterHero hero,
            FightingStyleDefinition fightingStyle)
        {
            var heroBuildingData = hero.GetHeroBuildingData();

            foreach (var featureDefinitionPointPool in fightingStyle.Features.OfType<FeatureDefinitionPointPool>())
            {
                if (!heroBuildingData.PointPoolStacks.TryGetValue(featureDefinitionPointPool.PoolType,
                        out var pointPoolStack))
                {
                    continue;
                }

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

            LevelUpHelper.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
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

            LevelUpHelper.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
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

            LevelUpHelper.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
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

            LevelUpHelper.RebuildCharacterStageProficiencyPanel(heroBuildingData.LevelingUp);
        }
    }
}
