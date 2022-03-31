using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsKeyboardTrap
{
    internal static class FlexibleCastingModalPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(FlexibleCastingModal), "OnConvertCb")]
        internal static class FlexibleCastingModalOnConvertCb
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                SharedSpellsContext.ForceLongRestSlot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
    }
}
