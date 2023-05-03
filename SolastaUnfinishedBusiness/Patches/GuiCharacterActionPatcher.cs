using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

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

    [HarmonyPatch(typeof(GuiCharacterAction), nameof(GuiCharacterAction.OnGoing), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnGoing_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GuiCharacterAction __instance, ref bool __result)
        {
            if (__instance.actionId != (ActionDefinitions.Id)ExtraActionId.FeatCrusherToggle &&
                __instance.actionId != (ActionDefinitions.Id)ExtraActionId.MonkKiPointsToggle &&
                __instance.actionId != (ActionDefinitions.Id)ExtraActionId.PaladinSmiteToggle &&
                __instance.actionId != (ActionDefinitions.Id)ExtraActionId.QuiveringPalmToggle)
            {
                return true;
            }

            __result = __instance.actingCharacter.RulesetCharacter.IsToggleEnabled(__instance.actionId);

            return false;
        }
    }
}
