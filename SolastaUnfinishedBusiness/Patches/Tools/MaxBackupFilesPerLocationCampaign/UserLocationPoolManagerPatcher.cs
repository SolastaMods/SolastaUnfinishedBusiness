using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.Tools.MaxBackupFilesPerLocationCampaign;

//PATCH: this patch allows the last X location files to be backed up in the mod folder
[HarmonyPatch(typeof(UserLocationPoolManager), "SaveUserLocation")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserLocationPoolManager_SaveUserLocation
{
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var deleteMethod = typeof(File).GetMethod("Delete");
        var backupAndDeleteMethod = typeof(DungeonMakerContext).GetMethod("BackupAndDelete");

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(deleteMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, backupAndDeleteMethod);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}
