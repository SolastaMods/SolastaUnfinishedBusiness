using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationPositioningManagerPatcher
{
    //PATCH: avoids a trace message when party greater than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationPositioningManager), "CharacterMoved", typeof(GameLocationCharacter),
        typeof(int3), typeof(int3), typeof(RulesetActor.SizeParameters), typeof(RulesetActor.SizeParameters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterMoved_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(string) }, null);
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
