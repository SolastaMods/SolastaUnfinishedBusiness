using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationTelekinesisPatcher
{
    [HarmonyPatch(typeof(CursorLocationTelekinesis), nameof(CursorLocationTelekinesis.OnClickMainPointer))]
    [UsedImplicitly]
    public static class OnClickMainPointer_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Makes Telekinesis work during exploration
            var customBindMethod =
                new Func<IGameLocationBattleService, bool>(True).Method;

            return instructions.ReplaceCall(
                "IsBattleInProgress",
                -1,
                "CursorLocationTelekinesis.OnClickMainPointer",
                new CodeInstruction(OpCodes.Call, customBindMethod));
        }

        private static bool True(IGameLocationBattleService battle)
        {
            return true;
        }
    }
}
