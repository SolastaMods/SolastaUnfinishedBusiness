using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches.DungeonMaker;

//PATCH: changes how the location / rooms are instantiated
[HarmonyPatch(typeof(WorldLocation), "BuildFromUserLocation")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class WorldLocation_BuildFromUserLocation
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = 0;
        var roomTransformPos = Main.IsDebugBuild ? 8 : 4;
        var userRoomPos = Main.IsDebugBuild ? 4 : 2;
        var setLocalPositionMethod = typeof(Transform).GetMethod("set_localPosition");
        var getTemplateVegetationMaskAreaMethod =
            typeof(DmProRendererContext).GetMethod("GetTemplateVegetationMaskArea");
        var setupLocationTerrainMethod = typeof(DmProRendererContext).GetMethod("SetupLocationTerrain");
        var setupFlatRoomsMethod = typeof(DmProRendererContext).GetMethod("SetupFlatRooms");
        var addVegetationMaskAreaMethod = typeof(DmProRendererContext).GetMethod("AddVegetationMaskArea");
        var fixFlatRoomReflectionProbeMethod =
            typeof(DmProRendererContext).GetMethod("FixFlatRoomReflectionProbe");

        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Call, getTemplateVegetationMaskAreaMethod);

        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Ldarg_1);
        yield return new CodeInstruction(OpCodes.Call, setupLocationTerrainMethod);

        foreach (var instruction in instructions)
        {
            if (instruction.Calls(setLocalPositionMethod) && ++found == 1)
            {
                yield return new CodeInstruction(OpCodes.Ldloc_S, roomTransformPos);
                yield return new CodeInstruction(OpCodes.Ldloc_S, userRoomPos);
                yield return new CodeInstruction(OpCodes.Call, addVegetationMaskAreaMethod);

                yield return instruction;

                yield return new CodeInstruction(OpCodes.Ldloc_S, roomTransformPos);
                yield return new CodeInstruction(OpCodes.Ldloc_S, userRoomPos);
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
