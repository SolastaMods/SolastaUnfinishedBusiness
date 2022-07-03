using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Races;

[HarmonyPatch(typeof(GraphicsCharacter), "ResetScale")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal class GraphicsCharacter_ResetScale
{
    internal static void Postfix(GraphicsCharacter __instance, ref float __result)
    {
        if (__instance.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero ||
            !RacesContext.RaceScaleMap.TryGetValue(rulesetCharacterHero.RaceDefinition, out var scale))
        {
            return;
        }

        __result *= scale;
        __instance.transform.localScale = new Vector3(__result, __result, __result);
    }
}
