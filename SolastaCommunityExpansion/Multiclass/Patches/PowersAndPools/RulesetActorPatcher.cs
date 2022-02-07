using System.Linq;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Multiclass.Patches.PowersAndPools
{
    internal static class RulesetActorPatcher
    {
        [HarmonyPatch(typeof(RulesetActor), "RefreshAttributes")]
        internal static class RulesetActorRefreshAttributes
        {
            internal static void Postfix(RulesetActor __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (__instance is not RulesetCharacterHero hero)
                {
                    return;
                }

                // fixes the Paladin pool to use the class level instead
                if (hero.ClassesAndLevels.ContainsKey(Paladin))
                {
                    var healingPoolAttribute = hero.GetAttribute(AttributeDefinitions.HealingPool, true);

                    if (healingPoolAttribute != null)
                    {
                        foreach (var activeModifier in healingPoolAttribute.ActiveModifiers
                            .Where(x => x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel
                                        || x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel))
                        {
                            activeModifier.Value = hero.ClassesAndLevels[Paladin];
                        }

                        healingPoolAttribute.Refresh();
                    }
                }

                // fixes the Sorcerer pool to use the class level instead
                if (hero.ClassesAndLevels.ContainsKey(Sorcerer))
                {
                    var sorceryPointsAttributes = hero.GetAttribute(AttributeDefinitions.SorceryPoints, true);

                    if (sorceryPointsAttributes != null)
                    {
                        foreach (var activeModifier in sorceryPointsAttributes.ActiveModifiers
                            .Where(x => x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByCharacterLevel
                                        || x.Operation == FeatureDefinitionAttributeModifier.AttributeModifierOperation.MultiplyByClassLevel))
                        {
                            activeModifier.Value = hero.ClassesAndLevels[Sorcerer];
                        }

                        sorceryPointsAttributes.Refresh();
                    }
                }
            }
        }
    }
}
