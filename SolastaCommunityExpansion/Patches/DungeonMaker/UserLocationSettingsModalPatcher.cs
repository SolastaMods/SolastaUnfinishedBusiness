using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Models.Level20Context;
using static SolastaCommunityExpansion.Models.DmProEditorContext;

namespace SolastaCommunityExpansion.Patches.DungeonMaker;

//PATCH: unlocks visual moods across all environments
[HarmonyPatch(typeof(UserLocationSettingsModal), "RefreshVisualMoods")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserLocationSettingsModal_RefreshVisualMoods
{
    internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
    {
        var found = 0;

        foreach (var instruction in instructions)
        {
            if (!Main.Settings.EnableDungeonMakerPro)
            {
                yield return instruction;
            }
            else if (Main.Settings.EnableDungeonMakerModdedContent && instruction.opcode == OpCodes.Brfalse_S &&
                     ++found == 1)
            {
                yield return new CodeInstruction(OpCodes.Pop);
            }
            else
            {
                yield return instruction;
            }
        }
    }
}

//PATCH: adds custom dungeons sizes
[HarmonyPatch(typeof(UserLocationSettingsModal), "RuntimeLoaded")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserLocationSettingsModal_RuntimeLoaded
{
    internal static void Postfix(UserLocationSettingsModal __instance)
    {
        if (!Main.Settings.EnableDungeonMakerPro || !Main.Settings.EnableDungeonMakerModdedContent)
        {
            return;
        }

        for (var size = ExtendedDungeonSize.Huge; size <= ExtendedDungeonSize.Gargantuan; size++)
        {
            var sizeString = UserLocationDefinitions.CellsBySize[(UserLocationDefinitions.Size)size].ToString();

            __instance.optionsListSize.Add(new GuiDropdown.OptionDataAdvanced
            {
                text = Gui.FormatLocationSize((UserLocationDefinitions.Size)size).Khaki() + " " +
                       Gui.Format("{0} x {1}", sizeString, sizeString),
                TooltipContent = string.Empty
            });
        }
    }
}

//PATCH: Allows locations to be created with min level 20 requirement
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

//PATCH: Allows locations to be created with max level 20 requirement
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
