using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
using static SolastaCommunityExpansion.Models.DmProEditorContext;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro;

// unlocks visual moods across all environments
[HarmonyPatch(typeof(UserLocationSettingsModal), "RefreshVisualMoods")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserLocationSettingsModal_RefreshVisualMoods
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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

// adds custom dungeons sizes
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
                text = Gui.FormatLocationSize((UserLocationDefinitions.Size)size).Yellow() + " " +
                       Gui.Format("{0} x {1}", sizeString, sizeString),
                TooltipContent = string.Empty
            });
        }
    }
}
