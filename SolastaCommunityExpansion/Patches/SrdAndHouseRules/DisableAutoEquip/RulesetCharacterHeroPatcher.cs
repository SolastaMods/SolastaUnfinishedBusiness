using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.DisableAutoEquip
{
    [HarmonyPatch(typeof(RulesetCharacterHero), "GrantItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacterHero_GrantItem
    {
        internal static void Prefix(RulesetCharacterHero __instance, ref bool tryToEquip)
        {
            if (!Main.Settings.DisableAutoEquip || !tryToEquip)
            {
                return;
            }

            // Comment out.

            //var buildingDataByHero = typeof(CharacterHeroBuildingData)
            //    .GetField("buildingDataByHero", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            //    .GetValue(null) as Dictionary<RulesetCharacterHero, CharacterHeroBuildingData>;

            //tryToEquip = buildingDataByHero.Keys.Any(x => x.Name == __instance.Name);
        }
    }
}
