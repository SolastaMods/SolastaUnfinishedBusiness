using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using TA.AddressableAssets;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionItemFormPatcher
{
    [HarmonyPatch(typeof(CharacterActionItemForm), nameof(CharacterActionItemForm.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: Get dynamic properties from forced attack
            if (__instance.guiCharacterAction.forcedAttackMode == null)
            {
                return;
            }

            __instance.dynamicItemPropertiesEnumerator.Unbind();
            __instance.dynamicItemPropertiesEnumerator.Bind(
                __instance.guiCharacterAction.forcedAttackMode.sourceObject as RulesetItem);
        }
    }

    [HarmonyPatch(typeof(CharacterActionItemForm), nameof(CharacterActionItemForm.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        private static void SetupUseSlots(
            GuiCharacterAction __instance,
            RectTransform useSlotsTable,
            GuiLabel highSlotNumber)
        {
            var activatedPower = SpellPointsContext.PowerSpellPoints;
            var rulesetCharacter = __instance.actingCharacter.RulesetCharacter;
            var usablePower = rulesetCharacter.UsablePowers.FirstOrDefault(x => x.PowerDefinition == activatedPower);
            var remainingUsesOfPower = rulesetCharacter.GetRemainingUsesOfPower(usablePower);

            highSlotNumber.gameObject.SetActive(true);
            useSlotsTable.gameObject.SetActive(false);
            highSlotNumber.Text = remainingUsesOfPower.ToString();
            highSlotNumber.GuiTooltip.Content = "Screen/&SpellAlternatePointsTooltip";
        }

        [UsedImplicitly]
        public static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: support display remaining spell points usage (SPELL_POINTS)
            if (Main.Settings.UseAlternateSpellPointsSystem &&
                (__instance.GuiCharacterAction.ActionDefinition == DatabaseHelper.ActionDefinitions.CastMain ||
                 __instance.GuiCharacterAction.ActionDefinition == DatabaseHelper.ActionDefinitions.CastBonus))
            {
                SetupUseSlots(__instance.GuiCharacterAction, __instance.useSlotsTable, __instance.highSlotNumber);

                return;
            }

            //PATCH: support for `IActionItemDiceBox` showing custom dice number/size
            var action = __instance.guiCharacterAction.ActionDefinition;
            var provider = action.GetFirstSubFeatureOfType<IActionItemDiceBox>();

            if (provider == null)
            {
                return;
            }

            var rulesetCharacter = __instance.GuiCharacterAction.ActingCharacter.RulesetCharacter;

            if (rulesetCharacter == null)
            {
                return;
            }

            var (size, number, format) = provider.GetDiceInfo(rulesetCharacter);

            var box = __instance.bardicInpirationBox;

            if (!box)
            {
                return;
            }

            var dieImage = __instance.bardicInspirationDieImage;
            var dieLabel = __instance.bardicInspirationNumberLabel;
            var tooltip = __instance.bardicInspirationTooltip;

            box.gameObject.SetActive(true);

            if (dieImage.sprite)
            {
                Gui.ReleaseAddressableAsset(dieImage.sprite);
                dieImage.sprite = null;
            }

            var dieSizeAssetPath = $"Gui/Bitmaps/Dice/{size}Icon";

            if (SyncAddressables.AddressableResourceExists<Sprite>(dieSizeAssetPath))
            {
                dieImage.sprite = Gui.LoadAssetSync<Sprite>(dieSizeAssetPath);
            }

            dieLabel.Text = $"{number}x";
            tooltip.Content = Gui.Format(format, size.ToString(), number.ToString());
        }
    }
}
