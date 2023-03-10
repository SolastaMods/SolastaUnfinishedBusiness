using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class AttunementModalPatcher
{
    [HarmonyPatch(typeof(AttunementModal), nameof(AttunementModal.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var method = new Func<AttunementModal, int>(GetLimit).Method;

            return instructions.ReplaceCode(x => x.opcode == OpCodes.Ldc_I4_3,
                -1, "AttunementModal.Refresh",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, method));
        }

        [UsedImplicitly]
        public static void Postfix(AttunementModal __instance)
        {
            var text = Gui.Format(AttunementModal.AttunementCountFormat,
                __instance.attunedItems.Count.ToString(),
                __instance.AttuningCharacter.RulesetCharacterHero.GetAttunementLimit().ToString());

            __instance.attunementCountLabel.Text = text.Replace("\n", "");
        }

        private static int GetLimit(AttunementModal modal)
        {
            return modal.AttuningCharacter.RulesetCharacterHero.GetAttunementLimit();
        }
    }

    [HarmonyPatch(typeof(AttunementModal), nameof(AttunementModal.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(AttunementModal __instance, RulesetCharacterHero attuningHero)
        {
            var limit = attuningHero.GetAttunementLimit();
            var table = __instance.attunementSlotsTable;

            while (table.childCount < limit)
            {
                Gui.GetPrefabFromPool(__instance.attunementSlotPrefab, table);
            }

            for (var i = 0; i < table.childCount; i++)
            {
                table.GetChild(i).gameObject.SetActive(i < limit);
            }

            var label = __instance.attunementCountLabel;
            var rect = label.GetComponent<RectTransform>();

            label.TMP_Text.alignment = TextAlignmentOptions.Left;

            rect.sizeDelta = new Vector2(300, rect.sizeDelta.y);
            rect.localPosition = new Vector2(10, 225);
        }
    }
}
