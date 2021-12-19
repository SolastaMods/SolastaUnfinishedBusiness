using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUiLocation
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
                switch (__instance.SpellSelectionPanel.ActionType)
                {
                    case ActionDefinitions.ActionType.Main:
                        actionId = ActionDefinitions.Id.CastMain;
                        break;
                    case ActionDefinitions.ActionType.Bonus:
                        actionId = ActionDefinitions.Id.CastBonus;
                        break;
                    case ActionDefinitions.ActionType.Reaction:
                        actionId = ActionDefinitions.Id.CastReaction;
                        break;
                    case ActionDefinitions.ActionType.NoCost:
                        actionId = ActionDefinitions.Id.CastNoCost;
                        break;
                    default:
                        actionId = ActionDefinitions.Id.CastMain;
                        break;
                }
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
                switch (__instance.PowerSelectionPanel.ActionType)
                {
                    case ActionDefinitions.ActionType.Main:
                        actionId = ActionDefinitions.Id.PowerMain;
                        break;
                    case ActionDefinitions.ActionType.Bonus:
                        actionId = ActionDefinitions.Id.PowerBonus;
                        break;
                    case ActionDefinitions.ActionType.Reaction:
                        actionId = ActionDefinitions.Id.PowerReaction;
                        break;
                    case ActionDefinitions.ActionType.NoCost:
                        actionId = ActionDefinitions.Id.PowerNoCost;
                        break;
                    default:
                        actionId = ActionDefinitions.Id.PowerMain;
                        break;
                }
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
#pragma warning disable S3458 // Empty "case" clauses that fall through to the "default" should be omitted
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
                        case ActionDefinitions.Id.AttackOpportunity:
                        case ActionDefinitions.Id.BlockAttack:
                        case ActionDefinitions.Id.CastReaction:
                        case ActionDefinitions.Id.PowerReaction:
                        case ActionDefinitions.Id.ReactionShot:
                        case ActionDefinitions.Id.NoAction:
                        case ActionDefinitions.Id.DropProne:
                        case ActionDefinitions.Id.Jump:
                        case ActionDefinitions.Id.ExplorationMove:
                        case ActionDefinitions.Id.FreeFall:
                        case ActionDefinitions.Id.Levitate:
                        case ActionDefinitions.Id.SpendSpellSlot:
                        case ActionDefinitions.Id.SpendPower:
                        case ActionDefinitions.Id.StandUp:
                        case ActionDefinitions.Id.TacticalMove:
                        case ActionDefinitions.Id.UncannyDodge:
                        case ActionDefinitions.Id.StartBattle:
                        case ActionDefinitions.Id.Pushed:
                        case ActionDefinitions.Id.SleightOfHand:
                        case ActionDefinitions.Id.AttackReadied:
                        case ActionDefinitions.Id.CastReadied:
                        case ActionDefinitions.Id.Ready:
                        case ActionDefinitions.Id.CounterAttackWithPower:
                        case ActionDefinitions.Id.PowerNoCost:
                        case ActionDefinitions.Id.GiantKiller:
                        case ActionDefinitions.Id.CastRitual:
                        case ActionDefinitions.Id.AlwaysAvailable:
                        case ActionDefinitions.Id.TriggerDefeat:
                        case ActionDefinitions.Id.CastNoCost:
                        case ActionDefinitions.Id.Unhide:
                        case ActionDefinitions.Id.Charge:
                        case ActionDefinitions.Id.DeflectMissile:
                        case ActionDefinitions.Id.ActionSurge:
                        case ActionDefinitions.Id.StepBack:
                        case ActionDefinitions.Id.BreakFree:
                        case ActionDefinitions.Id.SpecialMove:
                        case ActionDefinitions.Id.TakeAim:
                        case ActionDefinitions.Id.RushToBattle:
                        case ActionDefinitions.Id.UseLegendaryResistance:
                        case ActionDefinitions.Id.BreakEnchantment:
                        case ActionDefinitions.Id.Dismissal:
                        case ActionDefinitions.Id.LeafScales:
                        case ActionDefinitions.Id.UseIndomitableResistance:
                        default:
                            panelToActivate = battlePanel.GetField<CharacterActionPanel>("otherActionPanel");
                            break;
                    }
#pragma warning restore S3458 // Empty "case" clauses that fall through to the "default" should be omitted
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
}
