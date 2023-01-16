using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class PartyControlPanelPatcher
{
    [HarmonyPatch(typeof(PartyControlPanel), nameof(PartyControlPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] PartyControlPanel __instance)
        {
            //PATCH: scales down the party control panel whenever the party size is bigger than 4 (PARTYSIZE)
            var partyCount = Gui.GameCampaign.Party.CharactersList.Count;
            var parentRectTransform = __instance.partyPlatesTable.parent.GetComponent<RectTransform>();

            parentRectTransform.localScale = partyCount > ToolsContext.GamePartySize
                ? new Vector3(0.8f, 0.8f, 0.8f)
                : new Vector3(1, 1, 1);

            if (partyCount <= ToolsContext.GamePartySize)
            {
                return;
            }

            var y = 10f + (2f / 3f * __instance.partyPlatesTable.rect.height);
            var guestPlatesTable = __instance.guestPlatesTable;

            guestPlatesTable.anchoredPosition = new Vector2(guestPlatesTable.anchoredPosition.x, -y);
        }
    }
}
