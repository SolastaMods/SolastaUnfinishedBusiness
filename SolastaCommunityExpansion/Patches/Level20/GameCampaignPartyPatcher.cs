using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches
{
    class GameCampaignPartyPatcher
    {
        // replaces the hard-coded level and max experience
        [HarmonyPatch(typeof(GameCampaignParty), "UpdateLevelCaps")]
        internal static class GameCampaignParty_UpdateLevelCaps_Patch
        {
            internal static bool Prefix(GameCampaignParty __instance)
            {
                foreach (GameCampaignCharacter characters in __instance.CharactersList)
                {
                    RulesetAttribute characterLevelAttribute = characters.RulesetCharacter.GetAttribute("CharacterLevel");
                    characterLevelAttribute.MaxValue = MOD_MAX_LEVEL;
                    characterLevelAttribute.Refresh();

                    RulesetAttribute experienceAttribute = characters.RulesetCharacter.GetAttribute("Experience");
                    experienceAttribute.MaxValue = MAX_CHARACTER_EXPERIENCE;
                    experienceAttribute.Refresh();
                }

                return false;
            }
        }
    }
}