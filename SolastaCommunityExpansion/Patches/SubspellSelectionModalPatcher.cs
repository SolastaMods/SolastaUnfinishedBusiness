using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class SubspellSelectionModalPatcher
{
    //PATCH: customizes subspell activation for upcasted elemental/fey
    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class OnActivate_Patch
    {
        internal static bool Prefix(SubspellSelectionModal __instance, int index)
        {
            return UpcastConjureElementalAndFey.CheckSubSpellActivated(__instance, index);
        }
    }

    //PATCH: replaces available subspell list with additional higher level elemental/fey
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new[]
    {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)
    })]
    internal static class Bind_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return UpcastConjureElementalAndFey.ReplaceSubSpellList(instructions).ToList();
        }
    }
}
