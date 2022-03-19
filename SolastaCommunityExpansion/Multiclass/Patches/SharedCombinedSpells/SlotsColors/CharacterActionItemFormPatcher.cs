// keep this around if we need to support Warlock again
#if !WARLOCK_PACT_MAGIC
using HarmonyLib;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Multiclass.Patches.SharedCombinedSpells.SlotsColors
{
    internal static class CharacterActionItemFormPatcher
    {
        // guarantees rage and wildshape slots are always white
        [HarmonyPatch(typeof(CharacterActionItemForm), "Refresh")]
        internal static class CharacterActionItemFormRefresh
        {
            internal static void Postfix(CharacterActionItemForm __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                var useSlotsTable = __instance.GetField<CharacterActionItemForm, RectTransform>("useSlotsTable");

                if (useSlotsTable == null)
                {
                    return;
                }

                for (var i = 0; i < useSlotsTable.childCount; i++)
                {
                    var child = useSlotsTable.GetChild(i);
                    var component = child.GetComponent<SlotStatus>();

                    component.Available.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }
}
#endif
