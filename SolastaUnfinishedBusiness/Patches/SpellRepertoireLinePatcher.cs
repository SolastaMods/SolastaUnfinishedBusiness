using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellRepertoireLinePatcher
{
    [HarmonyPatch(typeof(SpellRepertoireLine), nameof(SpellRepertoireLine.FindAndSortRelevantSpells))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FindAndSortRelevantSpells_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] List<SpellDefinition> spellDefinitions)
        {
            //PATCH: hide reaction spells from spell panel
            spellDefinitions.RemoveAll(x => x.ActivationTime == ActivationTime.Reaction);
        }

        [UsedImplicitly]
        public static void Postfix([NotNull] List<SpellDefinition> spellDefinitions, SpellRepertoireLine __instance)
        {
            //PATCH: Enable Blast Reload feature
            var hero = __instance.caster.rulesetCharacter;

            hero?.GetSubFeaturesByType<PatronEldritchSurge.IQualifySpellToRepertoireLine>()
                .ForEach(f => f.QualifySpells(hero, __instance, spellDefinitions));
        }
    }
}
