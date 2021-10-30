
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCJDExtraContent.Patches
{
    class PowerSelectionPanelPatcher
    {
        private static RectTransform secondRow;
        private static RectTransform thirdRow;

        [HarmonyPatch(typeof(PowerSelectionPanel), "Bind")]
        internal static class PowerSelectionPanel_SecondLine
        {
            internal static void Postfix(PowerSelectionPanel __instance)
            {
                List<UsablePowerBox> powerBoxes = (List<UsablePowerBox>)Traverse.Create(__instance).Field("usablePowerBoxes").GetValue();
                RectTransform powersTable = (RectTransform)Traverse.Create(__instance).Field("powersTable").GetValue();
                if (powerBoxes.Count > 14)
                {
                    if (thirdRow == null)
                    {
                        thirdRow = GameObject.Instantiate(powersTable);
                    }
                    int toStayCount = powersTable.childCount * 2 / 3;
                    MovePowersToRow(powersTable, thirdRow, toStayCount, 200);
                }
                if (powerBoxes.Count > 7)
                {
                    if (secondRow == null)
                    {
                        secondRow = GameObject.Instantiate(powersTable);
                    }
                    int toStayCount = powersTable.childCount / 2;
                    MovePowersToRow(powersTable, secondRow, toStayCount, 80);
                }
                float height = __instance.transform.parent.GetComponent<RectTransform>().rect.height;
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
                for (int i = powersTable.childCount - 1; i > toStayCount; i--)
                {
                    Transform child = powersTable.GetChild(i);
                    child.SetParent(newRow, false);
                    child.localScale = powersTable.GetChild(0).localScale;

                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(newRow);
            }
        }

        [HarmonyPatch(typeof(PowerSelectionPanel), "Unbind")]
        internal static class PowerSelectionPanel_SecondLineUnbind
        {
            internal static void Postfix(PowerSelectionPanel __instance)
            {
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
