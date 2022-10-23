using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameLocationPositioningManagerPatcher
{
    //PATCH: avoids a trace message when party greater than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationPositioningManager), "CharacterMoved", typeof(GameLocationCharacter),
        typeof(int3), typeof(int3), typeof(RulesetActor.SizeParameters), typeof(RulesetActor.SizeParameters))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CharacterMoved_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logErrorMethod = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, new[] { typeof(string) }, null);

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(logErrorMethod))
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
