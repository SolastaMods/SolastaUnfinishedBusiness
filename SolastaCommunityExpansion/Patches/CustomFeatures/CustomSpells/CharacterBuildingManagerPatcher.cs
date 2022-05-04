using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using static FeatureDefinitionCastSpell;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    internal static class CharacterBuildingManagerPatcher
    {
        internal static bool IsSpellBonus(IPointPoolMaxBonus mod)
        {
            return mod.PoolType == HeroDefinitions.PointsPoolType.Cantrip
                || mod.PoolType == HeroDefinitions.PointsPoolType.Spell;
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

                poolMods.RemoveAll(IsSpellBonus);

                heroBuildingData.HeroCharacter.BrowseFeaturesOfType<FeatureDefinition>(features, (feature, _) =>
                {
                    if (feature is IPointPoolMaxBonus bonus)
                    {
                        poolMods.Remove(bonus);
                        if (IsSpellBonus(bonus))
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

                //remove spell bonuses ganed this level - they are added before this
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

        //Replacing this method completely to remove weird 'return'. TA confirmed they will remove it in next patch.
        [HarmonyPatch(typeof(CharacterBuildingManager), "BrowseGrantedFeaturesHierarchically")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterBuildingManager_BrowseGrantedFeaturesHierarchically
        {
            internal static bool Prefix(CharacterBuildingManager __instance,
                CharacterHeroBuildingData heroBuildingData,
                List<FeatureDefinition> grantedFeatures,
                string tag)
            {
                foreach (FeatureDefinition grantedFeature in grantedFeatures)
                {
                    switch (grantedFeature)
                    {
                        case FeatureDefinitionCastSpell spell:
                            __instance.SetupSpellPointPools(heroBuildingData, spell, tag);
                            continue; // In original code this was 'return'
                        case FeatureDefinitionBonusCantrips cantrips:
                            foreach (var cantrip in cantrips.BonusCantrips)
                            {
                                __instance.AcquireBonusCantrip(heroBuildingData, cantrip, tag);
                            }
                            continue;
                        case FeatureDefinitionProficiency proficiency:
                            if (proficiency.ProficiencyType == RuleDefinitions.ProficiencyType.FightingStyle)
                            {
                                foreach (var name in proficiency.Proficiencies)
                                {
                                    var style = DatabaseRepository.GetDatabase<FightingStyleDefinition>().GetElement(name);
                                    __instance.AcquireBonusFightingStyle(heroBuildingData, style, tag);
                                }

                                continue;
                            }
                            else
                                continue;
                        case FeatureDefinitionFeatureSet featureSet:
                            if (featureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Union)
                            {
                                __instance.BrowseGrantedFeaturesHierarchically(heroBuildingData,
                                    featureSet.FeatureSet, tag);
                            }
                            continue;
                        default:
                            continue;
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(CharacterBuildingManager), "ApplyFeatureCastSpell")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class CharacterBuildingManager_ApplyFeatureCastSpell
        {
            internal static void Postfix(CharacterHeroBuildingData heroBuildingData,
                FeatureDefinition feature)
            {
                if (feature is not FeatureDefinitionCastSpell spellCasting) { return;}

                var castingOrigin = spellCasting.SpellCastingOrigin;
                if (castingOrigin != CastingOrigin.Class && castingOrigin != CastingOrigin.Subclass)
                {
                    return;
                }

                var hero = heroBuildingData.HeroCharacter;
                var poolMods = hero.GetFeaturesByType<IPointPoolMaxBonus>();

                poolMods.RemoveAll(p => !IsSpellBonus(p));

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
            }
        }
    }
}
