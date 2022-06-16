using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.Crafting;

[HarmonyPatch(typeof(RulesetCharacterHero), "ComputeCraftingDurationHours")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacterHero_GComputeCraftingDurationHours
{
    internal static void Postfix(ref int __result)
    {
        __result = (int)((100f - Main.Settings.TotalCraftingTimeModifier) * __result);
    }
}
