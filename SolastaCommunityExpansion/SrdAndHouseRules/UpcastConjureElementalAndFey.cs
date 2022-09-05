﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.SrdAndHouseRules;

internal static class UpcastConjureElementalAndFey
{
    private static List<SpellDefinition> FilteredSubspells;


    /**
     * Patch implementation
     * Replaces subspell activation with custom code for upcasted elementals/fey
     */
    internal static bool CheckSubSpellActivated(SubspellSelectionModal __instance, int index)
    {
        if (Main.Settings.EnableUpcastConjureElementalAndFey && FilteredSubspells is { Count: > 0 })
        {
            if (FilteredSubspells.Count > index)
            {
                __instance.spellCastEngaged?.Invoke(__instance.spellRepertoire, FilteredSubspells[index],
                    __instance.slotLevel);

                __instance.Hide();

                FilteredSubspells.Clear();

                return false;
            }
        }

        return true;
    }

    /**
     * Patch implementation
     * Replaces calls to masterSpell.SubspellsList getter with custom method that adds extra options for upcasted elementals/fey
     */
    internal static IEnumerable<CodeInstruction> ReplaceSubSpellList(IEnumerable<CodeInstruction> instructions)
    {
        var subspellsListMethod = typeof(SpellDefinition).GetMethod("get_SubspellsList");
        var getSpellList = new Func<SpellDefinition, int, List<SpellDefinition>>(SubspellsList);

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(subspellsListMethod))
            {
                yield return new CodeInstruction(OpCodes.Ldarg, 5); // slotLevel
                yield return new CodeInstruction(OpCodes.Call, getSpellList.Method);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    [CanBeNull]
    private static List<SpellDefinition> SubspellsList([NotNull] SpellDefinition masterSpell, int slotLevel)
    {
        var subspellsList = masterSpell.SubspellsList;
        var mySlotLevel = masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureElemental.Name
                          || masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureFey.Name
            ? slotLevel
            : -1;

        if (!Main.Settings.EnableUpcastConjureElementalAndFey || mySlotLevel < 0 || subspellsList == null ||
            subspellsList.Count == 0)
        {
            return subspellsList;
        }

        var subspellsGroupedAndFilteredByCR = subspellsList
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
                    DatabaseRepository.GetDatabase<MonsterDefinition>()
                        .TryGetElement(s.MonsterDefinitionName, out var monsterDefinition)
                        ? monsterDefinition.ChallengeRating
                        : int.MaxValue
            })
            .GroupBy(s => s.ChallengeRating)
            .Select(g => new
            {
                ChallengeRating = g.Key,
                SpellDefinitions = g.Select(s => s.SpellDefinition)
                    .OrderBy(s => Gui.Localize(s.GuiPresentation.Title))
            })
            .Where(s => s.ChallengeRating <= mySlotLevel)
            .OrderByDescending(s => s.ChallengeRating)
            .ToList();

        var allOrMostPowerful = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey
            ? subspellsGroupedAndFilteredByCR.Take(1).ToList()
            : subspellsGroupedAndFilteredByCR;

        FilteredSubspells = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();

        FilteredSubspells.ForEach(s => Main.Log($"{Gui.Localize(s.GuiPresentation.Title)}"));

        return FilteredSubspells;
    }
}
