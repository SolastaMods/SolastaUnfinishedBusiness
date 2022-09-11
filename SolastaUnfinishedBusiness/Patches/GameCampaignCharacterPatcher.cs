using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameCampaignCharacterPatcher
{
    [HarmonyPatch(typeof(GameCampaignCharacter), "EngageRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class EngageRest_Patch
    {
        internal static bool Prefix([NotNull] GameCampaignCharacter __instance, RuleDefinitions.RestType restType)
        {
            // PATCH: BUGFIX: correctly terminate effects on world travel
            // call `RefreshEffectsForRest` instead of `ApplyRestForConditions` for heroes
            // this makes powers and spells that last until rest properly terminate on rest during world travel
            if (__instance.RulesetCharacter is RulesetCharacterHero hero)
            {
                hero.RefreshEffectsForRest(restType);
                return false;
            }

            return true;
        }
    }
}
