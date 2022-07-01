using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

internal static class PowerSelectionPanelPatcher
{
    private static RectTransform secondRow;
    private static RectTransform thirdRow;

    // second line bind
    [HarmonyPatch(typeof(PowerSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PowerSelectionPanel_Bind
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var power_canceled_handler = codes.FindIndex(x =>
                x.opcode == OpCodes.Call && x.operand.ToString().Contains("PowerCancelled"));

            var removePowersMethod = new Action<PowerSelectionPanel, RulesetCharacter>(RemoveInvalidPowers).Method;

            codes.InsertRange(power_canceled_handler + 1,
                new List<CodeInstruction>
                {
                    new(OpCodes.Ldarg_0), new(OpCodes.Ldarg_1), new(OpCodes.Call, removePowersMethod)
                }
            );
            return codes.AsEnumerable();
        }

        private static void RemoveInvalidPowers(PowerSelectionPanel panel, RulesetCharacter character)
        {
            var relevantPowers = panel.relevantPowers;

            for (var i = relevantPowers.Count - 1; i >= 0; i--)
            {
                var power = relevantPowers[i];
                var validator = power.PowerDefinition.GetFirstSubFeatureOfType<IPowerUseValidity>();
                if (validator != null && !validator.CanUsePower(character))
                {
                    relevantPowers.RemoveAt(i);
                }
            }
        }

        internal static void Postfix(PowerSelectionPanel __instance)
        {
            if (!Main.Settings.EnableMultiLinePowerPanel)
            {
                return;
            }

            var powerBoxes = __instance.usablePowerBoxes;
            var powersTable = __instance.powersTable;
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
            __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                powersTable.rect.width);
        }

        private static void MovePowersToRow(RectTransform powersTable, RectTransform newRow, int toStayCount,
            int yOffset)
        {
            var position = powersTable.transform.position;

            newRow.gameObject.SetActive(true);
            newRow.DetachChildren();
            newRow.SetParent(powersTable.parent.transform, true);
            newRow.localScale = powersTable.localScale;
            newRow.transform.position = new Vector3(position.x, position.y + yOffset, position.z);

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

            if (thirdRow == null || !thirdRow.gameObject.activeSelf)
            {
                return;
            }

            Gui.ReleaseChildrenToPool(thirdRow);
            thirdRow.gameObject.SetActive(false);
        }
    }
}
