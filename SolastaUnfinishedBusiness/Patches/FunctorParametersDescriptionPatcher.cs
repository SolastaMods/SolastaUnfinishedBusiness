using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

//PATCH: ensures all party members teleport to new locations (PARTYSIZE)
[HarmonyPatch(typeof(FunctorParametersDescription), "PlayerPlacementMarkers", MethodType.Getter)]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class FunctorParametersDescription_PlayerPlacementMarkers
{
    internal static void Postfix(ref Transform[] __result)
    {
        var partyCount = Gui.GameCampaign.Party.CharactersList.Count;

        if (partyCount <= 4 || __result.Length <= 0)
        {
            return;
        }

        var result = new Transform[partyCount];

        for (var idx = 0; idx < partyCount; idx++)
        {
            result[idx] = __result[0];
        }

        __result = result;
    }
}
