using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches
{
    [HarmonyPatch(typeof(InventoryPanel), "EndInteraction")]
    internal static class InventoryPanel_EndInteraction
    {
        internal static TA.int3 MyLocationPosition(GameLocationCharacter gameLocationCharacter)
        {
            return gameLocationCharacter == null ? TA.int3.zero : gameLocationCharacter.LocationPosition;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var get_LocationPositionMethod = typeof(GameLocationCharacter).GetMethod("get_LocationPosition");
            var myLocationPositionMethod = typeof(InventoryPanel_EndInteraction).GetMethod("MyLocationPosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(get_LocationPositionMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, myLocationPositionMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
