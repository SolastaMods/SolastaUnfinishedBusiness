using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FlexibleCastingModalPatcher
{
    //PATCH: register on acting character if SHIFT is pressed on slots convertions
    [HarmonyPatch(typeof(FlexibleCastingModal), nameof(FlexibleCastingModal.OnConvertCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnConvertCb_Patch
    {
        [UsedImplicitly]
        public static void Postfix(FlexibleCastingModal __instance)
        {
            if (__instance.selectedSlotLevel < 0 || __instance.createSlotMode)
            {
                return;
            }

            var rulesetCaster = __instance.caster;
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);

            caster?.RegisterShiftState();
        }
    }
}
