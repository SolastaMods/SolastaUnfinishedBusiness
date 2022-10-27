using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

public static class AttunementModalPatcher
{
    [HarmonyPatch(typeof(AttunementModal), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Refresh_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var method = new Func<AttunementModal, int>(GetLimit).Method;

            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4_3)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, method);
                }
                else
                {
                    yield return instruction;
                }
            }
        }

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

    [HarmonyPatch(typeof(AttunementModal), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Bind_Patch
    {
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
