using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationEffectPatcher
{
    //PATH: bypass effects serialization whenever we get an empty rulesetEffect
    [HarmonyPatch(typeof(GameLocationEffect), nameof(GameLocationEffect.SerializeAttributes))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RevealCharacter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationEffect __instance)
        {
            return __instance.rulesetEffect != null;
        }
    }
}
