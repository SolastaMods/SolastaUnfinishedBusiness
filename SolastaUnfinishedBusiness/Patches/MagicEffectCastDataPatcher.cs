using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MagicEffectCastDataPatcher
{
    [HarmonyPatch(typeof(ActionDefinitions.MagicEffectCastData),
        nameof(ActionDefinitions.MagicEffectCastData.IsQuickSpell), MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsQuickSpell_Getter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ActionDefinitions.MagicEffectCastData __instance, ref bool __result)
        {
            if (__instance.EffectDescription.SpeedParameter < 0)
            {
                __result = true;
            }
        }
    }
}
