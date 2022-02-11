using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    // changes how the location / rooms are instantiated
    [HarmonyPatch(typeof(WorldLocation), "BuildFromUserLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class WorldLocation_BuildFromUserLocation
    {
        //
        // IMPORTANT: this transpiler only works in BETA. we need to change lines 38-39 and 44-45 from 8,4 to 4,2 in PRODUCTION RELEASE
        //
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (!Main.Settings.EnableMulticlass)
            {
                foreach (var instruction in instructions)
                {
                    yield return instruction;
                }

                yield break;
            }

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
                if (instruction.Calls(setLocalPositionMethod) && ++found == 1)
                {
                    //
                    // WARNING: review parameters values 4 and 2 before release
                    //
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 8); // roomTransform 4
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4); // userRoom 2
                    yield return new CodeInstruction(OpCodes.Call, addVegetationMaskAreaMethod);

                    yield return instruction;

                    //
                    // WARNING: review parameters values 4 and 2 before release
                    //
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
