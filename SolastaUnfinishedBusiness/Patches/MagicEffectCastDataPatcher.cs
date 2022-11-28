using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class MagicEffectCastDataPatcher
{
    [HarmonyPatch(typeof(ActionDefinitions.MagicEffectCastData), "IsQuickSpell", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class IsQuickSpell_Patch
    {
        public static void Postfix(ActionDefinitions.MagicEffectCastData __instance, ref bool __result)
        {
            if (__instance.EffectDescription.SpeedParameter < 0)
            {
                __result = true;
            }
        }
    }
}
