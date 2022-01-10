using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaModApi;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.GameUiSpellSelection
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new Type[] {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)})]
    internal static class SubspellSelectionModal_Bind
    {
        public static void Prefix(SpellDefinition masterSpell, int slotLevel)
        {
            if (!Main.Settings.EnableUpcastConjureElementalAndFey)
            {
                return;
            }

            SpellDefinition_SubspellsList.FilterBySlotLevel =
                masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureElemental.Name
                || masterSpell.Name == DatabaseHelper.SpellDefinitions.ConjureFey.Name
                ? slotLevel
                : (int?)null;
        }

        public static void Postfix()
        {
            SpellDefinition_SubspellsList.FilterBySlotLevel = null;
        }
    }

    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        public static bool Prefix(SubspellSelectionModal __instance, int index, int ___slotLevel,
            RulesetSpellRepertoire ___spellRepertoire, SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged)
        {
            if (!Main.Settings.EnableUpcastConjureElementalAndFey
               || !SpellDefinition_SubspellsList.FilteredSubspells.Any())
            {
                return true;
            }

            var subspells = SpellDefinition_SubspellsList.FilteredSubspells;

            if (subspells.Count > index)
            {
                if (___spellCastEngaged != null)
                    ___spellCastEngaged(
                        ___spellRepertoire,
                        SpellDefinition_SubspellsList.FilteredSubspells[index],
                        ___slotLevel);

                // If a device had the summon function, implement here

                //else if (this.deviceFunctionEngaged != null)
                //    this.deviceFunctionEngaged(this.guiCharacter, this.rulesetItemDevice, this.rulesetDeviceFunction, 0, index);

                __instance.Hide();

                subspells.Clear();

                return false;
            }

            return true;
        }
    }

    // TODO: handle 2nd overload of Bind if using a device - e.g. staff of summoning 

    [HarmonyPatch(typeof(SpellDefinition), "SubspellsList", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellDefinition_SubspellsList
    {
        public static int? FilterBySlotLevel { get; internal set; }
        public static List<SpellDefinition> FilteredSubspells { get; internal set; }

        public static void Postfix(ref List<SpellDefinition> __result)
        {
            if (!FilterBySlotLevel.HasValue)
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
                .OrderByDescending(s => s.ChallengeRating)
                .Where(s => s.ChallengeRating <= FilterBySlotLevel.Value)
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
