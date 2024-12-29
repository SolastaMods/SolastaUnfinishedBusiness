using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using TA.AddressableAssets;
using TMPro;
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
            //PATCH: make caption on small form wrap, instead of truncating
            //TODO: do we need a setting to control this?
            TMP_Text tmpText;
            if (__instance.highSlotNumber == null && (tmpText = __instance.captionLabel.tmpText) != null)
            {
                tmpText.enableWordWrapping = true;
                tmpText.alignment = TextAlignmentOptions.Bottom;
                tmpText.overflowMode = TextOverflowModes.Overflow;
            }

            //PATCH: disable word wrapping for attack number
            //useful when you have Spell Points enabled and have 100+ of them, not noticeable otherwise
            var attacks = __instance.attacksNumberValue;
            if (attacks != null && (tmpText = attacks.tmpText) != null)
            {
                tmpText.enableWordWrapping = false;
            }

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
        internal const string HideAttacksNumberOnActionPanel = "HideAttacksNumberOnActionPanel";

        [UsedImplicitly]
        public static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: supports hiding attack numbers on action panel
            if (__instance.currentAttackMode.AttackTags.Contains(HideAttacksNumberOnActionPanel))
            {
                __instance.attacksNumberGroup.gameObject.SetActive(false);
            }

            //PATCH: support display remaining spell points on cast actions (SPELL_POINTS)
            SpellPointsContext.DisplayRemainingSpellPointsOnCastActions(__instance.GuiCharacterAction,
                __instance.useSlotsTable, __instance.highSlotNumber, __instance.attacksNumberValue);

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
