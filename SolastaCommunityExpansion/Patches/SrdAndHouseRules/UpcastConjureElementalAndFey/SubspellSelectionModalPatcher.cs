using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaModApi;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.UpcastConjureElementalAndFey
{
    [HarmonyPatch(typeof(SubspellSelectionModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch(new Type[] {
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)})]
    internal static class SubspellSelectionModal_Bind
    {
        internal static List<SpellDefinition> MySubspellsListCache { get; set; }

        public static List<SpellDefinition> MySubspellsList()
        {
            return MySubspellsListCache;
        }

        public static void ResetMySubspellsList()
        {
            MySubspellsListCache = null;
        }

        public static void CacheMySubspellsList(SubspellSelectionModal subspellSelectionModal, SpellDefinition masterSpell)
        {
            MySubspellsListCache = masterSpell.SubspellsList;

            if (!Main.Settings.EnableUpcastConjureElementalAndFey
                || MySubspellsListCache == null
                || masterSpell.Name != DatabaseHelper.SpellDefinitions.ConjureElemental.Name
                || masterSpell.Name != DatabaseHelper.SpellDefinitions.ConjureFey.Name)
            {
                return;
            }

            var slotLevel = subspellSelectionModal.GetField<SubspellSelectionModal, int>("slotLevel");

            var subspellsGroupedAndFilteredByCR = MySubspellsListCache
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
                .Where(s => s.ChallengeRating <= slotLevel)
                .OrderByDescending(s => s.ChallengeRating)
                .ToList();

            var allOrMostPowerful = Main.Settings.OnlyShowMostPowerfulUpcastConjuredElementalOrFey
                ? subspellsGroupedAndFilteredByCR.Take(1).ToList()
                : subspellsGroupedAndFilteredByCR;

            MySubspellsListCache = allOrMostPowerful.SelectMany(s => s.SpellDefinitions).ToList();
            MySubspellsListCache.ForEach(s => Main.Log($"{Gui.Format(s.GuiPresentation.Title)}"));
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var subspellsListMethod = typeof(SpellDefinition).GetMethod("get_SubspellsList");
            var cacheMySubspellsListMethod = typeof(SubspellSelectionModal_Bind).GetMethod("CacheMySubspellsList");
            var mySubspellsListMethod = typeof(SubspellSelectionModal_Bind).GetMethod("MySubspellsList");
            var resetMySubspellsListMethod = typeof(SubspellSelectionModal_Bind).GetMethod("ResetMySubspellsList");

            //
            // caches the result from masterSpell.SubspellsList
            //

            yield return new CodeInstruction(OpCodes.Ldarg_0); // SubspellSelectionModal (this)
            yield return new CodeInstruction(OpCodes.Ldarg_1); // SpellDefinition (masterSpell)
            yield return new CodeInstruction(OpCodes.Call, cacheMySubspellsListMethod);

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(subspellsListMethod))
                {
                    yield return new CodeInstruction(OpCodes.Pop); // remove SubspellSelectionModal (this)
                    yield return new CodeInstruction(OpCodes.Call, mySubspellsListMethod); // returns the cached result
                }
                else if (instruction.opcode == OpCodes.Ret) 
                {
                    yield return new CodeInstruction(OpCodes.Call, resetMySubspellsListMethod);
                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }

    [HarmonyPatch(typeof(SubspellSelectionModal), "OnActivate")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SubspellSelectionModal_OnActivate
    {
        internal static bool Prefix(SubspellSelectionModal __instance, int index, int ___slotLevel,
            RulesetSpellRepertoire ___spellRepertoire, SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged)
        {
            if (!Main.Settings.EnableUpcastConjureElementalAndFey ||
                SubspellSelectionModal_Bind.MySubspellsListCache == null ||
                SubspellSelectionModal_Bind.MySubspellsListCache.Count == 0)
            {
                return true;
            }

            var subspells = SubspellSelectionModal_Bind.MySubspellsListCache;

            if (subspells.Count > index)
            {
                ___spellCastEngaged?.Invoke(___spellRepertoire, SubspellSelectionModal_Bind.MySubspellsListCache[index], ___slotLevel);

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
}
