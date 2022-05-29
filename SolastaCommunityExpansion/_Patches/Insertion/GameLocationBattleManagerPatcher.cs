using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.Insertion
{
    internal static class GameLocationBattleManagerPatcher
    {
        [HarmonyPatch(typeof(GameLocationBattleManager), "CanAttack")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GameLocationBattleManager_CanAttack
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                RangedAttackInMeleeDisadvantageRemover.ApplyTranspile(code);

                return code;
            }
        }
    }
}
