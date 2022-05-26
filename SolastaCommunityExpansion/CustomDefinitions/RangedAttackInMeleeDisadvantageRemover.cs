using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class RangedAttackInMeleeDisadvantageRemover
    {
        public static readonly RangedAttackInMeleeDisadvantageRemover Marker = new();

        private RangedAttackInMeleeDisadvantageRemover() { }

        /**
         * Patches `GameLocationBattleManager.CanAttack`
         * Replaces starting value of a disadvantage on a ranged attack in melee flag from `true`
         * with a result of a call of custom method 
         */
        public static void ApplyTranspile(List<CodeInstruction> instructions)
        {
            var foundConscious = false;
            var insertionIndex = -1;
            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];
                if (foundConscious)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_1)
                    {
                        insertionIndex = i;
                        break;
                    }
                }
                else
                {
                    if (instruction.opcode == OpCodes.Call
                        && instruction.operand != null
                        && instruction.operand.ToString().Contains("IsConsciousCharacterOfSideNextToCharacter"))
                    {
                        foundConscious = true;
                    }
                }
            }

            if (insertionIndex > 0)
            {
                var method = new Func<BattleDefinitions.AttackEvaluationParams, bool>(HasDisadvantage).Method;
                instructions[insertionIndex] = new CodeInstruction(OpCodes.Call, method);
                instructions.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_1));
            }
        }

        private static bool HasDisadvantage(BattleDefinitions.AttackEvaluationParams attackParams)
        {
            if (attackParams.attackProximity != BattleDefinitions.AttackProximity.PhysicalRange)
            {
                return true;
            }

            var character = attackParams.attacker?.RulesetCharacter;
            if (character == null)
            {
                return true;
            }

            return !character.HasSubFeatureOfType<RangedAttackInMeleeDisadvantageRemover>();
        }
    }
}
