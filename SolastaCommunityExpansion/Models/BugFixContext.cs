using UnityEngine;

namespace SolastaCommunityExpansion.Models;

internal static class BugFixContext
{
    internal static void Load()
    {
        //
        // BUGFIX: expand color tables
        //

        for (var i = 21; i < 33; i++)
        {
            Gui.ModifierColors.Add(i, new Color32(0, 164, byte.MaxValue, byte.MaxValue));
            Gui.CheckModifierColors.Add(i, new Color32(0, 36, 77, byte.MaxValue));
        }
    }
}
