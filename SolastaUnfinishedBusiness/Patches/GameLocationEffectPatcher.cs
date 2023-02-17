using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationEffectPatcher
{
    //PATCH: avoid log messages when item effects aren't removed from heroes when unconscious (IShouldTerminateEffect)
    [HarmonyPatch(typeof(GameLocationEffect), nameof(GameLocationEffect.SerializeAttributes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SerializeAttributes
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: setup tooltip of a power passed to `GameConsole.AttackRolled`
            var methodLogError = typeof(Trace).GetMethod("LogError", BindingFlags.Public | BindingFlags.Static, null,
                new[] { typeof(string), typeof(object[]) }, null);
            var methodLogException = typeof(Trace).GetMethod("LogException", BindingFlags.Public | BindingFlags.Static);

            return instructions
                .ReplaceCalls(
                    methodLogError,
                    "SerializeAttributes.LogError",
                    new CodeInstruction(OpCodes.Pop),
                    new CodeInstruction(OpCodes.Pop))
                .ReplaceCalls(
                    methodLogException,
                    "SerializeAttributes.LogException",
                    new CodeInstruction(OpCodes.Pop));
        }
    }
}
