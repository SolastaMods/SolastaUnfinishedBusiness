using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools;
// enables the character checker button
// enables the level down button on characters pool if hero above level 1

[HarmonyPatch(typeof(CharactersPanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_Refresh
{
    private static bool HasInit { get; set; }
    private static int SelectedPlate { get; set; }

    internal static void Postfix(CharactersPanel __instance)
    {
        if (Main.Settings.EnableCharacterChecker)
        {
            __instance.characterCheckerButton.GetComponentInChildren<GuiTooltip>().Content = string.Empty;
            __instance.characterCheckerButton.gameObject.SetActive(true);
            ;
        }

        if (Main.Settings.EnableRespec)
        {
            var characterLevel = __instance.selectedPlate >= 0
                ? __instance.characterPlates[__instance.selectedPlate].GuiCharacter.CharacterLevel
                : 1;

            SelectedPlate = __instance.selectedPlate;
            __instance.exportPdfButton.gameObject.SetActive(characterLevel > 1);

            if (HasInit)
            {
                return;
            }

            __instance.exportPdfButton.onClick.RemoveAllListeners();
            __instance.exportPdfButton.onClick.AddListener(() =>
            {
                LevelDownContext.ConfirmAndExecute(__instance.characterPlates[SelectedPlate].Filename);
            });

            HasInit = true;
        }
    }
}
