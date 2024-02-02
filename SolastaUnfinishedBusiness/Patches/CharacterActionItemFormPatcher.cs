using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Subclasses.Builders;
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
        [UsedImplicitly]
        public static void Postfix(CharacterActionItemForm __instance)
        {
            //PATCH: support for `IActionItemDiceBox` showing custom dice number/size
            var action = __instance.guiCharacterAction.ActionDefinition;
            var provider = action.GetFirstSubFeatureOfType<GambitsBuilders.IActionItemDiceBox>();

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

            if (box == null)
            {
                return;
            }

            var dieImage = __instance.bardicInspirationDieImage;
            var dieLabel = __instance.bardicInspirationNumberLabel;
            var tooltip = __instance.bardicInspirationTooltip;

            box.gameObject.SetActive(true);

            if (dieImage.sprite != null)
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
