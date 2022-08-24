using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomAttacks;

[HarmonyPatch(typeof(RulesetItem), "FillTags")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetItem_FillTags
{
    public static void Postfix(
        RulesetItem __instance,
        Dictionary<string, TagsDefinitions.Criticity> tagsMap,
        object context,
        bool active = false)
    {
        if (WeaponValidators.IsPolearm(__instance))
        {
            tagsMap.TryAdd(CustomWeaponsContext.PolearmWeaponTag, TagsDefinitions.Criticity.Normal);
        }

        if (DiagnosticsContext.IsCeDefinition(__instance.ItemDefinition))
        {
            tagsMap.TryAdd(CeContentPackContext.CeTag, TagsDefinitions.Criticity.Normal);
        }
    }
}
