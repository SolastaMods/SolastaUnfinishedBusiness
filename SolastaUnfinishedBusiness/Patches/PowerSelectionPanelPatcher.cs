using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Patches;

public static class PowerSelectionPanelPatcher
{
    private static RectTransform _secondRow;
    private static RectTransform _thirdRow;

    [HarmonyPatch(typeof(PowerSelectionPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        [NotNull]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var setPowerCancelledMethod = typeof(PowerSelectionPanel).GetMethod("set_PowerCancelled");
            var removeInvalidPowersMethod =
                new Action<PowerSelectionPanel, RulesetCharacter>(RemoveInvalidPowers).Method;

            return instructions.ReplaceCall(
                setPowerCancelledMethod,
                1,
                "PowerSelectionPanel.Bind",
                new CodeInstruction(OpCodes.Call, setPowerCancelledMethod), // checked for Call vs CallVirtual
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, removeInvalidPowersMethod));
        }

        //PATCH: remove invalid powers from display using our custom validators
        private static void RemoveInvalidPowers(PowerSelectionPanel panel, RulesetCharacter character)
        {
            var usablePowers = character.UsablePowers;
            var relevantPowers = panel.relevantPowers;
            var actionType = panel.ActionType;

            foreach (var power in usablePowers)
            {
                var feature = power.PowerDefinition.GetFirstSubFeatureOfType<PowerVisibilityModifier>();

                if (feature == null)
                {
                    continue;
                }

                if (feature.IsVisible(character, power.PowerDefinition, actionType))
                {
                    relevantPowers.TryAdd(power);
                }
            }

            for (var i = relevantPowers.Count - 1; i >= 0; i--)
            {
                var power = relevantPowers[i];

                if (ValidatorsPowerUse.IsPowerNotValid(character, power)
                    || PowerVisibilityModifier.IsPowerHidden(character, power, actionType))
                {
                    relevantPowers.RemoveAt(i);
                }
            }
        }

        //PATCH: add additional rows to powers (EnableMultiLinePowerPanel)
        public static void Postfix(PowerSelectionPanel __instance)
        {
            if (!Main.Settings.EnableMultiLinePowerPanel)
            {
                return;
            }

            var powerBoxes = __instance.usablePowerBoxes;
            var powersTable = __instance.powersTable;

            if (powerBoxes.Count > 14)
            {
                if (_thirdRow == null)
                {
                    _thirdRow = Object.Instantiate(powersTable);
                }

                var toStayCount = powersTable.childCount * 2 / 3;

                MovePowersToRow(powersTable, _thirdRow, toStayCount, 200);
            }

            if (powerBoxes.Count > 7)
            {
                if (_secondRow == null)
                {
                    _secondRow = Object.Instantiate(powersTable);
                }

                var toStayCount = powersTable.childCount / 2;

                MovePowersToRow(powersTable, _secondRow, toStayCount, 80);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(powersTable);
            __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, powersTable.rect.width);
        }

        private static void MovePowersToRow(
            Transform powersTable,
            RectTransform newRow,
            int toStayCount,
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

    [HarmonyPatch(typeof(PowerSelectionPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Unbind_Patch
    {
        public static void Postfix()
        {
            //PATCH: add additional rows to powers (EnableMultiLinePowerPanel)
            if (!Main.Settings.EnableMultiLinePowerPanel)
            {
                return;
            }

            if (_secondRow != null && _secondRow.gameObject.activeSelf)
            {
                Gui.ReleaseChildrenToPool(_secondRow);
                _secondRow.gameObject.SetActive(false);
            }

            if (_thirdRow == null || !_thirdRow.gameObject.activeSelf)
            {
                return;
            }

            Gui.ReleaseChildrenToPool(_thirdRow);
            _thirdRow.gameObject.SetActive(false);
        }
    }
}
