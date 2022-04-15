using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.CharacterCheker
{
    [HarmonyPatch(typeof(CharactersPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharactersPanel_Refresh
    {
        internal static void Postfix(Button ___characterCheckerButton)
        {
            if (!Main.Settings.EnableCharacterChecker)
            {
                return;
            }

            ___characterCheckerButton.GetComponentInChildren<GuiTooltip>().Content = string.Empty;
            ___characterCheckerButton.gameObject.SetActive(true);
        }
    }
}
