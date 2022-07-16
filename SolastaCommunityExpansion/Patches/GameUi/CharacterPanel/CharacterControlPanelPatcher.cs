using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterPanel;

[HarmonyPatch(typeof(CharacterControlPanel), "OnConfigurationSwitchedHandler")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class CharacterControlPanel_OnConfigurationSwitchedHandler
{
    private static CharacterActionPanel _panelToActivate;
    private static ActionDefinitions.Id _actionId;

    internal static void Prefix([NotNull] CharacterControlPanel __instance)
    {
        if (!Main.Settings.KeepSpellsOpenSwitchingEquipment)
        {
            return;
        }

        var foundActivePanel = false;

        if (__instance.Visible && __instance.SpellSelectionPanel != null && __instance.SpellSelectionPanel.Visible)
        {
            foundActivePanel = true;

            _actionId = __instance.SpellSelectionPanel.ActionType switch
            {
                ActionDefinitions.ActionType.Main => ActionDefinitions.Id.CastMain,
                ActionDefinitions.ActionType.Bonus => ActionDefinitions.Id.CastBonus,
                ActionDefinitions.ActionType.Reaction => ActionDefinitions.Id.CastReaction,
                ActionDefinitions.ActionType.NoCost => ActionDefinitions.Id.CastNoCost,
                _ => ActionDefinitions.Id.CastMain
            };
            __instance.SpellSelectionPanel.Hide(true);
        }

        if (__instance.Visible && __instance.RitualSelectionPanel != null &&
            __instance.RitualSelectionPanel.Visible)
        {
            foundActivePanel = true;
            _actionId = ActionDefinitions.Id.CastRitual;
            __instance.RitualSelectionPanel.Hide(true);
        }

        if (__instance.Visible && __instance.PowerSelectionPanel != null && __instance.PowerSelectionPanel.Visible)
        {
            foundActivePanel = true;

            _actionId = __instance.PowerSelectionPanel.ActionType switch
            {
                ActionDefinitions.ActionType.Main => ActionDefinitions.Id.PowerMain,
                ActionDefinitions.ActionType.Bonus => ActionDefinitions.Id.PowerBonus,
                ActionDefinitions.ActionType.Reaction => ActionDefinitions.Id.PowerReaction,
                ActionDefinitions.ActionType.NoCost => ActionDefinitions.Id.PowerNoCost,
                _ => ActionDefinitions.Id.PowerMain
            };
            __instance.PowerSelectionPanel.Hide(true);
        }

        if (foundActivePanel)
        {
            switch (__instance)
            {
                case CharacterControlPanelExploration exploration:
                    _panelToActivate = exploration.ExplorationActionPanel;
                    break;
                case CharacterControlPanelBattle battlePanel:
                    ActivateBattlePanel(battlePanel);
                    break;
            }
        }

        [SuppressMessage("Minor Code Smell", "IDE0066:Use switch expression", Justification = "Prefer switch here")]
        static void ActivateBattlePanel([NotNull] CharacterControlPanelBattle battlePanel)
        {
            switch (_actionId)
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
                    _panelToActivate = battlePanel.MainActionPanel;
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
                    _panelToActivate = battlePanel.bonusActionPanel;
                    break;
                case ActionDefinitions.Id.NoAction:
                case ActionDefinitions.Id.AttackOpportunity:
                case ActionDefinitions.Id.BlockAttack:
                case ActionDefinitions.Id.CastReaction:
                case ActionDefinitions.Id.Cautious:
                case ActionDefinitions.Id.Climb:
                case ActionDefinitions.Id.DropProne:
                case ActionDefinitions.Id.ExplorationMove:
                case ActionDefinitions.Id.Jump:
                case ActionDefinitions.Id.FreeFall:
                case ActionDefinitions.Id.Levitate:
                case ActionDefinitions.Id.PowerReaction:
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
                case ActionDefinitions.Id.ReactionShot:
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
                case ActionDefinitions.Id.SwiftRetaliation:
                case ActionDefinitions.Id.Volley:
                case ActionDefinitions.Id.WhirlwindAttack:
                case ActionDefinitions.Id.AttackFree:
                case ActionDefinitions.Id.FastAim:
                case ActionDefinitions.Id.Sunbeam:
                case ActionDefinitions.Id.EyebiteAsleep:
                case ActionDefinitions.Id.EyebitePanicked:
                case ActionDefinitions.Id.EyebiteSickened:
                case ActionDefinitions.Id.RageStart:
                case ActionDefinitions.Id.RageStop:
                case ActionDefinitions.Id.RecklessAttack:
                case ActionDefinitions.Id.RecallItem:
                case ActionDefinitions.Id.ReapplyEffect:
                case ActionDefinitions.Id.ProxyMoonBeam:
                case ActionDefinitions.Id.ProxyCallLightning:
                case ActionDefinitions.Id.ProxyCallLightningFree:
                case ActionDefinitions.Id.ProxySpiritualWeaponFree:
                case ActionDefinitions.Id.WildShape:
                case ActionDefinitions.Id.RevertShape:
                case ActionDefinitions.Id.SpiritRally:
                case ActionDefinitions.Id.SharedPain:
                case ActionDefinitions.Id.SpiritRallyTeleport:
                case ActionDefinitions.Id.CoordinatedDefense:
                case ActionDefinitions.Id.BorrowLuck:
                case ActionDefinitions.Id.ProxyVengefulSpirits:
                case ActionDefinitions.Id.ProxyDelayedBlastFireball:
                default:
                    _panelToActivate = battlePanel.otherActionPanel;
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
        if (_panelToActivate != null)
        {
            _panelToActivate.OnActivateAction(_actionId);
        }

        _panelToActivate = null;
    }
}

// [HarmonyPatch(typeof(CharacterControlPanel), "Bind")]
// [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
// internal static class CharacterControlPanel_Bind
// {
//     internal static void Prefix(GameLocationCharacter gameCharacter)
//     {
//         Global.ActivePlayerCharacter = gameCharacter;
//     }
// }
//
// [HarmonyPatch(typeof(CharacterControlPanel), "Unbind")]
// [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
// internal static class CharacterControlPanel_Unbind
// {
//     internal static void Prefix()
//     {
//         Global.ActivePlayerCharacter = null;
//     }
// }
