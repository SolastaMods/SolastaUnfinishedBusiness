using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class ActionDefinitionsPatcher
{
    [HarmonyPatch(typeof(ActionDefinitions), "CanActionHaveMultipleGuiItems")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CanActionHaveMultipleGuiItems_Patch
    {
        internal static void Postfix(ref bool __result, ActionDefinitions.Id actionId)
        {
            //PATCH: (ExtraAttacksOnActionPanel) allow multiple offhand attacks on action panel
            if (actionId == ActionDefinitions.Id.AttackOff)
            {
                __result = true;
            }
        }
    }
}