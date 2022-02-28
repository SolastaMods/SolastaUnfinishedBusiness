using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterBuildingManagerPatcher
    {
        // captures the desired class and ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignClassLevel")]
        internal static class CharacterBuildingManagerAssignClassLevel
        {
            internal static bool Prefix(CharacterClassDefinition classDefinition)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel)
                {
                    LevelUpContext.SelectedClass = classDefinition;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // captures the desired sub class
        [HarmonyPatch(typeof(CharacterBuildingManager), "AssignSubclass")]
        internal static class CharacterBuildingManagerAssignSubclass
        {
            internal static void Prefix(CharacterSubclassDefinition subclassDefinition)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                LevelUpContext.SelectedSubclass = subclassDefinition;
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "ClearWieldedConfigurations")]
        internal static class CharacterBuildingManagerClearWieldedConfigurations
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantBaseEquipment")]
        internal static class CharacterBuildingManagerGrantBaseEquipment
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "GrantFeatures")]
        internal static class CharacterBuildingManagerGrantFeatures
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "RemoveBaseEquipment")]
        internal static class CharacterBuildingManagerRemoveBaseEquipment
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "TransferOrSpawnWieldedItem")]
        internal static class CharacterBuildingManagerTransferOrSpawnWieldedItem
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures this doesn't get executed in the class panel level up screen
        [HarmonyPatch(typeof(CharacterBuildingManager), "UnassignLastClassLevel")]
        internal static class CharacterBuildingManagerUnassignLastClassLevel
        {
            internal static bool Prefix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel)
                {
                    LevelUpContext.UngrantItemsIfRequired();
                }

                return !(LevelUpContext.LevelingUp && LevelUpContext.DisplayingClassPanel);
            }
        }

        // ensures the level up process only presents / offers spells based on all different mod settings
        [HarmonyPatch(typeof(CharacterBuildingManager), "EnumerateKnownAndAcquiredSpells")]
        internal static class CharacterBuildingManagerEnumerateKnownAndAcquiredSpells
        {
            internal static bool Prefix(CharacterBuildingManager __instance, string tagToIgnore, ref List<SpellDefinition> __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (!(LevelUpContext.LevelingUp && LevelUpContext.IsMulticlass))
                {
                    return true;
                }

                var spellDefinitionList = new List<SpellDefinition>();

                __instance.GetField<CharacterBuildingManager, List<FeatureDefinition>>("matchingFeatures").Clear();

                foreach (var spellRepertoire in __instance.HeroCharacter.SpellRepertoires)
                {
                    var isRepertoireFromSelectedClassSubclass = CacheSpellsContext.IsRepertoireFromSelectedClassSubclass(spellRepertoire);

                    // PATCH: don't allow cantrips to be re-learned
                    foreach (var spell in spellRepertoire.KnownCantrips)
                    {
                        if (!spellDefinitionList.Contains(spell) &&
                            (
                                isRepertoireFromSelectedClassSubclass ||
                                (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(spell))
                            ))
                        {
                            spellDefinitionList.Add(spell);
                        }
                    }

                    // PATCH: don't allow spells to be re-learned
                    if (spellRepertoire.SpellCastingFeature.SpellKnowledge == RuleDefinitions.SpellKnowledge.WholeList)
                    {
                        var classSpellLevel = SharedSpellsContext.GetClassSpellLevel(LevelUpContext.SelectedHero, spellRepertoire.SpellCastingClass, spellRepertoire.SpellCastingSubclass);

                        for (var spellLevel = 1; spellLevel <= classSpellLevel; spellLevel++)
                        {
                            foreach (var spell in spellRepertoire.SpellCastingFeature.SpellListDefinition.GetSpellsOfLevel(spellLevel))
                            {
                                if (!spellDefinitionList.Contains(spell) &&
                                    (
                                        isRepertoireFromSelectedClassSubclass ||
                                        (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(spell))
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
                                    (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(spell))
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
                                    (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(spell))
                                ))
                            {
                                spellDefinitionList.Add(spell);
                            }
                        }
                    }
                }

                // PATCH: don't allow scribed spells to be re-learned
                var foundSpellbooks = new List<RulesetItemSpellbook>();

                __instance.HeroCharacter.CharacterInventory.BrowseAllCarriedItems<RulesetItemSpellbook>(foundSpellbooks);
                foreach (var foundSpellbook in foundSpellbooks)
                {
                    foreach (var spell in foundSpellbook.ScribedSpells)
                    {
                        if (!spellDefinitionList.Contains(spell) &&
                            (
                                LevelUpContext.SelectedClass == Wizard ||
                                (!Main.Settings.EnableRelearnSpells && CacheSpellsContext.IsSpellOfferedBySelectedClassSubclass(spell))
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
            internal static bool Prefix(string tag, ref FeatureDefinitionCastSpell __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (!(LevelUpContext.LevelingUp && LevelUpContext.IsMulticlass))
                {
                    return true;
                }

                var localTag = tag;

                __result = null;

                if (localTag.StartsWith(AttributeDefinitions.TagClass))
                {
                    localTag = AttributeDefinitions.TagClass + LevelUpContext.SelectedClass.Name;
                }
                else if (localTag.StartsWith(AttributeDefinitions.TagSubclass))
                {
                    localTag = AttributeDefinitions.TagSubclass + LevelUpContext.SelectedClass.Name;
                }

                // PATCH
                foreach (var activeFeature in LevelUpContext.SelectedHero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
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

                localTag = AttributeDefinitions.TagClass + LevelUpContext.SelectedClass.Name;

                // PATCH
                foreach (var activeFeature in LevelUpContext.SelectedHero.ActiveFeatures.Where(x => x.Key.StartsWith(localTag)))
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
                ref int ___tempAcquiredCantripsNumber,
                ref int ___tempAcquiredSpellsNumber,
                ref int ___tempUnlearnedSpellsNumber)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (!(LevelUpContext.LevelingUp && LevelUpContext.IsMulticlass))
                {
                    return true;
                }

                foreach (var spellRepertoire in __instance.HeroCharacter.SpellRepertoires)
                {
                    var poolName = string.Empty;
                    var maxPoints = 0;

                    if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class)
                    {
                        // PATCH: short circuit if the feature is for another class (change from native code)
                        if (spellRepertoire.SpellCastingClass != LevelUpContext.SelectedClass)
                        {
                            continue;
                        }

                        poolName = AttributeDefinitions.GetClassTag(LevelUpContext.SelectedClass, LevelUpContext.SelectedClassLevel); // SelectedClassLevel ???
                    }
                    else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass)
                    {
                        // PATCH: short circuit if the feature is for another subclass (change from native code)
                        if (spellRepertoire.SpellCastingSubclass != LevelUpContext.SelectedSubclass)
                        {
                            continue;
                        }

                        poolName = AttributeDefinitions.GetSubclassTag(LevelUpContext.SelectedClass, LevelUpContext.SelectedClassLevel, LevelUpContext.SelectedSubclass); // SelectedClassLevel ???
                    }
                    else if (spellRepertoire.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Race)
                    {
                        poolName = "02Race";
                    }

                    var characterBuildingManagerType = typeof(CharacterBuildingManager);
                    var applyFeatureCastSpellMethod = characterBuildingManagerType.GetMethod("ApplyFeatureCastSpell", BindingFlags.NonPublic | BindingFlags.Instance);
                    var setPointPoolMethod = characterBuildingManagerType.GetMethod("SetPointPool", BindingFlags.NonPublic | BindingFlags.Instance);

                    ___tempAcquiredCantripsNumber = 0;
                    ___tempAcquiredSpellsNumber = 0;
                    ___tempUnlearnedSpellsNumber = 0;

                    applyFeatureCastSpellMethod.Invoke(__instance, new object[] { spellRepertoire.SpellCastingFeature });

                    if (__instance.HasAnyActivePoolOfType(HeroDefinitions.PointsPoolType.Cantrip) && __instance.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools.ContainsKey(poolName))
                    {
                        maxPoints = __instance.PointPoolStacks[HeroDefinitions.PointsPoolType.Cantrip].ActivePools[poolName].MaxPoints;
                    }

                    setPointPoolMethod.Invoke(__instance, new object[] { HeroDefinitions.PointsPoolType.Cantrip, poolName, ___tempAcquiredCantripsNumber + maxPoints });
                    setPointPoolMethod.Invoke(__instance, new object[] { HeroDefinitions.PointsPoolType.Spell, poolName, ___tempAcquiredSpellsNumber });
                    setPointPoolMethod.Invoke(__instance, new object[] { HeroDefinitions.PointsPoolType.SpellUnlearn, poolName, ___tempUnlearnedSpellsNumber });
                }

                return false;
            }
        }
    }
}
