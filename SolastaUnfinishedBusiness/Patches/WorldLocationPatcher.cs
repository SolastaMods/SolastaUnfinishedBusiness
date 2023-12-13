using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class WorldLocationPatcher
{
    //PATCH: changes how the location / rooms are instantiated (DMP)
    [HarmonyPatch(typeof(WorldLocation), nameof(WorldLocation.BuildFromUserLocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildFromUserLocation_Patch
    {
        [UsedImplicitly]
        public static void Prefix(WorldLocation __instance, UserLocation userLocation)
        {
            DmProRendererContext.GetTemplateVegetationMaskArea(__instance);
            DmProRendererContext.SetupLocationTerrain(__instance, userLocation);
        }

        [UsedImplicitly]
        public static void Postfix(WorldLocation __instance)
        {
            DmProRendererContext.FixFlatRoomReflectionProbe(__instance);
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var setLocalPositionMethod = typeof(Transform).GetMethod("set_localPosition");
            var setupFlatRoomsMethod = new Action<Transform, UserRoom>(DmProRendererContext.SetupFlatRooms).Method;
            var addVegetationMaskAreaMethod =
                new Action<Transform, UserRoom>(DmProRendererContext.AddVegetationMaskArea).Method;

            return instructions.ReplaceCall(setLocalPositionMethod,
                1,
                "WorldLocationPatcher.BindFromUserLocation",
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Ldloc_S, 2),
                new CodeInstruction(OpCodes.Call, addVegetationMaskAreaMethod),
                new CodeInstruction(OpCodes.Call, setLocalPositionMethod), // checked for Call vs CallVirtual
                new CodeInstruction(OpCodes.Ldloc_S, 4),
                new CodeInstruction(OpCodes.Ldloc_S, 2),
                new CodeInstruction(OpCodes.Call, setupFlatRoomsMethod));
        }
    }
}
