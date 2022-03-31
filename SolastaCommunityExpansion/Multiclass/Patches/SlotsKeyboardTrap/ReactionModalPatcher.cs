using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaMulticlass.Models;
using UnityEngine;

namespace SolastaMulticlass.Patches.SlotsKeyboardTrap
{
    internal static class ReactionModalPatcher
    {
        // traps if SHIFT is pressed to determine which slot type to consume
        [HarmonyPatch(typeof(ReactionModal), "OnReact")]
        internal static class ReactionModalOnReact
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
