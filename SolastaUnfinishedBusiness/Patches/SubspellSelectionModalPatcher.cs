using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SubspellSelectionModalPatcher
{
    [HarmonyPatch(typeof(SubspellSelectionModal), nameof(SubspellSelectionModal.OnActivate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivate_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(SubspellSelectionModal __instance, int index)
        {
            //PATCH: customizes subspell activation for upcasted elemental/fey
            return UpcastConjureElementalAndFey.CheckSubSpellActivated(__instance, index);
        }
    }

    [HarmonyPatch(typeof(SubspellSelectionModal), nameof(SubspellSelectionModal.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [HarmonyPatch([
        typeof(SpellDefinition), typeof(RulesetCharacter), typeof(RulesetSpellRepertoire),
        typeof(SpellsByLevelBox.SpellCastEngagedHandler), typeof(int), typeof(RectTransform)
    ])]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: replaces available subspell list with additional higher level elemental/fey
            var subspellsListMethod = typeof(SpellDefinition).GetMethod("get_SubspellsList");
            var getSpellList =
                new Func<SpellDefinition, int, List<SpellDefinition>>(UpcastConjureElementalAndFey.SubspellsList)
                    .Method;

            return instructions.ReplaceCalls(subspellsListMethod, "SubspellSelectionModal.Bind",
                new CodeInstruction(OpCodes.Ldarg, 5),
                new CodeInstruction(OpCodes.Call, getSpellList));
        }
    }
}
