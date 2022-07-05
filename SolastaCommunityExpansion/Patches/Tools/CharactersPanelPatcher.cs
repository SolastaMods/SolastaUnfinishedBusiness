using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using TMPro;

namespace SolastaCommunityExpansion.Patches.Tools;
// enables the character checker button
// enables the level down button on characters pool if hero above level 1

[HarmonyPatch(typeof(CharactersPanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_Refresh
{
    private static bool HasInit { get; set; }

    internal static void Postfix(CharactersPanel __instance)
    {
        // RESPEC setting controls both Respec and Level Down offerings
        if (!Main.Settings.EnableRespec)
        {
            return;
        }

        var selectedPlate = __instance.selectedPlate;
        var characterLevel = selectedPlate >= 0
            ? __instance.characterPlates[selectedPlate].GuiCharacter.CharacterLevel
            : 1;

        __instance.characterCheckerButton.gameObject.SetActive(characterLevel > 1);

        if (HasInit)
        {
            return;
        }

        __instance.characterCheckerButton.GetComponentInChildren<TextMeshProUGUI>().text = "MainMenu/&LevelDownTitle";
        __instance.characterCheckerButton.GetComponentInChildren<GuiTooltip>().Content =
            "MainMenu/&LevelDownDescription";
        __instance.characterCheckerButton.onClick.RemoveAllListeners();
        __instance.characterCheckerButton.onClick.AddListener(() =>
        {
            LevelDownContext.ConfirmAndExecute(__instance.characterPlates[selectedPlate].Filename);
        });

        HasInit = true;
    }
}
