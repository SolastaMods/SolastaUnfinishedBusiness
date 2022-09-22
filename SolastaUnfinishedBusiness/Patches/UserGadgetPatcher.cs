using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class UserGadgetPatcher
{
    //PATCH: Ensures game doesn't remove `invalid` monsters created with Dungeon Maker Pro (DMP)
    [HarmonyPatch(typeof(UserGadget), "PostLoadJson")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class PostLoadJson_Patch
    {
        private static MonsterDefinition.DungeonMaker DungeonMakerPresence()
        {
            return MonsterDefinition.DungeonMaker.Monster;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var dungeonMakerPresenceMethod = typeof(MonsterDefinition).GetMethod("get_DungeonMakerPresence");
            var myDungeonMakerPresenceMethod = new Func<MonsterDefinition.DungeonMaker>(DungeonMakerPresence).Method;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(dungeonMakerPresenceMethod))
                {
                    yield return new CodeInstruction(OpCodes.Pop); // pop MonsterDefinition instance
                    yield return new CodeInstruction(OpCodes.Call, myDungeonMakerPresenceMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
