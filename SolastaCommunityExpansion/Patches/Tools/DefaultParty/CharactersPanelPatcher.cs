using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty;

[HarmonyPatch(typeof(CharactersPanel), "EnumeratePlates")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharactersPanel_EnumeratePlates
{
    private static void Rebase(Transform parent, int max)
    {
        while (Main.Settings.defaultPartyHeroes.Count > max)
        {
            var heroToDelete = Main.Settings.defaultPartyHeroes.ElementAt(0);

            var child = parent.FindChildRecursive(heroToDelete);

            if (child)
            {
                child.GetComponentInChildren<Toggle>().isOn = false;
            }
        }
    }

    private static Transform CreateHeroCheckbox(Transform character)
    {
        var settingCheckboxItem = Resources.Load<GameObject>("Gui/Prefabs/Modal/Setting/SettingCheckboxItem");
        var smallToggleNoFrame = settingCheckboxItem.transform.Find("SmallToggleNoFrame");
        var checkBox = Object.Instantiate(smallToggleNoFrame, character.transform);
        var checkBoxRect = checkBox.GetComponent<RectTransform>();

        checkBox.name = "DefaultHeroToggle";
        checkBox.gameObject.SetActive(true);
        checkBox.Find("Background").gameObject.AddComponent<GuiTooltip>();

        checkBoxRect.anchoredPosition = new Vector2(160, 40);

        return checkBox;
    }

    internal static void Disable(RectTransform charactersTable)
    {
        for (var i = 0; i < charactersTable.childCount; i++)
        {
            var character = charactersTable.GetChild(i);
            var checkBoxToggle = character.GetComponentInChildren<Toggle>();

            if (checkBoxToggle)
            {
                checkBoxToggle.gameObject.SetActive(false);
            }
        }
    }

    internal static void Postfix(CharactersPanel __instance)
    {
        if (!Main.Settings.EnableTogglesToOverwriteDefaultTestParty)
        {
            Disable(__instance.charactersTable);

            return;
        }

        var max = Main.Settings.OverridePartySize;
        var characterPoolService = ServiceRepository.GetService<ICharacterPoolService>();

        Main.Settings.defaultPartyHeroes.RemoveAll(x => !characterPoolService.ContainsCharacter(x));

        for (var i = 0; i < __instance.charactersTable.childCount; i++)
        {
            var character = __instance.charactersTable.GetChild(i);
            var checkBox = character.Find("DefaultHeroToggle") ?? CreateHeroCheckbox(character);
            var tooltip = checkBox.Find("Background").GetComponentInChildren<GuiTooltip>();
            var checkBoxToggle = checkBox.GetComponentInChildren<Toggle>();

            tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
            tooltip.Content = Gui.Format("ToolTip/&CheckBoxDefaultPartyTitle", max.ToString());

            checkBoxToggle.gameObject.SetActive(true);
            checkBoxToggle.onValueChanged = new Toggle.ToggleEvent();
            checkBoxToggle.isOn = Main.Settings.defaultPartyHeroes.Contains(character.name);
            checkBoxToggle.onValueChanged.AddListener(delegate
            {
                if (checkBoxToggle.isOn)
                {
                    Main.Settings.defaultPartyHeroes.Add(character.name);
                    Rebase(character.parent.transform, max);
                }
                else
                {
                    Main.Settings.defaultPartyHeroes.Remove(character.name);
                }
            });
        }

        Rebase(__instance.charactersTable, max);
    }
}
