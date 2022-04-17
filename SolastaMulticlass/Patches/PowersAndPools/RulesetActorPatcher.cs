using System.Collections.Generic;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Patches.PowersAndPools
{
    internal static class RulesetActorPatcher
    {
        // enforce sorcery points and healing pool to correctly calculate class level
        [HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
        internal static class RulesetActorRefreshAttributes
        {
            private static readonly Dictionary<string, CharacterClassDefinition> rules = new()
            {
                { AttributeDefinitions.HealingPool, Paladin },
                { AttributeDefinitions.SorceryPoints, Sorcerer }
            };

            public static int GetClassOrCharacterLevel(int characterLevel, RulesetCharacter rulesetCharacter, string attribute)
            {
                if (rules.TryGetValue(attribute, out CharacterClassDefinition characterClass))
                {
                    var hero = rulesetCharacter as RulesetCharacterHero ?? rulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

                    if (hero != null && hero.ClassesAndLevels.TryGetValue(characterClass, out int classLevel))
                    {
                        return classLevel;
                    }
                }

                return characterLevel;
            }

            internal static bool Prefix(RulesetActor __instance)
            {
                if (__instance is not RulesetCharacter rulesetCharacter)
                {
                    return true;
                }

                var characterLevel = 1;
                var characterLevelAttribute = rulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel, true);

                if (characterLevelAttribute != null)
                {
                    characterLevelAttribute.Refresh();
                    characterLevel = characterLevelAttribute.CurrentValue;
                }

                foreach (var attribute in __instance.Attributes)
                {
                    foreach (RulesetAttributeModifier activeModifier in attribute.Value.ActiveModifiers)
                    {
                        if (activeModifier.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel)
                        {
                            activeModifier.Value = (float)characterLevel;
                        }
                        else if (activeModifier.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel)
                        {
                            activeModifier.Value = GetClassOrCharacterLevel(characterLevel, rulesetCharacter, attribute.Key);
                        }
                    }

                    attribute.Value.Refresh();
                }

                return false;
            }
        }
    }
}
