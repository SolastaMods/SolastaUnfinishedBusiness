// keep this around if we need to support Warlock again
#if false
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsColors
{
    internal static class UsablePowerBoxPatcher
    {
        // guarantees power box slots are always white
        [HarmonyPatch(typeof(UsablePowerBox), "RefreshSlotUses")]
        internal static class UsablePowerBoxRefreshSlotUses
        {
            internal static void Postfix(UsablePowerBox __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var slotStatusTable = __instance.GetField<UsablePowerBox, RectTransform>("slotStatusTable");

                for (var i = 0; i < slotStatusTable.childCount; i++)
                {
                    var child = slotStatusTable.GetChild(i);
                    var component = child.GetComponent<SlotStatus>();

                    component.Available.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
#endif
