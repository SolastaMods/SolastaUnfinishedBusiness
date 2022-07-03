using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Bugfix;

internal static class GameCampaignCharacterPatcher
{
    // call `RefreshEffectsForRest` instead of `ApplyRestForConditions` for ruleset characters
    // this makes powers and spells that last until rest properly terminate on rest during world travel
    [HarmonyPatch(typeof(GameCampaignCharacter), "EngageRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameCampaignCharacter_EngageRest
    {
        internal static bool Prefix(GameCampaignCharacter __instance, RuleDefinitions.RestType restType)
        {
            //
            // BUGFIX: correctly terminate effects on world travelS
            //

            if (__instance.RulesetCharacter is not RulesetCharacterHero hero)
            {
                return true;
            }

            hero.RefreshEffectsForRest(restType);

            return false;
        }
    }
}
