using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterInspectionScreenPatcher
{
    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            //PATCH: enable custom models renderer
            CustomModels.SwitchRenderer(true);

            //PATCH: sets the inspection context for MC heroes
            Global.InspectedHero = heroCharacter;

            //PATCH: gets more real state for the toggles on top (MULTICLASS)
            var transform = __instance.toggleGroup.transform;

            transform.position =
                new Vector3(__instance.characterPlate.transform.position.x / 2f, transform.position.y, 0);
        }

        [UsedImplicitly]
        public static void Postfix(CharacterInspectionScreen __instance, RulesetCharacterHero heroCharacter)
        {
            //PATCH: support display max spell points on inspection screen (SPELL_POINTS)
            SpellPointsContext.DisplayMaxSpellPointsOnInspectionScreen(__instance, heroCharacter);

            //PATCH: hide repertoires that have hidden spell casting feature
            for (var index = 3; index < __instance.toggleGroup.transform.childCount; ++index)
            {
                var child = __instance.toggleGroup.transform.GetChild(index);

                if (index <= 3)
                {
                    if (Gui.Game)
                    {
                        continue;
                    }
                }

                var repertoire = heroCharacter.SpellRepertoires[index - __instance.staticTogglesNumber];

                if (repertoire.SpellCastingFeature.GuiPresentation.Hidden)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.Unbind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Unbind_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            //PATCH: disable custom models renderer
            CustomModels.SwitchRenderer(false);

            //PATCH: resets the inspection context for MC heroes
            Global.InspectedHero = null;
        }
    }

    //PATCH: resets the inspection context for MC heroes otherwise we get class name bleeding on char pool
    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.DoClose))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class DoClose_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            Global.InspectedHero = null;
        }
    }

    //PATCH: modify caption if unlimited inventory actions is enabled
    [HarmonyPatch(typeof(CharacterInspectionScreen), nameof(CharacterInspectionScreen.RefreshCaption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshCaption_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterInspectionScreen __instance)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (__instance.inventoryManagementMode)
            {
                case ActionDefinitions.InventoryManagementMode.Battle:
                    __instance.screenCaption.gameObject.SetActive(true);
                    switch (Main.Settings.EnableUnlimitedInventoryActions)
                    {
                        case false when
                            __instance.InspectedCharacter.GameLocationCharacter.GetActionTypeStatus(ActionDefinitions
                                .ActionType
                                .FreeOnce) == ActionDefinitions.ActionStatus.Available:
                            __instance.screenCaption.Text =
                                Gui.Localize("Screen/&CharacterInspectionModeBattleAvailableTitle");
                            __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                            return false; // Skip the original method
                        case true when
                            __instance.InspectedCharacter.GameLocationCharacter.GetActionTypeStatus(ActionDefinitions
                                .ActionType
                                .FreeOnce) == ActionDefinitions.ActionStatus.Available:
                            __instance.screenCaption.Text =
                                Gui.Localize("Screen/&CharacterInspectionModeBattleUnlimitedTitle");
                            __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                            return false; // Skip the original method
                    }

                    __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeBattleSpentTitle");
                    __instance.screenCaption.TMP_Text.color = __instance.inventoryActionSpentColor;
                    return false; // Skip the original method
                case ActionDefinitions.InventoryManagementMode.SelectItem:
                    __instance.screenCaption.gameObject.SetActive(true);
                    __instance.screenCaption.Text = __instance.itemSelectionType switch
                    {
                        ActionDefinitions.ItemSelectionType.Equiped
                            or ActionDefinitions.ItemSelectionType.EquippedNoLightSource => Gui.Localize(
                                "Screen/&CharacterInspectionModeSelectEquipedItemTitle"),
                        ActionDefinitions.ItemSelectionType.Carried => Gui.Localize(
                            "Screen/&CharacterInspectionModeSelectCarriedItemTitle"),
                        ActionDefinitions.ItemSelectionType.MagicalUnidentified => Gui.Localize(
                            "Screen/&CharacterInspectionModeSelectMagicalUnidentifiedItemTitle"),
                        ActionDefinitions.ItemSelectionType.Weapon => Gui.Localize(
                            "Screen/&CharacterInspectionModeSelectWeaponTitle"),
                        ActionDefinitions.ItemSelectionType.WeaponNonMagical => Gui.Localize(
                            "Screen/&CharacterInspectionModeSelectWeaponNonMagicalTitle"),
                        ActionDefinitions.ItemSelectionType.WieldedClubOrQuarterstaff => Gui.Localize(
                            "Screen/&CharacterInspectionModeSelectWieldedClubOrQuarterstaffTitle"),
                        ActionDefinitions.ItemSelectionType.Spellbook => Gui.Format(
                            "Screen/&CharacterInspectionModeSelectSpellbookTitle",
                            __instance.spellToScribe.SpellLevel.ToString()),
                        _ => __instance.screenCaption.Text
                    };

                    __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                    return false; // Skip the original method
                default:
                    __instance.screenCaption.gameObject.SetActive(false);
                    return false; // Skip the original method
            }
        }
    }
}
