using System;
using System.Linq;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class RulesetCharacterMonsterPatcher
    {
        // ensures that wildshapes get all original character pools and current powers states
        [HarmonyPatch(typeof(RulesetCharacterMonster), "FinalizeMonster")]
        internal static class RulesetCharacterMonsterRefreshAttributes
        {
            // remaining pools must be added beforehand to avoid a null pointer exception
            internal static void Prefix(RulesetCharacterMonster __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (!__instance.IsSubstitute)
                {
                    return;
                }

                if (Gui.GameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == __instance.Name)?.RulesetCharacter is not RulesetCharacterHero hero)
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

                if (__instance?.IsSubstitute == false)
                {
                    return;
                }

                if (Gui.GameCampaign.Party.CharactersList.Find(x => x.RulesetCharacter.Name == __instance.Name)?.RulesetCharacter is not RulesetCharacterHero hero)
                {
                    return;
                }

                __instance.UsablePowers.Clear();

                foreach (var usablePower in hero.UsablePowers)
                {
                    __instance.UsablePowers.Add(usablePower);

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

                // adds additional AC from Unarmored Defense
                var modifier = 0;

                if (hero.ClassesAndLevels.ContainsKey(Models.IntegrationContext.MonkClass))
                {
                    var wisdomModifier = AttributeDefinitions.ComputeAbilityScoreModifier(hero.GetAttribute(AttributeDefinitions.Wisdom).CurrentValue);

                    modifier = Math.Max(wisdomModifier, modifier);
                }

                if (hero.ClassesAndLevels.ContainsKey(Barbarian))
                {
                    var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(hero.GetAttribute(AttributeDefinitions.Constitution).CurrentValue);

                    modifier = Math.Max(constitutionModifier, modifier);
                }

                __instance.GetAttribute(AttributeDefinitions.ArmorClass).BaseValue += modifier;
            }
        }
    }
}
