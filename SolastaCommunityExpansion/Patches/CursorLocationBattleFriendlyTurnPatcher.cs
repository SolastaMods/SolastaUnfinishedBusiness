using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class CursorLocationBattleFriendlyTurnPatcher
{
    [HarmonyPatch(typeof(CursorLocationBattleFriendlyTurn), "IsValidAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsValidAttack_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            ReachMeleeTargeting.ApplyCursorLocationIsValidAttackTranspile(code);

            return code;
        }
    }
}
