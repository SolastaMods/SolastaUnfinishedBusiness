using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    public static class RulesetCharacterMonsterPatcher
    {
        // ensures any spell related power can be used while in wildshape
        [HarmonyPatch(typeof(RulesetCharacterMonster), "SpellRepertoires", MethodType.Getter)]
        internal static class RulesetCharacterMonsterSpellRepertoires
        {
            internal static bool Prefix(RulesetCharacterMonster __instance, ref List<RulesetSpellRepertoire> __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                if (WildshapeContext.GetHero(__instance) is not RulesetCharacterHero hero)
                {
                    return true;
                }

                __result = hero.SpellRepertoires;

                return false;
            }
        }

        // ensures that wildshape get all original character pools and current powers states
        [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
        internal static class RulesetCharacterMonsterFinalizeMonster
        {
            // remaining pools must be added beforehand to avoid a null pointer exception
            internal static void Prefix(RulesetCharacterMonster __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is not RulesetCharacterHero hero)
                {
                    return;
                }

                foreach (var attribute in hero.Attributes.Where(x => !__instance.Attributes.ContainsKey(x.Key)))
                {
                    __instance.Attributes.Add(attribute.Key, attribute.Value);
                }
            }

            // usable powers must be added afterhand to overwrite default values from game
            internal static void Postfix(RulesetCharacterMonster __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (WildshapeContext.GetHero(__instance) is not RulesetCharacterHero hero)
                {
                    return;
                }

                __instance.UsablePowers.Clear();

                foreach (var usablePower in hero.UsablePowers)
                {
                    __instance.UsablePowers.Add(usablePower);

                    // ensures that original character rage pool is in sync with substitute
                    if (usablePower.PowerDefinition == PowerBarbarianRageStart)
                    {
                        var count = hero.UsedRagePoints;

                        while (count-- > 0)
                        {
                            __instance.SpendRagePoint();
                        }
                    }

                    __instance.RefreshUsablePower(usablePower);
                }

                // adds additional AC from class features or feat features
                var modifier = 0;
                var features = new List<FeatureDefinition>();

                hero.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(features);

                foreach (var feature in features
                    .OfType<FeatureDefinitionAttributeModifier>()
                    .Where(x => x.ModifiedAttribute == AttributeDefinitions.ArmorClass))
                {
                    modifier = Math.Max(feature.ModifierValue, modifier);
                }

                __instance.GetAttribute(AttributeDefinitions.ArmorClass).BaseValue += modifier;
            }
        }
    }
}
