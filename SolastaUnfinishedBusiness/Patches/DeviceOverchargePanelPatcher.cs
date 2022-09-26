using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using static EquipmentDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

internal static class DeviceOverchargePanelPatcher
{
    [HarmonyPatch(typeof(DeviceOverchargePanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static bool Prefix(DeviceOverchargePanel __instance,
            RulesetItemDevice usableDevice,
            RulesetDeviceFunction usableDeviceFunction,
            DeviceOverchargeBox.OnActivateHandler onActivateAdvanced)
        {
            var functionDescription = usableDeviceFunction.DeviceFunctionDescription;

            if (functionDescription.type == DeviceFunctionDescription.FunctionType.Spell)
            {
                return true;
            }

            //TODO: implement custom overcharge amounts
            var useAmount = usableDeviceFunction.DeviceFunctionDescription.UseAmount;
            var num = usableDevice.RemainingCharges - useAmount;

            var boxesTable = __instance.overchargeBoxesTable;
            while (boxesTable.childCount < num)
            {
                Gui.GetPrefabFromPool(__instance.overchargetBoxPrefab, boxesTable);
            }

            for (var index = 0; index < boxesTable.childCount; ++index)
            {
                var child = boxesTable.GetChild(index);
                child.gameObject.SetActive(index < num);
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                var component = child.GetComponent<DeviceOverchargeBox>();
                var addedCharges = num - index;
                var lastChargeWarning = index == 0 &&
                                        usableDevice.UsableDeviceDescription.OutOfChargesConsequence !=
                                        ItemOutOfCharges.Persist;


                if (functionDescription.type == DeviceFunctionDescription.FunctionType.Spell)
                {
                    component.BindSlot(functionDescription.SpellDefinition, addedCharges,
                        lastChargeWarning, usableDevice.UsableDeviceDescription.OutOfChargesConsequence,
                        onActivateAdvanced);
                }
                else
                {
                    BindPowerSlot(component, functionDescription.FeatureDefinitionPower, addedCharges,
                        lastChargeWarning, usableDevice.UsableDeviceDescription.OutOfChargesConsequence,
                        onActivateAdvanced);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(boxesTable);
            var movingGroup = __instance.movingGroup;
            var size = boxesTable.rect.height + boxesTable.anchoredPosition.y;
            movingGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

            var rect = __instance.movingGroup.rect;
            __instance.movingPanelModifier.StartPosition = new Vector3(0.0f, -rect.height, 0.0f);
            __instance.movingPanelModifier.EndPosition = new Vector3(0.0f, 0.0f, 0.0f);
            __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);

            return false;
        }

        private static void BindPowerSlot(
            DeviceOverchargeBox box,
            FeatureDefinitionPower powerDefinition,
            int addedCharges,
            bool lastChargeWarning,
            ItemOutOfCharges lastChargeConsequence,
            DeviceOverchargeBox.OnActivateHandler onActivate)
        {
            box.OnActivate = onActivate;
            box.addedCharges = addedCharges;
            box.chargesLabel.Text = Gui.Format(box.addedCharges == 1
                ? "Equipment/&FunctionChargeCostFormatSingle"
                : "Equipment/&FunctionChargeCostFormatPlural", box.addedCharges.ToString("+0;-#"));
            box.tooltip.Content = Gui.Format(box.addedCharges == 1
                ? "Action/&UseFunctionAdditionalChargeSingle"
                : "Action/&UseFunctionAdditionalChargePlural", box.addedCharges.ToString());
            box.tooltip.Content += FormatEnhancementEffect(powerDefinition.EffectDescription, addedCharges);
            box.lastChargeGroup.gameObject.SetActive(lastChargeWarning);
            if (!lastChargeWarning)
            {
                return;
            }

            box.lastChargeTooltip.Content = Gui.Localize(lastChargeConsequence == ItemOutOfCharges.Destroy
                ? "Equipment/&DeviceLastChargeDestroyDescription"
                : "Equipment/&DeviceLastChargeDestroyOn1Description");

            box.tooltip.Content += "\n" + box.lastChargeTooltip.Content;
        }

        public static string FormatEnhancementEffect(EffectDescription effect, int slotDelta)
        {
            if (effect.EffectAdvancement.EffectIncrementMethod == RuleDefinitions.EffectIncrementMethod.None)
            {
                return string.Empty;
            }

            var result = string.Empty;
            var targetsBySlotDelta = effect.EffectAdvancement.ComputeAdditionalTargetsBySlotDelta(slotDelta);
            if (targetsBySlotDelta > 0)
            {
                result = result + "\n" +
                         Gui.Format(targetsBySlotDelta == 1
                             ? "Action/&CastSpellHigherSlotAddTargetSingleTile"
                             : "Action/&CastSpellHigherSlotAddTargetPluralTile", targetsBySlotDelta.ToString("+0;-#"));
            }

            var additionalDiceBySlotDelta = effect.EffectAdvancement.ComputeAdditionalDiceBySlotDelta(slotDelta);
            if (additionalDiceBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format(additionalDiceBySlotDelta == 1
                    ? "Action/&CastSpellHigherSlotAddDieTile"
                    : "Action/&CastSpellHigherSlotAddDiceTile", additionalDiceBySlotDelta.ToString("+0;-#"));
            }

            var levelBySlotDelta = effect.EffectAdvancement.ComputeAdditionalSpellLevelBySlotDelta(slotDelta);
            if (levelBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format(levelBySlotDelta == 1
                        ? "Action/&CastSpellHigherSlotAddSpellLevelPluralTile"
                        : "Action/&CastSpellHigherSlotAddSpellLevelSingleTile",
                    levelBySlotDelta.ToString("+0;-#"));
            }

            var summonsBySlotDelta = effect.EffectAdvancement.ComputeAdditionalSummonsBySlotDelta(slotDelta);
            if (summonsBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format(summonsBySlotDelta == 1
                        ? "Action/&CastSpellHigherSlotAddSummonPluralTile"
                        : "Action/&CastSpellHigherSlotAddSummonSingleTile",
                    summonsBySlotDelta.ToString("+0;-#"));
            }

            var additionalHpBySlotDelta = effect.EffectAdvancement.ComputeAdditionalHPBySlotDelta(slotDelta);
            if (additionalHpBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format("Action/&CastSpellHigherSlotAddHPTile",
                    additionalHpBySlotDelta.ToString("+0;-#"));
            }

            var tempHpBySlotDelta = effect.EffectAdvancement.ComputeAdditionalTempHPBySlotDelta(slotDelta);
            if (tempHpBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format("Action/&CastSpellHigherSlotAddTempHPTile",
                    tempHpBySlotDelta.ToString("+0;-#"));
            }

            var cellsBySlotDelta = effect.EffectAdvancement.ComputeAdditionalTargetCellsBySlotDelta(slotDelta);
            if (cellsBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format("Action/&CastSpellHigherSlotAddTargetCellsTile",
                    cellsBySlotDelta.ToString("+0;-#"));
            }

            var bonusBySlotDelta = effect.EffectAdvancement.ComputeAdditionalItemBonusBySlotDelta(slotDelta);
            if (bonusBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format("Action/&CastSpellHigherSlotAddItemBonusTile",
                    bonusBySlotDelta.ToString("+0;-#"));
            }

            effect.EffectAdvancement.ComputeAdditionalWeaponDieBySlotDelta(slotDelta);
            if (bonusBySlotDelta > 0)
            {
                result = result + "\n" + Gui.Format("Action/&CastSpellHigherSlotAddWeaponDieTile",
                    bonusBySlotDelta.ToString("+0;-#"));
            }

            return result;
        }
    }
}
