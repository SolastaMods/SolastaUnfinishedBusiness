using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class NarrativeStatePlayerRoleAssignmentPatcher
{
    //PATCH: ensures dialogs won't break on official campaigns with parties less than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(NarrativeStatePlayerRoleAssignment), nameof(NarrativeStatePlayerRoleAssignment.BuildHook))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildHook_Patch
    {
        private static void TryAdd(
            Dictionary<string, WorldLocationCharacter> playerRolesMap,
            string role,
            WorldLocationCharacter actor)
        {
            playerRolesMap.TryAdd(role, actor);
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var tryAddMethod = new Action<
                Dictionary<string, WorldLocationCharacter>,
                string,
                WorldLocationCharacter>(TryAdd).Method;

            return instructions.ReplaceAdd("System.String, WorldLocationCharacter", -1,
                "NarrativeStatePlayerRoleAssignment.BuildHook",
                new CodeInstruction(OpCodes.Call, tryAddMethod));
        }
    }
}
