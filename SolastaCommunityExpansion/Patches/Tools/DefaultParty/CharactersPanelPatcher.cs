using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(CharactersPanel), "EnumeratePlates")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharactersPanel_EnumeratePlates
    {
        private static void Rebase(Transform parent)
        {
            while (Main.Settings.DefaultPartyHeroes.Count > Main.Settings.OverridePartySize)
            {
                var heroToDelete = Main.Settings.DefaultPartyHeroes.ElementAt(0);

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
            var tooltip = checkBox.Find("Background").gameObject.AddComponent<GuiTooltip>();
            var checkBoxRect = checkBox.GetComponent<RectTransform>();

            checkBox.name = "DefaultHeroToggle";
            checkBox.gameObject.SetActive(true);

            tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
            tooltip.Content = Gui.Format("ToolTip/&CheckBoxDefaultPartyTitle", Main.Settings.OverridePartySize.ToString());
            tooltip.gameObject.SetActive(true);

            checkBoxRect.anchoredPosition = new Vector2(160, 40);

            return checkBox;
        }

        internal static void Postfix(RectTransform ___charactersTable)
        {
            var characterPoolService = ServiceRepository.GetService<ICharacterPoolService>();

            Main.Settings.DefaultPartyHeroes.RemoveAll(x => !characterPoolService.ContainsCharacter(x));

            for (var i = 0; i < ___charactersTable.childCount; i++)
            {
                var character = ___charactersTable.GetChild(i);
                var checkBoxToggle = character.GetComponentInChildren<Toggle>();

                if (!checkBoxToggle)
                {
                    checkBoxToggle = CreateHeroCheckbox(character).GetComponent<Toggle>();
                }

                checkBoxToggle.gameObject.SetActive(true);
                checkBoxToggle.onValueChanged = new Toggle.ToggleEvent();
                checkBoxToggle.isOn = Main.Settings.DefaultPartyHeroes.Contains(character.name);
                checkBoxToggle.onValueChanged.AddListener(delegate
                {
                    if (checkBoxToggle.isOn)
                    {
                        Main.Settings.DefaultPartyHeroes.Add(character.name);
                        Rebase(character.parent.transform);
                    }
                    else
                    {
                        Main.Settings.DefaultPartyHeroes.Remove(character.name);
                    }
                });
            }

            Rebase(___charactersTable);
        }
    }
}
