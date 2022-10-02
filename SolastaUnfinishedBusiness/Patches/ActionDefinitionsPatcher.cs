using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class ActionDefinitionsPatcher
{
    [HarmonyPatch(typeof(ActionDefinitions), "CanActionHaveMultipleGuiItems")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CanActionHaveMultipleGuiItems_Patch
    {
        public static void Postfix(ref bool __result, ActionDefinitions.Id actionId)
        {
            //PATCH: (ExtraAttacksOnActionPanel) allow multiple offhand attacks on action panel
            if (actionId == ActionDefinitions.Id.AttackOff)
            {
                __result = true;
            }
        }
    }
}
