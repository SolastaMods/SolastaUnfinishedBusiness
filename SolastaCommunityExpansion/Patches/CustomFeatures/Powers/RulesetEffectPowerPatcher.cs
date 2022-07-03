using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Powers;

[HarmonyPatch(typeof(RulesetEffectPower), "SaveDC", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_UsePower_Getter
{
    public static void Postfix(RulesetEffectPower __instance, ref int __result)
    {
        var originItem = __instance.OriginItem;

        if (originItem == null || originItem.UsableDeviceDescription.SaveDC != -1)
        {
            return;
        }

        var usablePower = __instance.UsablePower;

        UsablePowersProvider.UpdateSaveDC(__instance.User, usablePower);
        __result = usablePower.SaveDC;
    }
}
