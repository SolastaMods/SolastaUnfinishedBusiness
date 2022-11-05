using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
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
    [HarmonyPatch(new[] { typeof(ActionDefinitions.Id), typeof(GuiCharacterAction) })]
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
}
