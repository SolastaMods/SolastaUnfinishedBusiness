using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CharacterActionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterActionPanel), "ReadyActionEngaged")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReadyActionEngaged_Patch
    {
        internal static void Prefix(CharacterActionPanel __instance, ActionDefinitions.ReadyActionType readyActionType)
        {
            //PATCH: used for `force preferred cantrip` option
            CustomReactionsContext.SaveReadyActionPreferredCantrip(__instance.actionParams, readyActionType);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "ComputeMultipleGuiCharacterActions")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ComputeMultipleGuiCharacterActions_Patch
    {
        internal static void Postfix(CharacterActionPanel __instance, ref int __result, ActionDefinitions.Id actionId)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //Allows multiple actions on panel for off-hand attacks and main attacks for non-guests
            __result = ExtraAttacksOnActionPanel.ComputeMultipleGuiCharacterActions(__instance, actionId, __result);
        }
    }

    [HarmonyPatch(typeof(CharacterActionPanel), "OnActivateAction")]
    [HarmonyPatch(new[] {typeof(ActionDefinitions.Id), typeof(GuiCharacterAction)})]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnActivateAction_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: Support for ExtraAttacksOnActionPanel
            //replaces calls to FindExtraActionAttackModes to custom method which supports forced attack modes for offhand attacks
            return ExtraAttacksOnActionPanel.ReplaceFindExtraActionAttackModes(instructions);
        }
    }
}