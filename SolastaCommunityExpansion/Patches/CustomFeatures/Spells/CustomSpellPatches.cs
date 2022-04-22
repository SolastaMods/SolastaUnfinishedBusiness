using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using ModKit;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Spells
{
    internal static class CustomSpellPatches
    {
        //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect depending on some caster properties
        //and IModifySpellEffect which modifies existing effect (changing elemental damage type for example)
        [HarmonyPatch(typeof(RulesetEffectSpell), "EffectDescription", MethodType.Getter)]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class RulesetEffectSpell_EffectDescription
        {
            internal static void Postfix(ref EffectDescription __result, RulesetEffectSpell __instance)
            {
                __result = CustomFeaturesContext.ModifySpellEffect(__result, __instance);
            }
        }

        //add support for ICustomMagicEffectBasedOnCaster allowing to pick spell effect for GUI depending on caster properties
        [HarmonyPatch(typeof(GuiSpellDefinition), "EffectDescription", MethodType.Getter)]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GuiSpellDefinitionl_EffectDescription
        {
            internal static void Postfix(ref EffectDescription __result, GuiSpellDefinition __instance)
            {
                __result = CustomFeaturesContext.ModifySpellEffectGui(__result, __instance);
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
                var hero = heroBuildingData.HeroCharacter;
                var poolMods = CustomFeaturesContext.FeaturesByType<IPointPoolMaxBonus>(hero);

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
                    if (pool == null)
                    {
                        poolStack.ActivePools.Add(tag, new PointPool(value));
                    }
                    else
                    {
                        pool.MaxPoints += value;
                        pool.RemainingPoints += value;
                    }
                }
            }
        }
    }
}
