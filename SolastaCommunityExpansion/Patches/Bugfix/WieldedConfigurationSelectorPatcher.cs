using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.BugFix;

/// <summary>
///     Issue: WieldedConfigurationSelector.Bind passes character=null to mainHandSlotBox.Bind and offHandSlotBox.Bind
///     Not fixed as of 1.3.40.
/// </summary>
[HarmonyPatch(typeof(WieldedConfigurationSelector), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class WieldedConfigurationSelector_Bind
{
    public static void MyBind(
        InventorySlotBox inventorySlotBox,
        RulesetInventorySlot rulesetInventorySlot,
        GuiCharacter guiCharacter,
        bool inMainHud,
        RectTransform anchor,
        TooltipDefinitions.AnchorMode anchorMode,
        bool refreshDirectly,
        GuiCharacter wieldedConfigurationSelectorGuiCharacter) // OpCodes.Ldarg_1 below...
    {
        inventorySlotBox.Bind(rulesetInventorySlot, guiCharacter ?? wieldedConfigurationSelectorGuiCharacter,
            inMainHud, anchor, anchorMode, refreshDirectly);
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        //
        // BUGFIX: wielded configuration selection
        //

        var bindMethod = typeof(InventorySlotBox).GetMethod("Bind");
        var myBindMethod = typeof(WieldedConfigurationSelector_Bind).GetMethod("MyBind");

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
