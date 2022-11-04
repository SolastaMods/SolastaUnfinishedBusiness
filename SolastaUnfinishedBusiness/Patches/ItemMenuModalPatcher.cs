using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

public static class ItemMenuModalPatcher
{
    [HarmonyPatch(typeof(ItemMenuModal), "SetupFromItem")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class SetupFromItem_Patch
    {
        //PATCH: allows mark deity to work with MC heroes (Multiclass)
        private static bool RequiresDeity(ItemMenuModal itemMenuModal)
        {
            return itemMenuModal.GuiCharacter.RulesetCharacterHero.ClassesHistory.Exists(x => x.RequiresDeity);
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var requiresDeityMethod = typeof(CharacterClassDefinition).GetMethod("get_RequiresDeity");
            var myRequiresDeityMethod = new Func<ItemMenuModal, bool>(RequiresDeity).Method;

            return instructions.ReplaceAllCalls(requiresDeityMethod,
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myRequiresDeityMethod));
        }
    }
}
