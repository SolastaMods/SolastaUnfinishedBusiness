// keep this around if we need to support Warlock again
#if false
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsTypesKeyboardTrap
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

                Models.SharedSpellsContext.ForceLongRestSlot = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
        }
    }
}
#endif
