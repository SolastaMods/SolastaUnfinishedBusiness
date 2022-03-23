using HarmonyLib;
using SolastaCommunityExpansion.Multiclass.Models;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class RulesetActorPatcher
    {
        // enforce sorcery points and healing pool to correctly calculate class level
        [HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
        internal static class RulesetActorRefreshAttributes
        {
            public static int GetClassOrCharacterLevel(int characterLevel, RulesetCharacter rulesetCharacter, string attribute)
            {
                // enforce class level on Sorcerer Sorcery Points
                if (attribute == AttributeDefinitions.SorceryPoints)
                {
                    if (WildshapeContext.GetHero(rulesetCharacter) is RulesetCharacterHero hero 
                        && hero.ClassesAndLevels.TryGetValue(Sorcerer, out int classLevel))
                    {
                        return classLevel;
                    }
                }

                // enforce class level on Paladin Healing Pool
                else if (attribute == AttributeDefinitions.HealingPool)
                {
                    if (WildshapeContext.GetHero(rulesetCharacter) is RulesetCharacterHero hero
                        && hero.ClassesAndLevels.TryGetValue(Paladin, out int classLevel))
                    {
                        return classLevel;
                    }
                }

                return characterLevel;
            }

            internal static bool Prefix(RulesetActor __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

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
