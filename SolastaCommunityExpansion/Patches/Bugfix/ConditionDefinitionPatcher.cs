using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(ConditionDefinition), "GetParticleParametersFromAncestryDamageType")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ConditionDefinition_GetParticleParametersFromAncestryDamageType
    {
        // this patches fixes an exception thrown for below damage type [there is no damage form particle parameters so using acid]
        public static bool Prefix(string ancestryDamageType, ref ConditionParticleParameters __result, ConditionParticleParameters ___acidParticleParameters)
        {
            if (ancestryDamageType == "DamageForce")
            {
                __result = ___acidParticleParameters;

                return false;
            }

            return true;
        }
    }
}
