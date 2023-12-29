using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterMovePanelPatcher
{
    //PATCH: avoids a trace message when using cunning strike withdraw
    [HarmonyPatch(typeof(CharacterMovePanel), nameof(CharacterMovePanel.RefreshCells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InitiateLevelUpInGame_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string)], null);

            return instructions.ReplaceCalls(logErrorMethod, "CharacterMovePanel.RefreshCells",
                new CodeInstruction(OpCodes.Pop));
        }
    }
}
