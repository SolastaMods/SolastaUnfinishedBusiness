using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserCampaignPoolManagerPatcher
{
    //PATCH: allows the last X campaign files to be backed up in the mod folder
    [HarmonyPatch(typeof(UserCampaignPoolManager), "SaveUserCampaign")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SaveUserCampaign_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var deleteMethod = typeof(File).GetMethod("Delete");
            var backupAndDeleteMethod = new Action<string, UserContent>(DungeonMakerContext.BackupAndDelete).Method;

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
}
