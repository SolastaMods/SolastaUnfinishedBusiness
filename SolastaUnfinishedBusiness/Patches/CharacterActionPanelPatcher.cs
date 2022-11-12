using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterActionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterActionPanel), "ReadyActionEngaged")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ReadyActionEngaged_Patch
    {
        public static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            //PATCH: used for `force preferred cantrip` option
            CustomReactionsContext.SaveReadyActionPreferredCantrip(__instance.actionParams, readyActionType);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "ComputeMultipleGuiCharacterActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ComputeMultipleGuiCharacterActions_Patch
    {
        public static void Postfix(CharacterActionPanel __instance, ref int __result, ActionDefinitions.Id actionId)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //Allows multiple actions on panel for off-hand attacks and main attacks for non-guests
            __result = ExtraAttacksOnActionPanel.ComputeMultipleGuiCharacterActions(__instance, actionId, __result);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "OnActivateAction")]
    [HarmonyPatch(new[] {typeof(ActionDefinitions.Id), typeof(GuiCharacterAction)})]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnActivateAction_Patch
    {
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

    [HarmonyPatch(typeof(CharacterActionPanel), "InvocationCastEngaged")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class InvocationCastEngaged_Patch
    {
        public static bool Prefix(CharacterActionPanel __instance, RulesetInvocation invocation, int subspellIndex)
        {
            var definition = invocation.InvocationDefinition;
            var power = definition.GetPower();

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

                var actionDefinitions =
                    ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions;

                __instance.actionId = power.BattleActionId;
                __instance.actionParams.actionDefinition = actionDefinitions[__instance.actionId];
                __instance.PowerEngaged(UsablePowersProvider.Get(power, __instance.GuiCharacter.RulesetCharacter));

                return false;
            }

            if (definition.GrantedSpell != null)
            {
                return true;
            }

            //Shouldn't happen - it should return from earlier, but just in case, to prevent crash
            Main.Error("InvocationCastEngaged with null spell and no power feature");
            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "SelectInvocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SelectInvocation_Patch
    {
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
}
