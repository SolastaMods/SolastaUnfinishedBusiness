using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel
{
    [HarmonyPatch(typeof(CharacterControlPanel), "OnConfigurationSwitchedHandler")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterControlPanel_OnConfigurationSwitchedHandler
    {
        private static CharacterActionPanel panelToActivate;
        private static ActionDefinitions.Id actionId;

        internal static void Prefix(CharacterControlPanel __instance)
        {
            if (!Main.Settings.KeepSpellsOpenSwitchingEquipment)
            {
                return;
            }

            bool foundActivePanel = false;

            if (__instance.Visible && __instance.SpellSelectionPanel != null && __instance.SpellSelectionPanel.Visible)
            {
                foundActivePanel = true;

                actionId = __instance.SpellSelectionPanel.ActionType switch
                {
                    ActionDefinitions.ActionType.Main => ActionDefinitions.Id.CastMain,
                    ActionDefinitions.ActionType.Bonus => ActionDefinitions.Id.CastBonus,
                    ActionDefinitions.ActionType.Reaction => ActionDefinitions.Id.CastReaction,
                    ActionDefinitions.ActionType.NoCost => ActionDefinitions.Id.CastNoCost,
                    _ => ActionDefinitions.Id.CastMain,
                };
                __instance.SpellSelectionPanel.Hide(true);
            }

            if (__instance.Visible && __instance.RitualSelectionPanel != null && __instance.RitualSelectionPanel.Visible)
            {
                foundActivePanel = true;
                actionId = ActionDefinitions.Id.CastRitual;
                __instance.RitualSelectionPanel.Hide(true);
            }

            if (__instance.Visible && __instance.PowerSelectionPanel != null && __instance.PowerSelectionPanel.Visible)
            {
                foundActivePanel = true;

                actionId = __instance.PowerSelectionPanel.ActionType switch
                {
                    ActionDefinitions.ActionType.Main => ActionDefinitions.Id.PowerMain,
                    ActionDefinitions.ActionType.Bonus => ActionDefinitions.Id.PowerBonus,
                    ActionDefinitions.ActionType.Reaction => ActionDefinitions.Id.PowerReaction,
                    ActionDefinitions.ActionType.NoCost => ActionDefinitions.Id.PowerNoCost,
                    _ => ActionDefinitions.Id.PowerMain,
                };
                __instance.PowerSelectionPanel.Hide(true);
            }

            if (foundActivePanel)
            {
                if (__instance is CharacterControlPanelExploration exploration)
                {
                    panelToActivate = exploration.ExplorationActionPanel;
                }
                else if (__instance is CharacterControlPanelBattle battlePanel)
                {
                    ActivateBattlePanel(battlePanel);
                }
            }

            [SuppressMessage("Minor Code Smell", "IDE0066:Use switch expression", Justification = "Prefer switch here")]
            static void ActivateBattlePanel(CharacterControlPanelBattle battlePanel)
            {
                switch (actionId)
                {
                    case ActionDefinitions.Id.CastMain:
                    case ActionDefinitions.Id.AttackMain:
                    case ActionDefinitions.Id.DashMain:
                    case ActionDefinitions.Id.DisengageMain:
                    case ActionDefinitions.Id.Dodge:
                    case ActionDefinitions.Id.HideMain:
                    case ActionDefinitions.Id.Manipulate:
                    case ActionDefinitions.Id.LootGround:
                    case ActionDefinitions.Id.PowerMain:
                    case ActionDefinitions.Id.Shove:
                    case ActionDefinitions.Id.UseItemMain:
                    case ActionDefinitions.Id.AssignTargetMain:
                    case ActionDefinitions.Id.Extinguish:
                    case ActionDefinitions.Id.Awaken:
                    case ActionDefinitions.Id.VampiricTouch:
                    case ActionDefinitions.Id.Stabilize:
                        panelToActivate = battlePanel.MainActionPanel;
                        break;
                    case ActionDefinitions.Id.CastBonus:
                    case ActionDefinitions.Id.AttackOff:
                    case ActionDefinitions.Id.CunningAction:
                    case ActionDefinitions.Id.DashBonus:
                    case ActionDefinitions.Id.DisengageBonus:
                    case ActionDefinitions.Id.HideBonus:
                    case ActionDefinitions.Id.PowerBonus:
                    case ActionDefinitions.Id.UseItemBonus:
                    case ActionDefinitions.Id.ShoveBonus:
                    case ActionDefinitions.Id.AssignTargetBonus:
                    case ActionDefinitions.Id.CunningActionFastHands:
                    case ActionDefinitions.Id.ProxySpiritualWeapon:
                    case ActionDefinitions.Id.ProxyFlamingSphere:
                    case ActionDefinitions.Id.ProxyDancingLights:
                        panelToActivate = battlePanel.GetField<CharacterActionPanel>("bonusActionPanel");
                        break;
                    default:
                        panelToActivate = battlePanel.GetField<CharacterActionPanel>("otherActionPanel");
                        break;
                }
            }
        }

        internal static void Postfix()
        {
            if (!Main.Settings.KeepSpellsOpenSwitchingEquipment)
            {
                return;
            }

            // Re transition to current state?
            if (panelToActivate != null)
            {
                panelToActivate.OnActivateAction(actionId);
            }

            panelToActivate = null;
        }
    }

    [HarmonyPatch(typeof(CharacterControlPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterControlPanel_Bind
    {
        internal static void Prefix(CharacterControlPanel __instance, GameLocationCharacter gameCharacter, RectTransform actionTooltipAnchor)
        {
            ActivePlayerCharacter.Set(gameCharacter);
        }
    }
    
    [HarmonyPatch(typeof(CharacterControlPanel), "Unbind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterControlPanel_Unbind
    {
        internal static void Prefix(CharacterControlPanel __instance)
        {
            ActivePlayerCharacter.Set();
        }
    }
}
