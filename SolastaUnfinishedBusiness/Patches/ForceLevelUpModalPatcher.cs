using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ForceLevelUpModalPatcher
{
    //PATCH: allows the force level up UI work with parties greater than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(ForceLevelUpModal), nameof(ForceLevelUpModal.Init))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ForceLevelUpModal __instance)
        {
            var characterPlatesTable = __instance.characterPlatesTable;
            var buttonsTable = __instance.buttonsTable;
            var firstPlateChild = characterPlatesTable.GetChild(0);
            var firstButtonChild = buttonsTable.GetChild(0);

            while (characterPlatesTable.childCount < Main.Settings.OverridePartySize)
            {
                Object.Instantiate(firstPlateChild, firstPlateChild.parent);
            }

            while (buttonsTable.childCount < Main.Settings.OverridePartySize)
            {
                Object.Instantiate(firstButtonChild, firstButtonChild.parent);
            }

            while (__instance.Heroes.Count > Main.Settings.OverridePartySize)
            {
                __instance.Heroes.RemoveAt(__instance.Heroes.Count - 1);
            }

            // scales down the plates table if required
            var parentRectTransform = __instance.characterPlatesTable.parent.GetComponent<RectTransform>();

            switch (Main.Settings.OverridePartySize)
            {
                case 6:
                    parentRectTransform.anchoredPosition = new Vector2(-150, -78);
                    parentRectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                    break;
                case 5:
                    parentRectTransform.anchoredPosition = new Vector2(-147, -78);
                    parentRectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    break;
                default:
                    parentRectTransform.anchoredPosition = new Vector2(0, -20);
                    parentRectTransform.localScale = new Vector3(1, 1, 1);
                    break;
            }
        }
    }
}
