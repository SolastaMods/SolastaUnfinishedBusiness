using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;

namespace SolastaUnfinishedBusiness.BehaviorsSpecific;

internal static class UpcastConjureElementalAndFey
{
    private static List<SpellDefinition> _filteredSubspells;

    /**
     * Patch implementation
     * Replaces subspell activation with custom code for upcasted elementals/fey
     */
    internal static bool CheckSubSpellActivated(SubspellSelectionModal __instance, int index)
    {
        if (!Main.Settings.EnableUpcastConjureElementalAndFey || _filteredSubspells is not { Count: > 0 })
        {
            return true;
        }

        if (_filteredSubspells.Count <= index)
        {
            return true;
        }

        __instance.spellCastEngaged?.Invoke(
            __instance.spellRepertoire, _filteredSubspells[index], __instance.slotLevel);

        __instance.Hide();

        _filteredSubspells.Clear();

        return false;
    }

    /**
     * Patch implementation
     * Replaces calls to masterSpell.SubspellsList getter with custom method that adds extra options for upcasted elementals/fey
     */
    [CanBeNull]
    internal static List<SpellDefinition> SubspellsList([NotNull] SpellDefinition masterSpell, int slotLevel)
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
                })
            .Select(s => new
            {
                s.SpellDefinition,
                s.MonsterDefinitionName,
                ChallengeRating =
                    DatabaseHelper.TryGetDefinition<MonsterDefinition>(s.MonsterDefinitionName,
                        out var monsterDefinition)
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

        _filteredSubspells = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();

        return _filteredSubspells;
    }
}
