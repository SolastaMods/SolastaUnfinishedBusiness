using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaMulticlass.Patches.Wildshape
{
    public static class RulesetCharacterMonsterPatcher
    {
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

                // adds additional AC from ability score bonus (stacking here to be consistent)
                var modifier = 0;

                foreach (var feature in hero.ActiveFeatures
                    .SelectMany(x => x.Value)
                    .OfType<FeatureDefinitionAttributeModifier>()
                    .Where(x => x.ModifiedAttribute == AttributeDefinitions.ArmorClass && x.ModifierType == FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddAbilityScoreBonus))
                {
                    modifier += AttributeDefinitions.ComputeAbilityScoreModifier(__instance.GetAttribute(feature.ModifierAbilityScore).CurrentValue);
                }

                __instance.GetAttribute(AttributeDefinitions.ArmorClass).BaseValue += modifier;
            }
        }
    }
}
