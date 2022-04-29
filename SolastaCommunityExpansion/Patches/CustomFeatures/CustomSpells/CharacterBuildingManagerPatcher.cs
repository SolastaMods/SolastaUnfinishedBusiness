using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
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
}
