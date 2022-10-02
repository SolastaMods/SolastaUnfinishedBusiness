using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

public static class CursorLocationBattleFriendlyTurnPatcher
{
    [HarmonyPatch(typeof(CursorLocationBattleFriendlyTurn), "IsValidAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsValidAttack_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            ReachMeleeTargeting.ApplyCursorLocationIsValidAttackTranspile(code);

            return code;
        }
    }
}
