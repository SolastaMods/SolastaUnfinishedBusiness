using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GuiCharacterActionPatcher
{
    [HarmonyPatch(typeof(GuiCharacterAction), nameof(GuiCharacterAction.LimitedUses), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LimitedUses_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiCharacterAction __instance, ref int __result)
        {
            //PATCH: Get remaining attack uses (ammunition) from forced attack mode
            if (__instance.forcedAttackMode == null)
            {
                return;
            }

            __result = __instance.ActingCharacter.RulesetCharacter.GetRemainingAttackUses(__instance.forcedAttackMode);
        }
    }

    [HarmonyPatch(typeof(GuiCharacterAction), nameof(GuiCharacterAction.SetupTooltip))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SetupTooltip_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GuiCharacterAction __instance,
            ActionDefinitions.ActionStatus actionStatus,
            GuiTooltip guiTooltip,
            RulesetAttackMode currentAttackMode,
            ref int currentEffectFormCount,
            ref string currentFailureString,
            ref string currentTooltip,
            ref ActionDefinitions.ActionStatus currentActionStatus)
        {
            //PATCH: Get custom error message for CastQuickened action
            CustomActionIdContext.CheckQuickenedStatus(__instance, actionStatus, guiTooltip, ref currentFailureString);
        }
    }
}
