using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.DisableAutoEquip
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_GrantItem
    {
        public static void Prefix(RulesetCharacterHero __instance, ref bool tryToEquip)
        {
            if (Main.Settings.DisableAutoEquip && tryToEquip && __instance != null)
            {
                var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

                tryToEquip = characterBuildingService == null
                    // if not building character, disable as per setting
                    ? false 
                    // if building this character leave enabled, otherwise disable as per setting
                    : characterBuildingService.CurrentLocalHeroCharacter == __instance;
            }
        }
    }
}
