using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Races
{
        [HarmonyPatch(typeof(GraphicsCharacter), "ResetScale")]
        class GraphicsCharacter_ResetScale
        {
            internal static void Postfix(GraphicsCharacter __instance, ref float __result)
            {
                Dictionary<CharacterRaceDefinition, float> raceScaleMap = new Dictionary<CharacterRaceDefinition, float>();
                
                raceScaleMap[SolastaCommunityExpansion.Races.BolgrifRaceBuilder.BolgrifRace] = 8.8f / 6.4f;
                raceScaleMap[SolastaCommunityExpansion.Races.GnomeRaceBuilder.GnomeRace] = -0.04f / -0.06f;
                
                var race = (__instance.RulesetCharacter as RulesetCharacterHero)?.RaceDefinition;
                
                if (race != null && raceScaleMap.ContainsKey(race))
                {
                    __result *= raceScaleMap[race];
                }
                else
                {
                    __result *= 1.0f / 1.0f;
                }

                __instance.transform.localScale = new Vector3(__result, __result, __result);

            }
        }
}
