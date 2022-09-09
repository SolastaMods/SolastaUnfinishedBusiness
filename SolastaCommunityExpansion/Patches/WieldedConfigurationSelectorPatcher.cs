using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches;

internal static class WieldedConfigurationSelectorPatcher
{
    //PATCH: WieldedConfigurationSelector.Bind passes wieldedConfigurationSelectorGuiCharacter to mainHandSlotBox.Bind and offHandSlotBox.Bind
    [HarmonyPatch(typeof(WieldedConfigurationSelector), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class Bind_Patch
    {
        public static void MyBind(
            [NotNull] InventorySlotBox inventorySlotBox,
            RulesetInventorySlot rulesetInventorySlot,
            [CanBeNull] GuiCharacter guiCharacter,
            bool inMainHud,
            RectTransform anchor,
            TooltipDefinitions.AnchorMode anchorMode,
            bool refreshDirectly,
            GuiCharacter wieldedConfigurationSelectorGuiCharacter) // OpCodes.Ldarg_1 below...
        {
            inventorySlotBox.Bind(rulesetInventorySlot, guiCharacter ?? wieldedConfigurationSelectorGuiCharacter,
                inMainHud, anchor, anchorMode, refreshDirectly);
        }

        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var bindMethod = typeof(InventorySlotBox).GetMethod("Bind");
            var myBindMethod = typeof(Bind_Patch).GetMethod("MyBind");

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(bindMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // GuiCharacter argument
                    yield return new CodeInstruction(OpCodes.Call, myBindMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
