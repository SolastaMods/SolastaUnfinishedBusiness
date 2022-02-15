using System.Collections.Generic;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class RulesetCharacterHeroPatcher
    {
        [HarmonyPatch(typeof(RulesetCharacterHero), "AddClassLevel")]
        internal static class RulesetCharacterHeroAddClassLevel
        {
            internal static bool Prefix(RulesetCharacterHero __instance, CharacterClassDefinition classDefinition, List<int> ___hitPointsGainHistory)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                __instance.ClassesHistory.Add(classDefinition);

                if (__instance.ClassesAndLevels.ContainsKey(classDefinition))
                {
                    __instance.ClassesAndLevels[classDefinition]++;
                } 
                else
                {
                    __instance.ClassesAndLevels.Add(classDefinition, 1);
                }

                if (__instance.ClassesAndLevels[classDefinition] == 1 && !Models.LevelUpContext.IsMulticlass)
                {
                    ___hitPointsGainHistory.Add(RuleDefinitions.DiceMaxValue[(int)classDefinition.HitDice]);
                }
                else
                {
                    ___hitPointsGainHistory.Add(HeroDefinitions.RollHitPoints(classDefinition.HitDice));
                }

                __instance.ComputeCharacterLevel();
                __instance.ComputeProficiencyBonus();

                return false;
            }
        }
    }
}
