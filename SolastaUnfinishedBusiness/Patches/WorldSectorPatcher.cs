using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class WorldSectorPatcher
{
    [HarmonyPatch(typeof(WorldSector), nameof(WorldSector.Voxelize))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetHighlightVisibility_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var logError1Method = typeof(Trace).GetMethod("LogWarning", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string)], null);
            var logError2Method = typeof(Trace).GetMethod("LogWarning", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string), typeof(object[])], null);
            var logError3Method = typeof(Trace).GetMethod("LogWarning", BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string), typeof(Object), typeof(object[])], null);

            var myLogError2Method = typeof(SetHighlightVisibility_Patch).GetMethod("LogWarning",
                BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string), typeof(object[])], null);
            var myLogError3Method = typeof(SetHighlightVisibility_Patch).GetMethod("LogWarning",
                BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder, [typeof(string), typeof(Object), typeof(object[])], null);

            return instructions
                .ReplaceCalls(logError1Method, "WorldSector.Voxelize1",
                    new CodeInstruction(OpCodes.Pop))
                .ReplaceCalls(logError2Method, "WorldSector.Voxelize2",
                    new CodeInstruction(OpCodes.Call, myLogError2Method))
                .ReplaceCalls(logError3Method, "WorldSector.Voxelize3",
                    new CodeInstruction(OpCodes.Call, myLogError3Method));
        }

        [UsedImplicitly]
#pragma warning disable IDE0060
        public static void LogWarning(
            [UsedImplicitly] string errorMessage,
            [UsedImplicitly] params object[] args)
#pragma warning restore IDE0060
        {
            // empty
        }

        [UsedImplicitly]
#pragma warning disable IDE0060
        public static void LogWarning(
            [UsedImplicitly] string errorMessage,
            [UsedImplicitly] Object obj,
            params object[] args)
#pragma warning restore IDE0060
        {
            // empty
        }
    }
}
