using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    internal static class PowerSelectionPanelPatcher
    {
        private static RectTransform secondRow;
        private static RectTransform thirdRow;

        // second line bind
        [HarmonyPatch(typeof(PowerSelectionPanel), "Bind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class PowerSelectionPanel_Bind
        {
            internal static void Postfix(PowerSelectionPanel __instance)
            {
                if (!Main.Settings.EnableMultiLinePowerPanel)
                {
                    return;
                }
                var powerBoxes = __instance.GetField<List<UsablePowerBox>>("usablePowerBoxes");
                var powersTable = __instance.GetField<RectTransform>("powersTable");
                if (powerBoxes.Count > 14)
                {
                    if (thirdRow == null)
                    {
                        thirdRow = Object.Instantiate(powersTable);
                    }
                    var toStayCount = powersTable.childCount * 2 / 3;
                    MovePowersToRow(powersTable, thirdRow, toStayCount, 200);
                }
                if (powerBoxes.Count > 7)
                {
                    if (secondRow == null)
                    {
                        secondRow = Object.Instantiate(powersTable);
                    }
                    var toStayCount = powersTable.childCount / 2;
                    MovePowersToRow(powersTable, secondRow, toStayCount, 80);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(powersTable);
                __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, powersTable.rect.width);
            }

            private static void MovePowersToRow(RectTransform powersTable, RectTransform newRow, int toStayCount, int yOffset)
            {
                newRow.gameObject.SetActive(true);
                newRow.DetachChildren();
                newRow.SetParent(powersTable.parent.transform, true);
                newRow.localScale = powersTable.localScale;
                newRow.transform.position = new Vector3(powersTable.transform.position.x, powersTable.transform.position.y + yOffset, powersTable.transform.position.z);
                for (var i = powersTable.childCount - 1; i > toStayCount; i--)
                {
                    var child = powersTable.GetChild(i);
                    child.SetParent(newRow, false);
                    child.localScale = powersTable.GetChild(0).localScale;
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(newRow);
            }
        }

        // second line unbind
        [HarmonyPatch(typeof(PowerSelectionPanel), "Unbind")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class PowerSelectionPanel_Unbind
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMultiLinePowerPanel)
                {
                    return;
                }

                if (secondRow != null && secondRow.gameObject.activeSelf)
                {
                    Gui.ReleaseChildrenToPool(secondRow);
                    secondRow.gameObject.SetActive(false);
                }

                if (thirdRow != null && thirdRow.gameObject.activeSelf)
                {
                    Gui.ReleaseChildrenToPool(thirdRow);
                    thirdRow.gameObject.SetActive(false);
                }
            }
        }
    }
}
