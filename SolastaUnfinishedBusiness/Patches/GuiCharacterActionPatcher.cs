using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;

namespace SolastaUnfinishedBusiness.Patches;

public static class GuiCharacterActionPatcher
{
    [HarmonyPatch(typeof(GuiCharacterAction), "LimitedUses", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class LimitedUses_Getter_Patch
    {
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

    [HarmonyPatch(typeof(GuiCharacterAction), "OnGoing", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnGoing_Getter_Patch
    {
        public static bool Prefix(GuiCharacterAction __instance, ref bool __result)
        {
            if (__instance.actionId != (ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle &&
                __instance.actionId != (ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle)
            {
                return true;
            }

            __result = __instance.actingCharacter.RulesetCharacter.IsToggleEnabled(__instance.actionId);

            return false;
        }
    }
}
