using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

                poolMods.RemoveAll(IsSpellBonus);

                heroBuildingData.HeroCharacter.BrowseFeaturesOfType<FeatureDefinition>(features, (feature, _) =>
                {
                    if (feature is IPointPoolMaxBonus bonus)
                    {
                        poolMods.Remove(bonus);
                    }
                }, tag);

                var values = new Dictionary<HeroDefinitions.PointsPoolType, int>();

                foreach (var mod in poolMods)
                {
                    values.AddOrReplace(mod.PoolType, values.GetValueOrDefault(mod.PoolType) + mod.MaxPointsBonus);
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
                if (feature is not FeatureDefinitionCastSpell spellCasting) { return;}

                var castingOrigin = spellCasting.SpellCastingOrigin;
                if (castingOrigin != CastingOrigin.Class && castingOrigin != CastingOrigin.Subclass)
                {
                    return;
                }

                var hero = heroBuildingData.HeroCharacter;
                
                ServiceRepository.GetService<ICharacterBuildingService>()
                    .GetLastAssignedClassAndLevel(hero, out var gainedClass, out _);
                
                var poolMods = hero.GetFeaturesByTypeAndTag<IPointPoolMaxBonus>(gainedClass.Name);

                poolMods.RemoveAll(p => IsSpellBonus(p));

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
