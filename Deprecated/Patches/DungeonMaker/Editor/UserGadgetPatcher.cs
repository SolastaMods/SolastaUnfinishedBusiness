using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Editor;

[HarmonyPatch(typeof(UserGadget), "PostLoadJson")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UserGadget_PostLoadJson
{
    public static MonsterDefinition.DungeonMaker DungeonMakerPresence()
    {
        return MonsterDefinition.DungeonMaker.Monster;
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var dungeonMakerPresenceMethod = typeof(MonsterDefinition).GetMethod("get_DungeonMakerPresence");
        var myDungeonMakerPresenceMethod = typeof(UserGadget_PostLoadJson).GetMethod("DungeonMakerPresence");

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
