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

    // TODO: handle 2nd overload of Bind if using a device - e.g. staff of summoning 

    [HarmonyPatch(typeof(SpellDefinition), "SubspellsList", MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellDefinition_SubspellsList
    {
        public static int? FilterBySlotLevel { get; internal set; }

        public static void Postfix(ref List<SpellDefinition> __result)
        {
            if (!FilterBySlotLevel.HasValue)
            {
                return;
            }

            __result = __result
                .Where(s =>
                {
                    var monsterName = s.EffectDescription
                        .GetFirstFormOfType(EffectForm.EffectFormType.Summon)
                        .SummonForm
                        .MonsterDefinitionName;

                    return DatabaseRepository.GetDatabase<MonsterDefinition>().TryGetElement(monsterName, out var monsterDefinition)
                        && monsterDefinition.ChallengeRating <= FilterBySlotLevel.Value;
                })
                .ToList();
        }
    }
}
