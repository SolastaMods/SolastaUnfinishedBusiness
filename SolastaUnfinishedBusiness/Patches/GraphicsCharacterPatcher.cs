using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GraphicsCharacterPatcher
{
    [HarmonyPatch(typeof(GraphicsCharacter), nameof(GraphicsCharacter.ResetScale))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ResetScale_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GraphicsCharacter __instance, ref float __result)
        {
            //PATCH: Allows custom races with different scales
            if (__instance.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero ||
                !RacesContext.RaceScaleMap.TryGetValue(rulesetCharacterHero.RaceDefinition, out var scale))
            {
                return;
            }

            __result *= scale;
            __instance.transform.localScale = new Vector3(__result, __result, __result);
        }
    }
}
