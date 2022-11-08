using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomBehaviors;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

public static class InvocationActivationBoxPatcher
{
    const string TABLE_NAME = "SlotStatusTable";
    const string HIGH_SLOTS_NAME = "HighSlotNumber";

    [HarmonyPatch(typeof(InvocationActivationBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
        public static void Postfix(InvocationActivationBox __instance,
            RulesetInvocation invocation,
            RulesetCharacter activator)
        {
            //PATCH: make sure hidden invocations are indeed hidden and not interactable
            if (__instance.Invocation.invocationDefinition.HasSubFeatureOfType<Hidden>())
            {
                __instance.gameObject.SetActive(false);
                __instance.button.interactable = false;
            }
            else
            {
                __instance.gameObject.SetActive(true);
                //PATCH: Show power use slots for invocations that grant powers
                UpdatePowerSlots(__instance, invocation, activator);
            }
        }

        private static GameObject slotPrefab;

        private static void UpdatePowerSlots(InvocationActivationBox box, RulesetInvocation invocation,
            RulesetCharacter character)
        {
            var power = invocation.invocationDefinition.GrantedFeature as FeatureDefinitionPower;

            if (power == null) { return; }

            var atWill = power.rechargeRate == RuleDefinitions.RechargeRate.AtWill;
            box.infinitySymbol.gameObject.SetActive(atWill);

            if (atWill) { return; }

            box.gameObject.SetActive(false);

            var boxRect = box.rectTransform;
            var slotTable = boxRect.Find(TABLE_NAME);
            var highTransform = boxRect.Find(HIGH_SLOTS_NAME);

            RectTransform tableTransform;
            GuiLabel highSlots;
            if (slotTable == null)
            {
                var panel = Gui.GuiService.GetScreen<PowerSelectionPanel>();
                var powerBox = panel.usablePowerPrefab.GetComponent<UsablePowerBox>();

                slotPrefab = powerBox.slotStatusPrefab;

                var table = Object.Instantiate(powerBox.slotStatusTable.gameObject, boxRect);
                table.name = TABLE_NAME;
                var rect = table.GetComponent<RectTransform>();
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 5, 0);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 10, 0);
                tableTransform = rect;

                highTransform = powerBox.transform.Find("Header/HighSlotNumber");
                var high = Object.Instantiate(highTransform.gameObject, boxRect);
                high.name = HIGH_SLOTS_NAME;
                rect = high.GetComponent<RectTransform>();
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 25, 0);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 10, 0);
                highSlots = high.GetComponent<GuiLabel>();
            }
            else
            {
                tableTransform = slotTable.GetComponent<RectTransform>();
                highSlots = highTransform.GetComponent<GuiLabel>();
            }

            var usablePower = UsablePowersProvider.Get(power, character);

            int maxUses = character.GetMaxUsesOfPower(usablePower);
            int remainingUses = character.GetRemainingUsesOfPower(usablePower);
            bool powerUsesPoints = character.IsPowerUsingPool(usablePower);
            bool powerUsesSlots = character.IsPowerUsingSlots(usablePower);
            bool manySlots = maxUses > 5 || powerUsesPoints;

            highSlots.gameObject.SetActive(manySlots);
            if (manySlots)
            {
                highSlots.Text = remainingUses.ToString();
                remainingUses = remainingUses == 0 ? 0 : 1;
                maxUses = 1;
            }

            tableTransform.gameObject.SetActive(true);
            while (tableTransform.childCount < maxUses)
            {
                Gui.GetPrefabFromPool(slotPrefab, tableTransform);
            }

            for (int index = 0; index < maxUses; ++index)
            {
                Transform child = tableTransform.GetChild(index);
                child.gameObject.SetActive(true);
                SlotStatus status = child.GetComponent<SlotStatus>();
                status.Used.gameObject.SetActive(index >= remainingUses);
                status.Available.gameObject.SetActive(index < remainingUses);
            }

            if (maxUses >= 0)
            {
                for (int index = maxUses; index < tableTransform.childCount; ++index)
                {
                    tableTransform.GetChild(index).gameObject.SetActive(false);
                }
            }

            var tooltip = tableTransform.GetComponent<GuiTooltip>();
            tooltip.anchorMode = TooltipDefinitions.AnchorMode.FREE;
            if (manySlots)
            {
                tooltip.Content = !powerUsesPoints
                    ? (!powerUsesSlots ? string.Empty : "Screen/&PowerRemainingSlotsDescription")
                    : "Screen/&PowerRemainingHealingPoolDescription";
                highSlots.GetComponent<GuiTooltip>().Content = tooltip.Content;
            }
            else if (remainingUses == 0)
            {
                tooltip.Content = UsablePowerBox.PowerUsedAllDescription;
            }
            else if (remainingUses == maxUses)
            {
                tooltip.Content = "Screen/&PowerUsedNoneDescription";
            }
            else
            {
                tooltip.Content = Gui.FormatWithHighlight("Screen/&PowerUsedSomeDescription",
                    (maxUses - remainingUses).ToString());
            }

            box.gameObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(boxRect);
        }
    }

    [HarmonyPatch(typeof(InvocationActivationBox), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Unbind_Patch
    {
        public static void Prefix(InvocationActivationBox __instance)
        {
            //PATCH: clean up custom widgets added for power invocations
            var boxRect = __instance.rectTransform;

            var slotTable = boxRect.Find(TABLE_NAME);
            if (slotTable != null)
            {
                slotTable.gameObject.SetActive(false);
            }

            var highTransform = boxRect.Find(HIGH_SLOTS_NAME);
            if (highTransform != null)
            {
                highTransform.gameObject.SetActive(false);
            }
        }
    }
}
