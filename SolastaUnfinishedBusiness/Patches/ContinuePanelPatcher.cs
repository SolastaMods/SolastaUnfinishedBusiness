using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ContinuePanelPatcher
{
    //PATCH: tweaks the UI to allow less/more heroes to be selected on a campaign (PARTYSIZE)
    [HarmonyPatch(typeof(ContinuePanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] ContinuePanel __instance)
        {
            // scales down the plates table if required
            var parentRectTransform = __instance.charactersTable.GetComponent<RectTransform>();

            switch (Main.Settings.OverridePartySize)
            {
                case 6:
                    parentRectTransform.anchoredPosition = new Vector2(-115f, 390f);
                    parentRectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    break;
                case 5:
                    parentRectTransform.anchoredPosition = new Vector2(-65f, 390f);
                    parentRectTransform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                    break;
                default:
                    parentRectTransform.anchoredPosition = new Vector2(0, 430f);
                    parentRectTransform.localScale = new Vector3(1, 1, 1);
                    break;
            }
        }
    }
}
