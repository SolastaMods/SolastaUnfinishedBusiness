using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using I2.Loc;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class EffectParticleParametersPatcher
{
    [HarmonyPatch(typeof(EffectParticleParameters), "Copy")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Copy_Patch
    {
        public static void Postfix([NotNull] EffectParticleParameters __instance, EffectParticleParameters reference)
        {
            //BUGFIX: it's missing 2 attribute copies as of 1.4.16
            __instance.beforeImpactParticleReference = reference.beforeImpactParticleReference;
            __instance.activeEffectCellParticleReference = reference.activeEffectCellParticleReference;
        }
    }
}
