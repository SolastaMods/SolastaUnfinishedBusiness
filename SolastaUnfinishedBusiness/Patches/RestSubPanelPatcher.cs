using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class RestSubPanelPatcher
{
    [HarmonyPatch(typeof(RestSubPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow_Patch
    {
        public static void Prefix([NotNull] RestSubPanel __instance)
        {
            //PATCH: scales down the rest sub panel whenever the party size is bigger than 4 (PARTYSIZE)
            var partyCount = Math.Min(Gui.GameCampaign.Party.CharactersList.Count, ToolsContext.GamePartySize);
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
}
