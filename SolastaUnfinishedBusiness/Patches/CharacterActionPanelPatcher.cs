using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.ReadyActionEngaged))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReadyActionEngaged_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            //PATCH: used for `force preferred cantrip` option
            CustomReactionsContext.SaveReadyActionPreferredCantrip(__instance.actionParams, readyActionType);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.ComputeMultipleGuiCharacterActions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeMultipleGuiCharacterActions_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterActionPanel __instance, ref int __result, ActionDefinitions.Id actionId)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //Allows multiple actions on panel for off-hand attacks and main attacks for non-guests
            __result = ExtraAttacksOnActionPanel.ComputeMultipleGuiCharacterActions(__instance, actionId, __result);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.RefreshActions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActions_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: hide power button on action panel if no valid powers to use or see
            var method = new Func<
                List<ActionDefinitions.Id>,
                CharacterActionPanel,
                int
            >(FilterActions).Method;

            //TODO: it replaces first call to get_Count, find a better way to find proper place
            return instructions.ReplaceCode(
                code => code.opcode == OpCodes.Callvirt && $"{code.operand}".Contains("get_Count"), 1,
                "CharacterActionPanel.RefreshActions",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }

        private static int FilterActions(List<ActionDefinitions.Id> actions, CharacterActionPanel panel)
        {
            var character = panel.GuiCharacter.RulesetCharacter;
            var battle = Gui.Battle != null;

            //PATCH: reorder the actions panel in case we have custom toggles
            void DoReorder(ActionDefinitions.Id actionId)
            {
                var powerNdx = actions.FindIndex(x => x == ActionDefinitions.Id.PowerMain);

                if (powerNdx < 0)
                {
                    return;
                }

                actions.Remove(actionId);
                actions.Insert(powerNdx, actionId);
            }

            if (actions.Contains((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle))
            {
                DoReorder((ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle);
            }

            if (actions.Contains((ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle))
            {
                DoReorder((ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle);
            }

            if (actions.Contains((ActionDefinitions.Id)ExtraActionId.WildshapeSwapAttackToggle))
            {
                DoReorder((ActionDefinitions.Id)ExtraActionId.WildshapeSwapAttackToggle);
            }

            //PATCH: hide power button on action panel if no valid powers to use or see
            actions.RemoveAll(id => ActionIsInvalid(id, character, battle));
            return actions.Count;
        }

        private static bool ActionIsInvalid(ActionDefinitions.Id id, RulesetCharacter character, bool battle)
        {
            return id switch
            {
                ActionDefinitions.Id.PowerMain => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.Main, battle),
                ActionDefinitions.Id.PowerBonus => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.Bonus, battle),
                ActionDefinitions.Id.PowerNoCost => !character.CanSeeAndUseAtLeastOnePower(
                    ActionDefinitions.ActionType.NoCost, battle),
                (ActionDefinitions.Id)ExtraActionId.WildshapeSwapAttackToggle => GameLocationCharacter
                    .GetFromActor(character).HasAttackedSinceLastTurn,
                _ => false
            };
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.OnActivateAction))]
    [HarmonyPatch(new[] { typeof(ActionDefinitions.Id), typeof(GuiCharacterAction) })]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivateAction_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            var findAttacks = typeof(GameLocationCharacter).GetMethod("FindActionAttackMode");
            var method = new Func<
                GameLocationCharacter,
                ActionDefinitions.Id,
                bool,
                bool,
                GuiCharacterAction,
                RulesetAttackMode
            >(ExtraAttacksOnActionPanel.FindExtraActionAttackModesFromGuiAction).Method;

            return instructions.ReplaceCalls(findAttacks, "CharacterActionPanel.OnActivateAction",
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.InvocationCastEngaged))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class InvocationCastEngaged_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionPanel __instance, RulesetInvocation invocation, int subspellIndex)
        {
            var definition = invocation.InvocationDefinition;
            var power = definition.GetPower();
            var actionDefinitions =
                ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions;

            if (power != null)
            {
                var bundle = power.GetBundle();

                if (bundle != null)
                {
                    if (subspellIndex >= 0 && bundle.SubPowers.Count > subspellIndex)
                    {
                        power = bundle.SubPowers[subspellIndex];
                    }
                    else
                    {
                        Main.Error($"Wrong index for power bundle in '{definition.Name}' invocation: {subspellIndex}");
                    }
                }

                __instance.actionId = power.BattleActionId;
                __instance.actionParams.actionDefinition = actionDefinitions[__instance.actionId];
                __instance.PowerEngaged(UsablePowersProvider.Get(power, __instance.GuiCharacter.RulesetCharacter));

                return false;
            }

            if (definition.GrantedSpell != null)
            {
                if (__instance.actionId == ActionDefinitions.Id.CastInvocation)
                {
                    return true;
                }

                __instance.actionId = definition.GrantedSpell.BattleActionId;
                __instance.actionParams.actionDefinition = actionDefinitions[__instance.actionId];

                return true;
            }

            //Shouldn't happen - it should return from earlier, but just in case, to prevent crash
            Main.Error("InvocationCastEngaged with null spell and no power feature");
            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SelectInvocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectInvocation_Patch
    {
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for bonus action invocations
            //replaces calls to InvocationSelectionPanel.Bind to custom method which supports filtering bonus and main action invocations
            var bindInvocationSelectionPanel = typeof(InvocationSelectionPanel).GetMethod("Bind");
            var method = new Action<
                InvocationSelectionPanel,
                GameLocationCharacter, // caster,
                InvocationSelectionPanel.InvocationSelectedHandler, // selected,
                InvocationSelectionPanel.InvocationCancelledHandler, // canceled,
                CharacterActionPanel // panel
            >(InvocationSelectionPanelExtensions.CustomBind).Method;

            return instructions.ReplaceCalls(bindInvocationSelectionPanel, "CharacterActionPanel.SelectInvocation",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), nameof(CharacterActionPanel.SpellcastEngaged))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SpellcastEngaged_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            CharacterActionPanel __instance,
            // RulesetSpellRepertoire spellRepertoire,
            ref SpellDefinition spellDefinition,
            int slotLevel)
        {
            var rulesetCharacter = __instance.GuiCharacter.RulesetCharacter;
            var spellLevel = spellDefinition.SpellLevel;
            var upcastDelta = slotLevel - spellLevel;
            var spell = spellDefinition; // cannot pass ref to enumerator
            var requiresConcentration = !rulesetCharacter
                .GetSubFeaturesByType<IBypassSpellConcentration>()
                .Where(x => upcastDelta >= x.OnlyWithUpcastGreaterThan())
                .Any(y => y.SpellDefinitions().Contains(spell));

            if (!requiresConcentration)
            {
                spellDefinition =
                    DatabaseHelper.GetDefinition<SpellDefinition>($"{spellDefinition.Name}NoConcentration");
            }
        }
    }
}
