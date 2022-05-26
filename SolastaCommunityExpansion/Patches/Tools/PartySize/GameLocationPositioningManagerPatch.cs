using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize
{
    // avoids a trace message when party greater than 4
    //
    // this shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationPositioningManager), "CharacterMoved", typeof(GameLocationCharacter),
        typeof(TA.int3), typeof(TA.int3), typeof(RulesetActor.SizeParameters), typeof(RulesetActor.SizeParameters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationPositioningManager_CharacterMoved
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] {typeof(string)}, null);
            var found = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(logErrorMethod) && ++found == 1)
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
