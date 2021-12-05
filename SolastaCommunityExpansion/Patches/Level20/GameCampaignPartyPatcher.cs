using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // replaces the hard-coded level and max experience
    [HarmonyPatch(typeof(GameCampaignParty), "UpdateLevelCaps")]
    internal static class GameCampaignParty_UpdateLevelCaps
    {
        internal static bool Prefix(GameCampaignParty __instance)
        {
            if (Main.Settings.EnableLevel20)
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

            return true;
        }
    }
}
