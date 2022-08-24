using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class SubspellItemPatcher
{
    [HarmonyPatch(typeof(SubspellItem), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        internal static bool Prefix(
            SubspellItem __instance,
            RulesetCharacter caster,
            SpellDefinition spellDefinition,
            int index,
            SubspellItem.OnActivateHandler onActivate)
        {
            //PATCH: replaces tooltip and name with the ones from actual sub-power being represented by this spell
            return PowerBundleContext.ModifySubspellItemBind(__instance, caster, spellDefinition, index, onActivate);
        }
    }
}