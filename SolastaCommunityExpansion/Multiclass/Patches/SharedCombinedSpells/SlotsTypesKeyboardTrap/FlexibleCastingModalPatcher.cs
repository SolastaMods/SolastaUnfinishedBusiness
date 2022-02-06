using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsTypesKeyboardTrap
{
    internal static class FlexibleCastingModalPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(FlexibleCastingModal), "OnConvertCb")]
        internal static class ReactionModalOnReact
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.SharedSpellsContext.ForceLongRestSlot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
    }
}
