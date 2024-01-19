using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Races;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationVisibilityManagerPatcher
{
    //PATH: supports `RaceLightSensitivityApplyOutdoorsOnly`
    //need to force lightning affinity effects recalculation if one of the dark races
    [HarmonyPatch(typeof(GameLocationVisibilityManager), nameof(GameLocationVisibilityManager.ComputeIllumination))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RevealCharacter_Patch
    {
        [UsedImplicitly]
        public static void Postfix(LocationDefinitions.LightingState __result, IIlluminable illuminable)
        {
            if (__result != LocationDefinitions.LightingState.Bright ||
                illuminable is not GameLocationCharacter { RulesetCharacter: not null } glc)
            {
                return;
            }

            var originalHero = glc.RulesetCharacter.GetOriginalHero();

            if (originalHero != null &&
                (originalHero.SubRaceDefinition == RaceKoboldBuilder.SubraceDarkKobold ||
                 originalHero.SubRaceDefinition == SubraceDarkelfBuilder.SubraceDarkelf ||
                 originalHero.SubRaceDefinition == SubraceGrayDwarfBuilder.SubraceGrayDwarf))
            {
                glc.CheckLightingAffinityEffects();
            }
        }
    }

    //PATCH: supports lighting and obscurement feature by allowing ranged targeting within obscured areas
    //this is mainly vanilla code except for the BEGIN/END patch block
    [HarmonyPatch(typeof(GameLocationVisibilityManager),
        nameof(GameLocationVisibilityManager.IsPositionPerceivedByCharacter))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsPositionPerceivedByCharacter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationVisibilityManager __instance,
            ref bool __result,
            Vector3 position,
            Vector3 origin,
            RulesetCharacter rulesetCharacter,
            List<SenseMode.Type> optionalRequiredSense = null)
        {
            var fromWorldPosition1 =
                __instance.gameLocationPositioningService.GetGridPositionFromWorldPosition(origin);
            var fromWorldPosition2 =
                __instance.gameLocationPositioningService.GetGridPositionFromWorldPosition(position);
            var hasImpairedSight = false;

            // BEGIN PATCH
            // impaired sight should not prevent targeting
            if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules)
            {
                var gridAccessor = GridAccessor.Default;

                hasImpairedSight =
                    rulesetCharacter.ImpairedSight ||
                    (gridAccessor.RuntimeFlags(fromWorldPosition1) & CellFlags.Runtime.DynamicSightImpaired) != 0;
            }
            // END PATCH

            var magnitude = (fromWorldPosition1 - fromWorldPosition2).magnitude;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var senseMode in rulesetCharacter.SenseModes)
            {
                var senseType = senseMode.SenseType;

                if (senseType == SenseMode.Type.None ||
                    (optionalRequiredSense is { Count: > 0 } && !optionalRequiredSense.Contains(senseType)))
                {
                    continue;
                }

                var senseRange = senseMode.SenseRange;

                if (magnitude > senseRange ||
                    (hasImpairedSight && SenseMode.CanBeImpaired(senseType) && magnitude > 1.7999999523162842) ||
                    __instance.gameLocationPositioningService.RaycastGridSightBlocker(
                        origin, position, __instance.GameLocationService))
                {
                    continue;
                }

                __result = true;

                return false;
            }

            __result = false;

            return false;
        }
    }
}
