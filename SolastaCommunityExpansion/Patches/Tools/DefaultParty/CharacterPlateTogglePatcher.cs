using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.Tools.DefaultParty
{
    [HarmonyPatch(typeof(CharacterPlateToggle), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterPlateToggle_Bind
    {
        internal static void Postfix(CharacterPlateToggle __instance, Button ___deleteButton)
        {
            if (!Main.Settings.EnableTogglesToOverwriteDefaultTestParty || Global.IsNewAdventurePanelInContext)
            {
                return;
            }

            var name = __instance.Filename;
            var parent = ___deleteButton.transform.parent;
            var settingCheckboxItem = Resources.Load<GameObject>("Gui/Prefabs/Modal/Setting/SettingCheckboxItem");
            var smallToggleNoFrame = settingCheckboxItem.transform.Find("SmallToggleNoFrame");
            var checkBox = Object.Instantiate(smallToggleNoFrame, parent);
            var tooltip = checkBox.Find("Background").gameObject.AddComponent<GuiTooltip>();
            var checkBoxRect = checkBox.GetComponent<RectTransform>();
            var checkBoxToggle = checkBox.GetComponent<Toggle>();

            checkBox.name = name;
            checkBox.gameObject.SetActive(true);

            tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_CENTER;
            tooltip.Content = "ToolTip/&CheckBoxDefaultPartyTitle";
            tooltip.gameObject.SetActive(true);

            checkBoxRect.anchoredPosition = new Vector2(160, 40);

            checkBoxToggle.onValueChanged = new Toggle.ToggleEvent();
            checkBoxToggle.isOn = Main.Settings.TestPartyHeroes.Contains(name);
            checkBoxToggle.onValueChanged.AddListener(delegate
            {
                if (checkBoxToggle.isOn)
                {
                    Main.Settings.TestPartyHeroes.Add(name);

                    if (Main.Settings.TestPartyHeroes.Count > 4)
                    {
                        var heroToDelete = Main.Settings.TestPartyHeroes.ElementAt(0);

                        parent.parent.FindChildRecursive(heroToDelete)
                            .GetComponent<Toggle>().isOn = false;
                    }
                }
                else
                {
                    Main.Settings.TestPartyHeroes.Remove(name);
                }
            });
        }
    }
}
