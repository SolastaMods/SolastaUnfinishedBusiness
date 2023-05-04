using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

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
}
