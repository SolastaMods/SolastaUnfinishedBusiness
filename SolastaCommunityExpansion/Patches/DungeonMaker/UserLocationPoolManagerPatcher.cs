using HarmonyLib;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Emit;

namespace SolastaCommunityExpansion.Patches.DungeonMaker
{
    // this patch allows the last X location files to be backed up in the mod folder
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(UserLocationPoolManager), "SaveUserLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UserLocationPoolManager_SaveUserLocation
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var deleteMethod = typeof(File).GetMethod("Delete");
            var backupAndDeleteMethod = typeof(Models.DungeonMakerContext).GetMethod("BackupAndDelete");

            foreach (CodeInstruction instruction in instructions)
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
}
