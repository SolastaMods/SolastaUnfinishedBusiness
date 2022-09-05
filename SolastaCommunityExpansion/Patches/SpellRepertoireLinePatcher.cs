using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Patches;

internal static class SpellRepertoireLinePatcher
{
    [HarmonyPatch(typeof(SpellRepertoireLine), "FindAndSortRelevantSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FindAndSortRelevantSpells_Patch
    {
        internal static void Prefix([NotNull] List<SpellDefinition> spellDefinitions)
        {
            //PATCH: hide reaction spells from spell panel
            spellDefinitions.RemoveAll(x => x.ActivationTime == RuleDefinitions.ActivationTime.Reaction);
        }
    }
}
