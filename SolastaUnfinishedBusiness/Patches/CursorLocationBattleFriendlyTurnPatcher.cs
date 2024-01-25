using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationBattleFriendlyTurnPatcher
{
    [HarmonyPatch(typeof(CursorLocationBattleFriendlyTurn), nameof(CursorLocationBattleFriendlyTurn.IsValidAttack))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsValidAttack_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: ReachMeleeTargeting
            var findBestActionDestinationMethod = typeof(CursorLocationBattleFriendlyTurn)
                .GetMethod("FindBestActionDestination", BindingFlags.Instance | BindingFlags.NonPublic);
            var method = typeof(ReachMeleeTargeting)
                .GetMethod("FindBestActionDestination", BindingFlags.Static | BindingFlags.NonPublic);

            return instructions.ReplaceCalls(findBestActionDestinationMethod,
                "CursorLocationBattleFriendlyTurn.IsValidAttack",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Call, method));
        }
    }
}
