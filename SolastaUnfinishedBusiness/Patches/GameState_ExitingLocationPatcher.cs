using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameState_ExitingLocationPatcher
{
    [HarmonyPatch(typeof(GameState_ExitingLocation), nameof(GameState_ExitingLocation.Begin))]
    [UsedImplicitly]
    public static class StopCharacterEffectsIfRelevant_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: fix summoned creatures being unable to transfer with party
            var method =
                typeof(RulesetActor).GetMethod(nameof(RulesetActor.HasConditionOfType), [typeof(string)]);
            var custom = new Func<RulesetActor, string, bool>(HasConditionOfType).Method;

            return instructions.ReplaceCalls(method,
                "GameState_ExitingLocation.Begin",
                new CodeInstruction(OpCodes.Call, custom));
        }

        private static bool HasConditionOfType(RulesetActor character, string condition)
        {
            return false;
        }
    }
}
