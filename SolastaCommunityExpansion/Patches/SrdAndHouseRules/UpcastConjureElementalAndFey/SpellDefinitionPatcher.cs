using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey
{
    // TODO: handle 2nd overload of Bind if using a device - e.g. staff of summoning 

    [HarmonyPatch(typeof(SpellDefinition), "SubspellsList", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellDefinition_SubspellsList
    {
        public static int? FilterBySlotLevel { get; internal set; }
        public static List<SpellDefinition> FilteredSubspells { get; internal set; }

        public static void Postfix(ref List<SpellDefinition> __result)
        {
            if (!FilterBySlotLevel.HasValue || __result == null || __result.Count == 0)
            {
                return;
            }

            var subspellsGroupedAndFilteredByCR = __result
                .Select(s =>
                    new
                    {
                        SpellDefinition = s,
                        s.EffectDescription
                            .GetFirstFormOfType(EffectForm.EffectFormType.Summon)
                            .SummonForm
                            .MonsterDefinitionName
                    }
                )
                .Select(s => new
                {
                    s.SpellDefinition,
                    s.MonsterDefinitionName,
                    ChallengeRating =
                        DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(s.MonsterDefinitionName, out var monsterDefinition)
                        ? monsterDefinition.ChallengeRating
                        : int.MaxValue
                })
                .GroupBy(s => s.ChallengeRating)
                .Select(g => new
                {
                    ChallengeRating = g.Key,
                    SpellDefinitions = g.Select(s => s.SpellDefinition).OrderBy(s => Gui.Format(s.GuiPresentation.Title))
                })
                .Where(s => s.ChallengeRating <= FilterBySlotLevel.Value)
                .OrderByDescending(s => s.ChallengeRating)
                .ToList();

            var allOrMostPowerful = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey
                ? subspellsGroupedAndFilteredByCR.Take(1).ToList()
                : subspellsGroupedAndFilteredByCR;

            FilteredSubspells = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();

            FilteredSubspells.ForEach(s => Main.Log($"{Gui.Format(s.GuiPresentation.Title)}"));

            __result = FilteredSubspells;
        }
    }
}
