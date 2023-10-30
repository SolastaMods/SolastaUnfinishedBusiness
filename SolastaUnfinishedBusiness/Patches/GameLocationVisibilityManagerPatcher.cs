using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Races;

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
}
