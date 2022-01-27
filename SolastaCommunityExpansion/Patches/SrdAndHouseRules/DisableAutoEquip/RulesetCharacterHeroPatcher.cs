using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static SolastaModApi.DatabaseHelper.QuestTreeDefinitions;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.DisableAutoEquip
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_GrantItem
    {
        public static void Prefix(ref bool tryToEquip)
        {
            if (Main.Settings.DisableAutoEquip)
            {
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

                tryToEquip = characterBuildingService?.HeroCharacter != null;
            }
        }
    }
}
