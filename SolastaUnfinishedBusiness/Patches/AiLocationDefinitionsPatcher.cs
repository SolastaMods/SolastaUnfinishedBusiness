using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AiLocationDefinitionsPatcher
{
    //PATCH: supports `UseOfficialLightingObscurementAndVisionRules`
    [HarmonyPatch(typeof(AiLocationDefinitions), nameof(AiLocationDefinitions.IsValidMagicEffect))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CanCastMagic_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            ref bool __result,
            AiLocationCharacter aiCharacter,
            IMagicEffect magicEffect,
            GameLocationCharacter optionalTarget)
        {
            var locationCharacter = aiCharacter.GameLocationCharacter;

            if (__result &&
                !LightingAndObscurementContext.IsMagicEffectValidIfHeavilyObscuredOrInNaturalDarkness(
                    locationCharacter, magicEffect, optionalTarget))
            {
                __result = false;
            }
        }
    }
}
