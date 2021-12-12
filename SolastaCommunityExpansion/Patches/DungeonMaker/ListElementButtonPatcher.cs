using HarmonyLib;

using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.DungeonEditor
{
    // better tooltips on DM selected items
    [HarmonyPatch(typeof(ListElementButton), "BindItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ListElementButtonBindItem
    {
        internal static void Postfix(GuiLabel ___nameLabel)
        {
            if (Main.Settings.DungeonMakerEditorBetterTooltips)
            {
                var guiTooltip = ___nameLabel.gameObject.AddComponent<GuiTooltip>();

                guiTooltip.Content = Gui.Format("Caption/&GadgetParametersCustomRemoveDescription", ___nameLabel.Text);
            }
        }
    }
}
