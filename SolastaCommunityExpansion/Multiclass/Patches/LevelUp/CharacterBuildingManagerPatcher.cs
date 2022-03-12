using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
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
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                LevelUpContext.RegisterHero(hero);
            }
        }

        // unregister the hero leveling up
        [HarmonyPatch(typeof(CharacterBuildingManager), "ReleaseCharacter")]
        internal static class CharacterBuildingManagerReleaseCharacter
        {
            internal static void Prefix(RulesetCharacterHero hero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                LevelUpContext.UnregisterHero(hero);
            }
        }

        // captures the desired class and ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
        internal static class CharacterBuildingManagerAssignClassLevel
        {
            internal static bool Prefix(CharacterHeroBuildingData heroBuildingData, CharacterClassDefinition classDefinition)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                if (isLevelingUp && isClassSelectionStage)
                {
                    LevelUpContext.SetSelectedClass(hero, classDefinition);
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
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                LevelUpContext.SetSelectedSubclass(hero, subclassDefinition);
            }
        }

        //
        // filter FeatureUnlocks patches
        //

        // no template character are multiclass ones. leaving this here in case we ever need to support this use case
#if false
        // filter active features
        [HarmonyPatch(typeof(CharacterBuildingManager), "CreateCharacterFromTemplate")]
        internal static class CharacterBuildingManagerCreateCharacterFromTemplate
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_1);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_1);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
#endif

        // filter active features
        [HarmonyPatch(typeof(CharacterBuildingManager), "FinalizeCharacter")]
        internal static class CharacterBuildingManagerFinalizeCharacter
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // filter active features
        [HarmonyPatch(typeof(CharacterBuildingManager), "RepairSubclass")]
        internal static class CharacterBuildingManagerRepairSubclass
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var classFeatureUnlocksMethod = typeof(CharacterClassDefinition).GetMethod("get_FeatureUnlocks");
                var classFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("ClassFilteredFeatureUnlocks");

                var subclassFeatureUnlocksMethod = typeof(CharacterSubclassDefinition).GetMethod("get_FeatureUnlocks");
                var subclassFilteredFeatureUnlocksMethod = typeof(LevelUpContext).GetMethod("SubclassFilteredFeatureUnlocks");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(classFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, classFilteredFeatureUnlocksMethod);
                    }
                    else if (instruction.Calls(subclassFeatureUnlocksMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, subclassFilteredFeatureUnlocksMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        //
        // disabling method call patches
        //

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "ClearWieldedConfigurations")]
        internal static class CharacterBuildingManagerClearWieldedConfigurations
        {
            internal static bool Prefix(CharacterHeroBuildingData heroBuildingData)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
        internal static class CharacterBuildingManagerGrantFeatures
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

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "RemoveBaseEquipment")]
        internal static class CharacterBuildingManagerRemoveBaseEquipment
        {
            internal static bool Prefix(CharacterHeroBuildingData heroBuildingData)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "TransferOrSpawnWieldedItem")]
        internal static class CharacterBuildingManagerTransferOrSpawnWieldedItem
        {
            internal static bool Prefix(CharacterHeroBuildingData heroBuildingData)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignEquipment")]
        internal static class CharacterBuildingManagerUnassignEquipment
        {
            internal static bool Prefix(CharacterHeroBuildingData heroBuildingData)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
        internal static class CharacterBuildingManagerUnassignLastClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero hero)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isClassSelectionStage = LevelUpContext.IsClassSelectionStage(hero);

                if (isLevelingUp && isClassSelectionStage)
                {
                    LevelUpContext.UngrantItemsIfRequired(hero);
                }

                return !(isLevelingUp && isClassSelectionStage);
            }
        }

        //
        // SPELLS
        //

        // correctly computes the highest spell level when auto acquiring spells
        [HarmonyPatch(typeof(CharacterBuildingManager), "AutoAcquireSpells")]
        internal static class CharacterBuildingManagerAutoAcquireSpells
        {
            public static int ComputeHighestSpellLevel(FeatureDefinitionCastSpell featureDefinitionCastSpell, int classLevel, CharacterHeroBuildingData heroBuildingData)
            {
                var hero = heroBuildingData.HeroCharacter;
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);
                var isMulticlass = LevelUpContext.IsMulticlass(hero);

                if (isLevelingUp && isMulticlass)
                {
                    var selectedClass = LevelUpContext.GetSelectedClass(hero);
                    var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);

                    return SharedSpellsContext.GetClassSpellLevel(
                        hero,
                        selectedClass,
                        selectedSubclass);
                }

                return featureDefinitionCastSpell.ComputeHighestSpellLevel(classLevel);
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var computeHighesteSpellMethod = typeof(FeatureDefinitionCastSpell).GetMethod("ComputeHighestSpellLevel");
                var customComputeHighestSpellMethod = typeof(CharacterBuildingManagerAutoAcquireSpells).GetMethod("ComputeHighestSpellLevel");

                foreach (var instruction in instructions)
                {
                    if (instruction.Calls(computeHighesteSpellMethod))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, customComputeHighestSpellMethod);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }

        // ensures the level up process only presents / offers spells based on all different mod settings
        [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
        internal static class CharacterBuildingManagerEnumerateKnownAndAcquiredSpells
        {
            internal static bool Prefix(
                CharacterBuildingManager __instance,
                CharacterHeroBuildingData heroBuildingData,
                string tagToIgnore,
                ref List<SpellDefinition> __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isMulticlass = LevelUpContext.IsMulticlass(hero);
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);

                if (!(isLevelingUp && isMulticlass))
                {
                    return true;
                }

                var selectedClass = LevelUpContext.GetSelectedClass(hero);
                var spellDefinitionList = new List<SpellDefinition>();

                heroBuildingData.MatchingFeatures.Clear();

                foreach (var spellRepertoire in hero.SpellRepertoires)
                {
                    var isRepertoireFromSelectedClassSubclass = CacheSpellsContext.IsRepertoireFromSelectedClassSubclass(hero, spellRepertoire);

                    // PATCH: don't allow cantrips to be re-learned
                    foreach (var spell in spellRepertoire.KnownCantrips)
                    {
                        if (!spellDefinitionList.Contains(spell) &&
                            (
                                isRepertoireFromSelectedClassSubclass ||
                                (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, spell))
                            ))
                        {
                            spellDefinitionList.Add(spell);
                        }
                    }

                    // PATCH: don't allow spells to be re-learned
                    if (spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.WholeList)
                    {
                        var classSpellLevel = SharedSpellsContext.GetClassSpellLevel(hero, spellRepertoire.SpellCastingClass, spellRepertoire.SpellCastingSubclass);

                        for (var spellLevel = 1; spellLevel <= classSpellLevel; spellLevel++)
                        {
                            foreach (var spell in spellRepertoire.SpellCastingFeature.SpellListDefinition.GetSpellsOfLevel(spellLevel))
                            {
                                if (!spellDefinitionList.Contains(spell) &&
                                    (
                                        isRepertoireFromSelectedClassSubclass ||
                                        (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, spell))
                                    ))
                                {
                                    spellDefinitionList.Add(spell);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var spell in spellRepertoire.KnownSpells)
                        {
                            if (!spellDefinitionList.Contains(spell) &&
                                (
                                    isRepertoireFromSelectedClassSubclass ||
                                    (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, spell))
                                ))
                            {
                                spellDefinitionList.Add(spell);
                            }
                        }
                        foreach (var spell in spellRepertoire.PreparedSpells)
                        {
                            if (!spellDefinitionList.Contains(spell) &&
                                (
                                    isRepertoireFromSelectedClassSubclass ||
                                    (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, spell))
                                ))
                            {
                                spellDefinitionList.Add(spell);
                            }
                        }
                    }
                }

                // PATCH: don't allow scribed spells to be re-learned
                var foundSpellbooks = new List<RulesetItemSpellbook>();

                hero.CharacterInventory.BrowseAllCarriedItems<RulesetItemSpellbook>(foundSpellbooks);
                foreach (var foundSpellbook in foundSpellbooks)
                {
                    foreach (var spell in foundSpellbook.ScribedSpells)
                    {
                        if (!spellDefinitionList.Contains(spell) &&
                            (
                                selectedClass == Wizard ||
                                (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(hero, spell))
                            ))
                        {
                            spellDefinitionList.Add(spell);
                        }
                    }
                }

                // GAME CODE FROM HERE

                var bonusCantrips = __instance.GetField<CharacterBuildingManager, Dictionary<string, List<SpellDefinition>>>("bonusCantrips");

                foreach (var bonusCantrip in bonusCantrips)
                {
                    if (bonusCantrip.Key != tagToIgnore)
                    {
                        foreach (var spellDefinition in bonusCantrip.Value)
                        {
                            if (!spellDefinitionList.Contains(spellDefinition))
                            {
                                spellDefinitionList.Add(spellDefinition);
                            }
                        }
                    }
                }

                var acquiredCantrips = __instance.GetField<CharacterBuildingManager, Dictionary<string, List<SpellDefinition>>>("acquiredCantrips");

                foreach (var acquiredCantrip in acquiredCantrips)
                {
                    if (acquiredCantrip.Key != tagToIgnore)
                    {
                        foreach (var spellDefinition in acquiredCantrip.Value)
                        {
                            if (!spellDefinitionList.Contains(spellDefinition))
                            {
                                spellDefinitionList.Add(spellDefinition);
                            }
                        }
                    }
                }

                var acquiredSpells = __instance.GetField<CharacterBuildingManager, Dictionary<string, List<SpellDefinition>>>("acquiredSpells");

                foreach (var acquiredSpell in acquiredSpells)
                {
                    if (acquiredSpell.Key != tagToIgnore)
                    {
                        foreach (var spellDefinition in acquiredSpell.Value)
                        {
                            if (!spellDefinitionList.Contains(spellDefinition))
                            {
                                spellDefinitionList.Add(spellDefinition);
                            }
                        }
                    }
                }

                __result = spellDefinitionList;

                return false;
            }
        }

        // removes any levels from the tag otherwise it'll have a hard time finding it if multiclassed
        [HarmonyPatch(typeof(CharacterBuildingManager), "GetSpellFeature")]
        internal static class CharacterBuildingManagerGetSpellFeature
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
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);

                if (!(isLevelingUp && isMulticlass))
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
                    foreach (var featureDefinition in activeFeature.Value)
                    {
                        if (featureDefinition is FeatureDefinitionCastSpell)
                        {
                            __result = featureDefinition as FeatureDefinitionCastSpell;
                            return false;
                        }
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
                    foreach (var featureDefinition in activeFeature.Value)
                    {
                        if (featureDefinition is FeatureDefinitionCastSpell)
                        {
                            __result = featureDefinition as FeatureDefinitionCastSpell;
                            return false;
                        }
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
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var hero = heroBuildingData.HeroCharacter;
                var isMulticlass = LevelUpContext.IsMulticlass(hero);
                var isLevelingUp = LevelUpContext.IsLevelingUp(hero);

                if (!(isLevelingUp && isMulticlass))
                {
                    return true;
                }

                var selectedClass = LevelUpContext.GetSelectedClass(hero);
                var selectedSubclass = LevelUpContext.GetSelectedSubclass(hero);
                var selectedClassLevel = LevelUpContext.SelectedClassLevel(hero);

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
                        poolName = "02Race";
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
