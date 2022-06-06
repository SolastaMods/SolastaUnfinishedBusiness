using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.BugFix;

[HarmonyPatch(typeof(SpellRepertoireLine), "FindAndSortRelevantSpells")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellsByLevelGroup_BindInspectionOrPreparation
{
    internal static void Prefix(List<SpellDefinition> spellDefinitions)
    {
        //
        // BUGFIX: hide reaction spells
        //

        spellDefinitions.RemoveAll(x => x.ActivationTime == RuleDefinitions.ActivationTime.Reaction);
    }
}
