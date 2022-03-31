using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsKeyboardTrap
{
    internal static class SpellActivationBoxPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(SpellActivationBox), "OnActivateCb")]
        internal static class SpellActivationBoxOnActivateCb
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
