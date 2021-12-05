using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static SolastaCommunityExpansion.Models.Level20Context;

namespace SolastaCommunityExpansion.Patches.Level20
{
    // replaces the hard-coded level and max experience
    [HarmonyPatch(typeof(GameCampaignParty), "UpdateLevelCaps")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameCampaignParty_UpdateLevelCaps
    {
        internal static bool Prefix(GameCampaignParty __instance)
        {
            if (Main.Settings.EnableLevel20)
            {
                foreach (var character in __instance.CharactersList.Select(cl => cl.RulesetCharacter))
                {
                    RulesetAttribute characterLevelAttribute = character.GetAttribute("CharacterLevel");
                    characterLevelAttribute.MaxValue = MOD_MAX_LEVEL;
                    characterLevelAttribute.Refresh();

                    RulesetAttribute experienceAttribute = character.GetAttribute("Experience");
                    experienceAttribute.MaxValue = MAX_CHARACTER_EXPERIENCE;
                    experienceAttribute.Refresh();
                }

                return false;
            }

            return true;
        }
    }
}
