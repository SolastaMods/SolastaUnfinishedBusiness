using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA;
using TA.AI;
using TA.AI.Considerations;

namespace SolastaUnfinishedBusiness.Patches;
public static class HasEnemiesInMeleeRangePatcher
{
    [HarmonyPatch(typeof(HasEnemiesInMeleeRange), nameof(HasEnemiesInMeleeRange.Score))]
    [UsedImplicitly]
    public static class Score_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            DecisionContext context, 
            ConsiderationDescription consideration, 
            DecisionParameters parameters, 
            ScoringResult scoringResult)
        {
            Score(context, consideration, parameters, scoringResult);

            return false;
        }

        private static void Score(DecisionContext context, ConsiderationDescription consideration, DecisionParameters parameters, ScoringResult scoringResult)
        {
            string stringParameter = consideration.StringParameter;
            int num = consideration.IntParameter;
            bool boolParameter = consideration.BoolParameter;
            bool boolSecParameter = consideration.BoolSecParameter;
            bool boolTerParameter = consideration.BoolTerParameter;
            int3 defenderPosition = (boolParameter ? context.position : parameters.character.GameLocationCharacter.LocationPosition);
            foreach (GameLocationCharacter relevantEnemy in parameters.situationalInformation.RelevantEnemies)
            {
                if (!AiLocationDefinitions.IsRelevantTargetForCharacter(parameters.character.GameLocationCharacter, relevantEnemy, parameters.situationalInformation.HasRelevantPerceivedTarget) || (!string.IsNullOrEmpty(stringParameter) && relevantEnemy.RulesetCharacter != null && !relevantEnemy.RulesetCharacter.HasConditionOfTypeOrSubType(stringParameter)))
                {
                    continue;
                }


                var attackMode = relevantEnemy.FindActionAttackMode(ActionDefinitions.Id.AttackMain);
                var reachRange = attackMode?.reachRange ?? 1;
                if (parameters.situationalInformation.BattleService.IsWithinXCells(relevantEnemy, relevantEnemy.LocationPosition, parameters.character.GameLocationCharacter, defenderPosition, reachRange))
                {
                    bool flag = true;
                    if (boolSecParameter)
                    {
                        bool flag2 = false;
                        flag2 = ((!boolParameter) ? relevantEnemy.PerceivedFoes.Contains(parameters.character.GameLocationCharacter) : parameters.situationalInformation.BattleService.CanAttackerSeeCharacterFromPosition(defenderPosition, relevantEnemy.LocationPosition, parameters.character.GameLocationCharacter, relevantEnemy));
                        flag = flag && flag2;
                    }

                    if (boolTerParameter)
                    {
                        flag &= parameters.situationalInformation.BattleService.IsValidAttackerForOpportunityAttackOnCharacter(relevantEnemy, parameters.character.GameLocationCharacter);
                    }

                    if (flag)
                    {
                        num--;
                    }
                }

                if (num <= 0)
                {
                    break;
                }
            }

            scoringResult.Score = ((num <= 0) ? 1f : 0f);
        }

    }
}
