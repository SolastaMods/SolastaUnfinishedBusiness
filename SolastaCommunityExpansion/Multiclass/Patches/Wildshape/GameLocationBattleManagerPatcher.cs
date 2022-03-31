using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.Wildshape
{
    internal static class GameLocationBattleManagerPatcher
    {
        // fixes additional damage calculation under wildshape (i.e.: rage, etc.)
        [HarmonyPatch(typeof(GameLocationBattleManager), "ComputeAndNotifyAdditionalDamage")]
        internal static class GameLocationBattleManagerComputeAndNotifyAdditionalDamage
        {
            public static bool IsRulesetCharacterHero(RulesetCharacter rulesetCharacter)
            {
                return WildshapeContext.GetHero(rulesetCharacter) is RulesetCharacterHero;
            }

            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    foreach (var instruction in instructions)
                    {
                        yield return instruction;
                    }

                    yield break;
                }

                var found = 0;
                var IsRulesetCharacterHeroMethod = typeof(GameLocationBattleManagerComputeAndNotifyAdditionalDamage).GetMethod("IsRulesetCharacterHero");

                foreach (var instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Isinst && instruction.operand is RulesetCharacterHero)
                    {
                        ++found;

                        if (found == 2 || found == 4) // only need to trap the "is" not the "as"
                        {
                            yield return new CodeInstruction(OpCodes.Call, IsRulesetCharacterHeroMethod); // RulesetCharacter is on stack at this point
                        }
                        else
                        {
                            yield return instruction;
                        }
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}
