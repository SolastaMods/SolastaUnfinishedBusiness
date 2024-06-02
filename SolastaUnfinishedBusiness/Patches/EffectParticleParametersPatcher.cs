using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class EffectParticleParametersPatcher
{
    //PATCH: supports Earthquake spell interaction with widened metamagic
    [HarmonyPatch(typeof(EffectParticleParameters),
        nameof(EffectParticleParameters.GetActiveEffectSurfaceParticlePerIndex))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetActiveEffectSurfaceParticlePerIndex_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(EffectParticleParameters __instance, out GameObject __result, int cellIndex)
        {
            __result = null;

            if (string.IsNullOrEmpty(__instance.activeEffectSurfaceParticlePerIndex))
            {
                return false;
            }

            //BEGIN PATCH
            if (__instance.activeEffectSurfaceParticlePerIndex == "Earthquake" &&
                cellIndex > 143)
            {
                cellIndex -= 36;
            }
            //END PATCH

            if (ServiceRepository.GetService<IGameLocationService>() is { LocationShuttingDown: false })
            {
                __result = ServiceRepository.GetService<IGraphicsResourceService>()
                    .FetchOrLoadAssetSync<GameObject>(
                        $"{(object)__instance.activeEffectSurfaceParticlePerIndex}_{(object)cellIndex}",
                        GraphicsDefinitions.RetentionScope.Location);
            }

            return false;
        }
    }
}
