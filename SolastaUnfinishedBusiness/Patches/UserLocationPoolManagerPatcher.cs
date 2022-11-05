using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserLocationPoolManagerPatcher
{
    //PATCH: allows the last X campaign files to be backed up in the mod folder
    [HarmonyPatch(typeof(UserLocationPoolManager), "SaveUserLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveUserLocation_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var deleteMethod = typeof(File).GetMethod("Delete");
            var backupAndDeleteMethod = new Action<string, UserContent>(DmProEditorContext.BackupAndDelete).Method;

            return instructions.ReplaceCalls(deleteMethod, "UserLocationPoolManagerPatcher.SaveUserLocation_Patch",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, backupAndDeleteMethod));
        }
    }
}
