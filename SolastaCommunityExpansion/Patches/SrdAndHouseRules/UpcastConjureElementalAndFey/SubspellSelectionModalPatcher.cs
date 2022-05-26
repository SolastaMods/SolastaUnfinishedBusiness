using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new[]
    {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)
    })]
    internal static class SubspellSelectionModal_Bind
    {
        public static List<SpellDefinition> FilteredSubspells { get; internal set; }

        public static List<SpellDefinition> MySubspellsList(SpellDefinition masterSpell, int slotLevel)
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
                        .OrderBy(s => Gui.Format(s.GuiPresentation.Title))
                })
                .Where(s => s.ChallengeRating <= mySlotLevel)
                .OrderByDescending(s => s.ChallengeRating)
                .ToList();

            var allOrMostPowerful = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey
                ? subspellsGroupedAndFilteredByCR.Take(1).ToList()
                : subspellsGroupedAndFilteredByCR;

            FilteredSubspells = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();

            FilteredSubspells.ForEach(s => Main.Log($"{Gui.Format(s.GuiPresentation.Title)}"));

            return FilteredSubspells;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var subspellsListMethod = typeof(SpellDefinition).GetMethod("get_SubspellsList");
            var mySubspellsListMethod = typeof(SubspellSelectionModal_Bind).GetMethod("MySubspellsList");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(subspellsListMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg, 5); // slotLevel
                    yield return new CodeInstruction(OpCodes.Call, mySubspellsListMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    //
    //  This is now handled at SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle
    //
#if false
    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        public static bool Prefix(SubspellSelectionModal __instance, int index, int ___slotLevel,
            RulesetSpellRepertoire ___spellRepertoire, SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged)
        {
            if (Main.Settings.EnableUpcastConjureElementalAndFey 
                && SubspellSelectionModal_Bind.FilteredSubspells != null
                && SubspellSelectionModal_Bind.FilteredSubspells.Count > 0)
            {
                var subspells = SubspellSelectionModal_Bind.FilteredSubspells;

                if (subspells.Count > index)
                {
                    ___spellCastEngaged?.Invoke(___spellRepertoire, SubspellSelectionModal_Bind.FilteredSubspells[index], ___slotLevel);

                    // If a device had the summon function, implement here

                    //else if (this.deviceFunctionEngaged != null)
                    //    this.deviceFunctionEngaged(this.guiCharacter, this.rulesetItemDevice, this.rulesetDeviceFunction, 0, index);

                    __instance.Hide();

                    subspells.Clear();

                    return false;
                }
            }

            return true;
        }
    }
#endif
}
