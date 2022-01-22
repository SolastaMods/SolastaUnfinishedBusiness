using HarmonyLib;
using ModKit;
using System.Collections.Generic;
using System.Reflection.Emit;
using TMPro;
using static SolastaCommunityExpansion.Models.DmProEditorContext;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    internal static class UserLocationSettingsModalPatcher
    {
        // unlocks visual moods across all environments
        [HarmonyPatch(typeof(UserLocationSettingsModal), "RefreshVisualMoods")]
        internal static class UserLocationSettingsModalRefreshVisualMoods
        {
            internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int found = 0;

                foreach (CodeInstruction instruction in instructions)
                {
                    if (!Main.Settings.EnableDungeonMakerPro)
                    {
                        yield return instruction;
                    }
                    else if (Main.Settings.EnableDungeonMakerModdedContent && instruction.opcode == OpCodes.Brfalse_S && ++found == 1)
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
        internal static class UserLocationSettingsModalRuntimeLoaded
        {
            internal static void Postfix(UserLocationSettingsModal __instance, List<TMP_Dropdown.OptionData> ___optionsListSize)
            {
                if (!Main.Settings.EnableDungeonMakerPro || !Main.Settings.EnableDungeonMakerModdedContent)
                {
                    return;
                }

                for (var size = ExtendedDungeonSize.Huge; size <= ExtendedDungeonSize.Gargantuan; size++)
                {
                    var sizeString = UserLocationDefinitions.CellsBySize[(UserLocationDefinitions.Size)size].ToString();

                    ___optionsListSize.Add(new GuiDropdown.OptionDataAdvanced
                    {
                        text = Gui.FormatLocationSize((UserLocationDefinitions.Size)size).yellow() + " " + Gui.Format("{0} x {1}", sizeString, sizeString),
                        TooltipContent = string.Empty
                    });
                }
            }
        }
    }
}
