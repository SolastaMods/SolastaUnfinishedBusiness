using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameCampaignCharacterPatcher
{
    [HarmonyPatch(typeof(GameCampaignCharacter), "EngageRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class EngageRest_Patch
    {
        public static bool Prefix([NotNull] GameCampaignCharacter __instance, RuleDefinitions.RestType restType)
        {
            //BUGFIX: terminates effects correctly on world travel
            // call `RefreshEffectsForRest` instead of `ApplyRestForConditions` for heroes
            // this makes powers and spells that last until rest properly terminate on rest during world travel
            if (__instance.RulesetCharacter is not RulesetCharacterHero hero)
            {
                return true;
            }

            hero.RefreshEffectsForRest(restType);
            return false;
        }
    }
}
