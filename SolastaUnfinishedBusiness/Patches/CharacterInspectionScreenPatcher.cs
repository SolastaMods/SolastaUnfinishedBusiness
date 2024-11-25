using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
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
        public static bool Prefix(CharacterInspectionScreen __instance)
        {
            if (__instance.inventoryManagementMode == ActionDefinitions.InventoryManagementMode.Battle)
            {
                __instance.screenCaption.gameObject.SetActive(true);
                if (!Main.Settings.EnableUnlimitedInventoryActions && __instance.InspectedCharacter.GameLocationCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.FreeOnce, ActionDefinitions.ActionScope.Battle, false) == ActionDefinitions.ActionStatus.Available)
                {
                    __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeBattleAvailableTitle", false, null, null);
                    __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                    return false; // Skip the original method
                }
                else if (Main.Settings.EnableUnlimitedInventoryActions && __instance.InspectedCharacter.GameLocationCharacter.GetActionTypeStatus(ActionDefinitions.ActionType.FreeOnce, ActionDefinitions.ActionScope.Battle, false) == ActionDefinitions.ActionStatus.Available)
                {
                    __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeBattleUnlimitedTitle", false, null, null);
                    __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                    return false; // Skip the original method
                }
                __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeBattleSpentTitle", false, null, null);
                __instance.screenCaption.TMP_Text.color = __instance.inventoryActionSpentColor;
                return false; // Skip the original method
            }
            else
            {
                if (__instance.inventoryManagementMode == ActionDefinitions.InventoryManagementMode.SelectItem)
                {
                    __instance.screenCaption.gameObject.SetActive(true);
                    switch (__instance.itemSelectionType)
                    {
                        case ActionDefinitions.ItemSelectionType.Equiped:
                        case ActionDefinitions.ItemSelectionType.EquippedNoLightSource:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectEquipedItemTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.Carried:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectCarriedItemTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.MagicalUnidentified:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectMagicalUnidentifiedItemTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.Weapon:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectWeaponTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.WeaponNonMagical:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectWeaponNonMagicalTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.WieldedClubOrQuarterstaff:
                            __instance.screenCaption.Text = Gui.Localize("Screen/&CharacterInspectionModeSelectWieldedClubOrQuarterstaffTitle", false, null, null);
                            break;
                        case ActionDefinitions.ItemSelectionType.Spellbook:
                            __instance.screenCaption.Text = Gui.Format("Screen/&CharacterInspectionModeSelectSpellbookTitle", new string[]
                            {
                                __instance.spellToScribe.SpellLevel.ToString()
                            });
                            break;
                    }
                    __instance.screenCaption.TMP_Text.color = __instance.inventoryActionAvailableColor;
                    return false; // Skip the original method
                }
                __instance.screenCaption.gameObject.SetActive(false);
                return false; // Skip the original method
            }
        }
    }
}
