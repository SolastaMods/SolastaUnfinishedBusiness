using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Definitions;
using UnityEngine;
using UnityEngine.UI;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class InvocationActivationBoxPatcher
{
    private const string TableName = "SlotStatusTable";
    private const string HighSlotsName = "HighSlotNumber";

    private static void DisablePowerUseSlots(GuiBehaviour box)
    {
        var boxRect = box.rectTransform;

        var slotTable = boxRect.Find(TableName);

        // don't use null propagation on unity objects
        if (slotTable != null)
        {
            slotTable.gameObject.SetActive(false);
        }

        var highTransform = boxRect.Find(HighSlotsName);

        // don't use null propagation on unity objects
        if (highTransform != null)
        {
            highTransform.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(InvocationActivationBox), nameof(InvocationActivationBox.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        private static GameObject _slotPrefab;

        [UsedImplicitly]
        public static void Postfix(
            InvocationActivationBox __instance,
            RulesetInvocation invocation,
            RulesetCharacter activator)
        {
            //PATCH: clean up custom widgets added for power invocations
            DisablePowerUseSlots(__instance);

            var definition = invocation.InvocationDefinition;
            var isValid = definition
                .GetAllSubFeaturesOfType<IsInvocationValidHandler>()
                .All(v => v(activator, definition));

            //PATCH: make sure hidden invocations are indeed hidden and not interactable
            if (__instance.Invocation.invocationDefinition.HasSubFeatureOfType<ModifyInvocationVisibility>() ||
                !isValid)
            {
                __instance.gameObject.SetActive(false);
                __instance.button.interactable = false;
            }
            else
            {
                __instance.gameObject.SetActive(true);
                //PATCH: make custom tooltip
                SetupCustomTooltip(__instance, invocation, activator);

                //PATCH: Show power use slots for invocations that grant powers
                UpdatePowerSlots(__instance, invocation, activator);

                //PATCH: support for invocations that recharge on short rest (like Fey Teleportation feat)
                UpdateUsedState(__instance, invocation, activator);
            }
        }

        private static void UpdateUsedState(InvocationActivationBox box, RulesetInvocation invocation,
            RulesetCharacter activator)
        {
            if (!invocation.InvocationDefinition.HasSubFeatureOfType<RechargeInvocationOnShortRest>())
            {
                return;
            }

            var restSymbol = box.restSymbol;

            restSymbol.gameObject.SetActive(true);
            box.infinitySymbol.gameObject.SetActive(false);

            restSymbol.color = invocation.Used ? box.unavailableColor : box.availableColor;
            box.restSymbolTooltip.Content = invocation.Used
                ? "Tooltip/&InvocationUsageShortRestUsedDescription"
                : "Tooltip/&InvocationUsageShortRestAvailableDescription";

            var available = !invocation.Used && activator.CanCastInvocation(invocation);

            box.button.interactable = available;
            box.canvasGroup.interactable = available;
            box.icon.material = available ? box.availableMaterial : box.unavailableMaterial;
            box.icon.color = available ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }

        private static void SetupCustomTooltip(
            InvocationActivationBox instance,
            RulesetInvocation invocation,
            RulesetCharacter character)
        {
            var feature = invocation.invocationDefinition as InvocationDefinitionCustom;

            if (feature == null || feature.PoolType == null || feature.grantedSpell != null)
            {
                return;
            }

            var tooltip = instance.tooptip;
            var gui = new GuiPresentationBuilder(feature.GuiPresentation).Build();
            var item = feature.Item;
            var dataProvider = item == null
                ? new CustomTooltipProvider(feature, gui)
                : new CustomItemTooltipProvider(feature, gui, item);

            tooltip.TooltipClass = dataProvider.TooltipClass;
            tooltip.Content = feature.GuiPresentation.Description;
            tooltip.Context = character;
            tooltip.DataProvider = dataProvider;
        }

        private static void UpdatePowerSlots(InvocationActivationBox box, RulesetInvocation invocation,
            RulesetCharacter character)
        {
            var power = invocation.invocationDefinition.GetPower();

            if (power == null)
            {
                return;
            }

            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiPowerDefinition(power.Name).SetupTooltip(box.spellTooltip, character);

            var atWill = power.rechargeRate == RechargeRate.AtWill;

            box.infinitySymbol.gameObject.SetActive(atWill);

            if (atWill)
            {
                return;
            }

            var boxRect = box.rectTransform;
            var slotTable = boxRect.Find(TableName);
            var highTransform = boxRect.Find(HighSlotsName);

            RectTransform tableTransform;
            GuiLabel highSlots;

            if (slotTable == null)
            {
                var panel = Gui.GuiService.GetScreen<PowerSelectionPanel>();
                var powerBox = panel.usablePowerPrefab.GetComponent<UsablePowerBox>();

                _slotPrefab = powerBox.slotStatusPrefab;

                var table = Object.Instantiate(powerBox.slotStatusTable.gameObject, boxRect);

                table.name = TableName;

                var rect = table.GetComponent<RectTransform>();
                var position = box.infinitySymbol.transform.position;

                rect.position = new Vector3(rect.position.x, position.y, 0);
                tableTransform = rect;

                highTransform = powerBox.transform.Find("Header/HighSlotNumber");

                var high = Object.Instantiate(highTransform.gameObject, boxRect);

                high.name = HighSlotsName;

                var rect2 = high.GetComponent<RectTransform>();

                rect2.position = new Vector3(rect2.position.x, position.y, 0);
                highSlots = high.GetComponent<GuiLabel>();
            }
            else
            {
                tableTransform = slotTable.GetComponent<RectTransform>();
                highSlots = highTransform.GetComponent<GuiLabel>();
            }

            var usablePower = PowerProvider.Get(power, character);
            var maxUses = character.GetMaxUsesOfPower(usablePower);
            var remainingUses = character.GetRemainingUsesOfPower(usablePower);
            var powerUsesPoints = character.IsPowerUsingPool(usablePower);
            var powerUsesSlots = character.IsPowerUsingSlots(usablePower);
            var manySlots = maxUses > 5 || powerUsesPoints;

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
                Gui.GetPrefabFromPool(_slotPrefab, tableTransform);
            }

            for (var index = 0; index < maxUses; ++index)
            {
                var child = tableTransform.GetChild(index);

                child.gameObject.SetActive(true);

                var status = child.GetComponent<SlotStatus>();

                status.Used.gameObject.SetActive(index >= remainingUses);
                status.Available.gameObject.SetActive(index < remainingUses);
            }

            if (maxUses >= 0)
            {
                for (var index = maxUses; index < tableTransform.childCount; ++index)
                {
                    tableTransform.GetChild(index).gameObject.SetActive(false);
                }
            }

            var tooltip = tableTransform.GetComponent<GuiTooltip>();

            tooltip.anchorMode = TooltipDefinitions.AnchorMode.FREE;

            if (manySlots)
            {
                tooltip.Content = !powerUsesPoints
                    ? !powerUsesSlots ? string.Empty : "Screen/&PowerRemainingSlotsDescription"
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

            LayoutRebuilder.ForceRebuildLayoutImmediate(boxRect);
        }
    }

    [HarmonyPatch(typeof(InvocationActivationBox), nameof(InvocationActivationBox.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(InvocationActivationBox __instance)
        {
            //PATCH: clean up custom widgets added for power invocations
            DisablePowerUseSlots(__instance);
        }
    }
}
