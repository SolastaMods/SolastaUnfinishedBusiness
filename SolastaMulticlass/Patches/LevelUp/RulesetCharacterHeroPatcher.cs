using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class RulesetCharacterHeroPatcher
    {
        // ensures we only add the dice max value on level 1
        [HarmonyPatch(typeof(RulesetCharacterHero), "AddClassLevel")]
        internal static class RulesetCharacterHeroAddClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero __instance, CharacterClassDefinition classDefinition, List<int> ___hitPointsGainHistory)
            {
                if (!LevelUpContext.IsLevelingUp(__instance))
                {
                    return true;
                }

                __instance.ClassesHistory.Add(classDefinition);

                __instance.ClassesAndLevels.TryAdd(classDefinition, 0);
                __instance.ClassesAndLevels[classDefinition]++;

                ___hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));

                __instance.ComputeCharacterLevel();
                __instance.ComputeProficiencyBonus();

                return false;
            }
        }
    }
}
