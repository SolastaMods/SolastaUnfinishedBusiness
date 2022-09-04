using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize;

//PATCH: scales down the rest sub panel whenever the party size is bigger than 4
//
// this patch is protected by partyCount result
//
[HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RestSubPanel_OnBeginShow
{
    internal static void Prefix([NotNull] RestSubPanel __instance)
    {
        var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

        var width = 128 * partyCount;
        __instance.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        var modules = __instance.restModulesTable;

        for (var i = 0; i < modules.childCount; i++)
        {
            modules.GetChild(i)
                .GetComponent<RectTransform>()
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }
}
