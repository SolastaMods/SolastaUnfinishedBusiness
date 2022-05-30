using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.DisableAutoEquip;

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

        tryToEquip = __instance.TryGetHeroBuildingData(out var _);
    }
}
