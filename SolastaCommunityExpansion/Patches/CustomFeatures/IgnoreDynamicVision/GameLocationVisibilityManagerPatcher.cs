using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.IgnoreDynamicVision
{
    [HarmonyPatch(typeof(GameLocationVisibilityManager), "IsPositionPerceivedByCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationVisibilityManager_IsPositionPerceivedByCharacter
    {
        internal static void Postfix(GameLocationVisibilityManager __instance,
            Vector3 position,
            Vector3 origin,
            RulesetCharacter rulesetCharacter,
            ref bool __result)
        {
            var gridAccessor = GridAccessor.Default;
            var positioning = ServiceRepository.GetService<IGameLocationPositioningService>();
            if (__result
                || rulesetCharacter.ImpairedSight
                || !((gridAccessor.RuntimeFlags(positioning.GetGridPositionFromWorldPosition(origin)) & CellFlags.Runtime.DynamicSightImpaired) > 0U)
                || positioning.RaycastGridSightBlocker(origin, position, __instance.GameLocationService)
               )
            {
                return;
            }

            var features = rulesetCharacter.GetFeaturesByType<FeatureDefinitionIgnoreDynamicVisionImpairment>();
            if (features.Empty()) { return; }

            var range = (positioning.GetGridPositionFromWorldPosition(origin) -
                           positioning.GetGridPositionFromWorldPosition(position)).magnitude;

            __result = features.Any(f => f.CanIgnoreDynamicVisionImpairment(rulesetCharacter, range));
        }
    }
}
