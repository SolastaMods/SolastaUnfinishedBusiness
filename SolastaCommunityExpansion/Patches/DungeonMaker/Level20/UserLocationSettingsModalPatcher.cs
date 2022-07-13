using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Level20;

[HarmonyPatch(typeof(UserLocationSettingsModal), "OnMinLevelEndEdit")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
public static class UserLocationSettingsModal_OnMinLevelEndEdit
{
    [NotNull]
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (Main.Settings.AllowDungeonsMaxLevel20)
        {
            code
                .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GameMaxLevel)
                .ForEach(x => x.operand = ModMaxLevel);
        }

        return code;
    }
}

[HarmonyPatch(typeof(UserLocationSettingsModal), "OnMaxLevelEndEdit")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
public static class UserLocationSettingsModal_OnMaxLevelEndEdit
{
    [NotNull]
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);

        if (Main.Settings.AllowDungeonsMaxLevel20)
        {
            code
                .FindAll(x => x.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(x.operand) == GameMaxLevel)
                .ForEach(x => x.operand = ModMaxLevel);
        }

        return code;
    }
}
