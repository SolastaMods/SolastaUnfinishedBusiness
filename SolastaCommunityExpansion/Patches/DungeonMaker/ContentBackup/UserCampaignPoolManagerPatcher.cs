using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.ContentBackup;

//PATCH: this patch allows the last X campaign files to be backed up in the mod folder
[HarmonyPatch(typeof(UserCampaignPoolManager), "SaveUserCampaign")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserCampaignPoolManager_SaveUserCampaign
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
