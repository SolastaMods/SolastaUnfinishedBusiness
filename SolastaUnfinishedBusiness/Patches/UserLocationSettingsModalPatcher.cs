using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static SolastaUnfinishedBusiness.Models.DmProEditorContext;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UserLocationSettingsModalPatcher
{
    [HarmonyPatch(typeof(UserLocationSettingsModal), "RuntimeLoaded")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RuntimeLoaded_Patch
    {
        internal static void Postfix(UserLocationSettingsModal __instance)
        {
            //PATCH: adds custom dungeons sizes (DMP)
            if (!Main.Settings.EnableDungeonMakerModdedContent)
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

#if false
    //PATCH: Allows locations to be created with min level 20 requirement (DMP)
    [HarmonyPatch(typeof(UserLocationSettingsModal), "OnMinLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnMinLevelEndEdit_Patch
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

    //PATCH: Allows locations to be created with max level 20 requirement (DMP)
    [HarmonyPatch(typeof(UserLocationSettingsModal), "OnMaxLevelEndEdit")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnMaxLevelEndEdit_Patch
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
#endif
}
