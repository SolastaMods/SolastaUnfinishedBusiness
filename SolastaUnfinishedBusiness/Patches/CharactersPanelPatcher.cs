using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharactersPanelPatcher
{
    //PATCH: Keeps last level up hero selected
    [HarmonyPatch(typeof(CharactersPanel), nameof(CharactersPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharactersPanel __instance, out float __state)
        {
            // Remember the current scroll position
            __state = __instance.charactersScrollview.verticalNormalizedPosition;
        }

        [UsedImplicitly]
        public static void Postfix(CharactersPanel __instance, float __state)
        {
            if (Global.LastLevelUpHeroName == null)
            {
                return;
            }

            __instance.OnSelectPlate(
                __instance.characterPlates.Find(x => x.GuiCharacter.Name == Global.LastLevelUpHeroName));

            Global.LastLevelUpHeroName = null;

            // Reset the scroll position because OnBeginShow sets it to 1.0f.
            // This keeps the selected character in view unless the panel is sorted by level
            // in which case the character will move.

            __instance.charactersScrollview.verticalNormalizedPosition = __state;
        }
    }

    //PATCH: add checkboxes to heroes plate to allow heroes pre-selection (DEFAULT_PARTY)
    [HarmonyPatch(typeof(CharactersPanel), nameof(CharactersPanel.EnumeratePlates))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnumeratePlates_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharactersPanel __instance)
        {
            if (!Main.Settings.EnableTogglesToOverwriteDefaultTestParty)
            {
                ToolsContext.Disable(__instance.charactersTable);

                return;
            }

            var max = Main.Settings.OverridePartySize;
            var characterPoolService = ServiceRepository.GetService<ICharacterPoolService>();

            Main.Settings.DefaultPartyHeroes.RemoveAll(x => !characterPoolService.ContainsCharacter(x));

            for (var i = 0; i < __instance.charactersTable.childCount; i++)
            {
                var character = __instance.charactersTable.GetChild(i);
                var checkBox = character.Find("DefaultHeroToggle") ?? ToolsContext.CreateHeroCheckbox(character);
                var tooltip = checkBox.Find("Background").GetComponentInChildren<GuiTooltip>();
                var checkBoxToggle = checkBox.GetComponentInChildren<Toggle>();

                tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
                tooltip.Content = Gui.Format("ToolTip/&CheckBoxDefaultPartyTitle", max.ToString());

                checkBoxToggle.gameObject.SetActive(true);
                checkBoxToggle.onValueChanged = new Toggle.ToggleEvent();
                checkBoxToggle.isOn = Main.Settings.DefaultPartyHeroes.Contains(character.name);
                checkBoxToggle.onValueChanged.AddListener(delegate
                {
                    if (checkBoxToggle.isOn)
                    {
                        Main.Settings.DefaultPartyHeroes.Add(character.name);
                        ToolsContext.Rebase(character.parent.transform, max);
                    }
                    else
                    {
                        Main.Settings.DefaultPartyHeroes.Remove(character.name);
                    }
                });
            }

            ToolsContext.Rebase(__instance.charactersTable, max);
        }
    }

    //PATCH: enable the character checker button on main > characters
    [HarmonyPatch(typeof(CharactersPanel), nameof(CharactersPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharactersPanel __instance)
        {
            __instance.characterCheckerButton.gameObject.SetActive(Main.Settings.EnableCharacterChecker);
        }
    }
}
