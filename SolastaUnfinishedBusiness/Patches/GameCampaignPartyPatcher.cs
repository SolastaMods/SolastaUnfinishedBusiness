using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameCampaignPartyPatcher
{
    //PATCH: Correctly updates the level cap under Level 20 scenarios
    [HarmonyPatch(typeof(GameCampaignParty), nameof(GameCampaignParty.UpdateLevelCaps))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UpdateLevelCaps_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] GameCampaignParty __instance, int levelCap)
        {
            var max = Main.Settings.EnableLevel20 ? Level20Context.ModMaxLevel : Level20Context.GameMaxLevel;

            levelCap = Main.Settings.EnableLevel20 || levelCap == 0 ? max : levelCap;

            foreach (var character in __instance.CharactersList)
            {
                var characterLevel = character.RulesetCharacter.GetAttribute(AttributeDefinitions.CharacterLevel);
                var experience = character.RulesetCharacter.GetAttribute(AttributeDefinitions.Experience);

                characterLevel.MaxValue = levelCap;
                characterLevel.Refresh();

                experience.MaxValue = HeroDefinitions.MaxHeroExperience(characterLevel.MaxValue);
                experience.Refresh();
            }

            return false;
        }
    }
}
