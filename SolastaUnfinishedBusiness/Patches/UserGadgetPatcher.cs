using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Helpers;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserGadgetPatcher
{
    [HarmonyPatch(typeof(UserGadget), "PostLoadJson")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class PostLoadJson_Patch
    {
        //PATCH: Ensures game doesn't remove `invalid` monsters created with Dungeon Maker Pro (DMP)
        private static MonsterDefinition.DungeonMaker DungeonMakerPresence(MonsterDefinition _)
        {
            return MonsterDefinition.DungeonMaker.Monster;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var dungeonMakerPresenceMethod = typeof(MonsterDefinition).GetMethod("get_DungeonMakerPresence");
            var myDungeonMakerPresenceMethod =
                new Func<MonsterDefinition, MonsterDefinition.DungeonMaker>(DungeonMakerPresence).Method;

            return instructions.ReplaceCalls(dungeonMakerPresenceMethod,
                new CodeInstruction(OpCodes.Call, myDungeonMakerPresenceMethod));
        }
    }
}
