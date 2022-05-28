using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches.Insertion
{
    internal static class CursorLocationSelectTargetPatcher
    {
        [HarmonyPatch(typeof(CursorLocationSelectTarget), "IsValidAttack")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class IsValidAttack
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);

                ExtraAttacksOnActionPanel.ApplyCursorLocationSelectTargetTranspile(code);

                return code;
            }
        }
    }
}
