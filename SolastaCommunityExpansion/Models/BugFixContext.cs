using UnityEngine;

namespace SolastaCommunityExpansion.Models;

internal static class BugFixContext
{
    internal static void Load()
    {
        if (Main.Settings.BugFixExpandColorTables)
        {
            Gui.ModifierColors.Add(21, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(22, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(23, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(24, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(25, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(26, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(27, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(28, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(29, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(30, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(31, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.ModifierColors.Add(32, new Color32(0, 164, byte.MaxValue, byte.MaxValue));

            Gui.CheckModifierColors.Add(21, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(22, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(23, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(24, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(25, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(26, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(27, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(28, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(29, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(30, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(31, new Color32(0, 36, 77, byte.MaxValue));
            Gui.CheckModifierColors.Add(32, new Color32(0, 36, 77, byte.MaxValue));
        }
    }
}
