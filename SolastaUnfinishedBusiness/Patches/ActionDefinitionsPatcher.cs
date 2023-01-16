using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActionDefinitionsPatcher
{
    [HarmonyPatch(typeof(ActionDefinitions), nameof(ActionDefinitions.CanActionHaveMultipleGuiItems))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanActionHaveMultipleGuiItems_Patch
    {
        [UsedImplicitly]
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
