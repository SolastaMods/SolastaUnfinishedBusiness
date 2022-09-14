#if false
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using TMPro;

namespace SolastaUnfinishedBusiness.Patches.Tools.Respec;

//PATCH: enables the level down button on characters pool if hero above level 1
[HarmonyPatch(typeof(CharactersPanel), "Refresh")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_Refresh
{
    private static bool HasInit { get; set; }
    private static int SelectedPlate { get; set; }

    internal static void Postfix(CharactersPanel __instance)
    {
        // RESPEC setting controls both Respec and Level Down offerings
        if (!Main.Settings.EnableRespec)
        {
            return;
        }

        SelectedPlate = __instance.selectedPlate;

        var characterLevel = SelectedPlate >= 0
            ? __instance.characterPlates[SelectedPlate].GuiCharacter.CharacterLevel
            : 1;

        __instance.characterCheckerButton.gameObject.SetActive(characterLevel > 1);

        if (HasInit)
        {
            return;
        }

        __instance.characterCheckerButton.GetComponentInChildren<TextMeshProUGUI>().text =
            Gui.Localize("MainMenu/&LevelDownTitle");
        __instance.characterCheckerButton.GetComponentInChildren<GuiTooltip>().Content =
            Gui.Localize("MainMenu/&LevelDownDescription");
        __instance.characterCheckerButton.onClick.RemoveAllListeners();
        __instance.characterCheckerButton.onClick.AddListener(() =>
        {
            LevelDownContext.ConfirmAndExecute(__instance.characterPlates[SelectedPlate].Filename);
        });

        HasInit = true;
    }
}

//PATCH: blocks this from happening if we're using this button for Level Down
[HarmonyPatch(typeof(CharactersPanel), "OnCharacterCheckerCb")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_OnCharacterCheckerCb
{
    internal static bool Prefix()
    {
        return !Main.Settings.EnableRespec;
    }
}
#endif
