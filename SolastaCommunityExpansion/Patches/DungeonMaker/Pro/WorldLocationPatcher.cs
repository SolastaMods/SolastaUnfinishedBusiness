using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    // changes how the location / rooms are instantiated
    [HarmonyPatch(typeof(WorldLocation), "BuildFromUserLocation")]
    internal static class WorldLocationBuildFromUserLocation
    {
        //
        // IMPORTANT: this transpiler only works in BETA. we need to change lines 38-39 and 44-45 from 8,4 to 4,2 in PRODUCTION RELEASE
        //
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var found = 0;
            var setLocalPositionMethod = typeof(Transform).GetMethod("set_localPosition");
            var getTemplateVegetationMaskAreaMethod = typeof(Models.DmProRendererContext).GetMethod("GetTemplateVegetationMaskArea");
            var setupLocationTerrainMethod = typeof(Models.DmProRendererContext).GetMethod("SetupLocationTerrain");
            var setupFlatRoomsMethod = typeof(Models.DmProRendererContext).GetMethod("SetupFlatRooms");
            var addVegetationMaskAreaMethod = typeof(Models.DmProRendererContext).GetMethod("AddVegetationMaskArea");
            var fixFlatRoomReflectionProbeMethod = typeof(Models.DmProRendererContext).GetMethod("FixFlatRoomReflectionProbe");

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, getTemplateVegetationMaskAreaMethod);

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, setupLocationTerrainMethod);

            foreach (var instruction in instructions)
            {
                if (!Main.Settings.EnableDungeonMakerPro)
                {
                    yield return instruction;
                }
                else if (instruction.Calls(setLocalPositionMethod) && ++found == 1)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8); // roomTransform 4
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4); // userRoom 2
                    yield return new CodeInstruction(OpCodes.Call, addVegetationMaskAreaMethod);

                    yield return instruction;

                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8); // roomTransform 4
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4); // userRoom 2
                    yield return new CodeInstruction(OpCodes.Call, setupFlatRoomsMethod);
                }
                else if (instruction.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, fixFlatRoomReflectionProbeMethod);

                    yield return instruction;
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
