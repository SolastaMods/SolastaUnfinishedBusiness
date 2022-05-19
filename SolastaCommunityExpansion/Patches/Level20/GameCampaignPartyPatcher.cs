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
            foreach (var character in __instance.CharactersList.Select(cl => cl.RulesetCharacter))
            {
                var characterLevelAttribute = character.GetAttribute(AttributeDefinitions.CharacterLevel);
                characterLevelAttribute.MaxValue = MOD_MAX_LEVEL;
                characterLevelAttribute.Refresh();

                var experienceAttribute = character.GetAttribute(AttributeDefinitions.Experience);
                experienceAttribute.MaxValue = MAX_CHARACTER_EXPERIENCE;
                experienceAttribute.Refresh();
            }

            return false;
        }
    }
}
