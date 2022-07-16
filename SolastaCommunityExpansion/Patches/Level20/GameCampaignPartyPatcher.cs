using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.Level20;

[HarmonyPatch(typeof(GameCampaignParty), "UpdateLevelCaps")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameCampaignParty_UpdateLevelCaps
{
    internal static bool Prefix([NotNull] GameCampaignParty __instance, int levelCap)
    {
        var max = Main.Settings.EnableLevel20 ? Level20Context.ModMaxLevel : Level20Context.GameMaxLevel;

        levelCap = Main.Settings.OverrideMinMaxLevel ? Level20Context.ModMaxLevel : levelCap;

        foreach (GameCampaignCharacter characters in __instance.CharactersList)
        {
            var characterLevel = characters.RulesetCharacter.GetAttribute("CharacterLevel");
            var experience = characters.RulesetCharacter.GetAttribute("Experience");

            characterLevel.MaxValue = levelCap > 0 ? Mathf.Min(levelCap, max) : max;
            characterLevel.Refresh();

            experience.MaxValue = HeroDefinitions.MaxHeroExperience(characterLevel.MaxValue);
            experience.Refresh();
        }

        return false;
    }
}
