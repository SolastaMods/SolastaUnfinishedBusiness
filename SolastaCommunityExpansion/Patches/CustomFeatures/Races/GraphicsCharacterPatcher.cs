using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.CharacterRaceDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Races
{
        [HarmonyPatch(typeof(GraphicsCharacter), "ResetScale")]
        class GraphicsCharacter_ResetScale
        {
		    //Both scales work, just have to make them work together now
            //const float bolgrifScaleMap = 7.8f /6.4f;

            const float gnomeScaleMap = -0.02f /-0.03f;
        
			
            internal static void Postfix(GraphicsCharacter __instance, ref float __result)
            {
                var race = (__instance.RulesetCharacter as RulesetCharacterHero)?.RaceDefinition;
				
				if (/*race != SolastaCommunityExpansion.Races.BolgrifRaceBuilder.BolgrifRace &&*/ race != SolastaCommunityExpansion.Races.GnomeRaceBuilder.GnomeRace)
				{
					return;
				}

                    //__result *= bolgrifScaleMap;
                    //__instance.transform.localScale = new Vector3(__result, __result, __result);


                    __result *= gnomeScaleMap;
                    __instance.transform.localScale = new Vector3(__result, __result, __result);



            }
        }
}
