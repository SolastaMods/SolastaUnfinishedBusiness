using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Helpers;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserGadgetPatcher
{
    [HarmonyPatch(typeof(UserGadget), nameof(UserGadget.PostLoadJson))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PostLoadJson_Patch
    {
        //PATCH: Ensures game doesn't remove `invalid` monsters created with Dungeon Maker Pro (DMP)
        private static MonsterDefinition.DungeonMaker DungeonMakerPresence(MonsterDefinition _)
        {
            return MonsterDefinition.DungeonMaker.Monster;
        }

        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var dungeonMakerPresenceMethod = typeof(MonsterDefinition).GetMethod("get_DungeonMakerPresence");
            var myDungeonMakerPresenceMethod =
                new Func<MonsterDefinition, MonsterDefinition.DungeonMaker>(DungeonMakerPresence).Method;

            return instructions.ReplaceCalls(dungeonMakerPresenceMethod, "UserGadget.PostLoadJson",
                new CodeInstruction(OpCodes.Call, myDungeonMakerPresenceMethod));
        }
    }

    //PATCH: Expands exits and exits multiple sense grids if party greater than 4 (PARTYSIZE)
    [HarmonyPatch(typeof(UserGadget), nameof(UserGadget.ApplySpecialDimensions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ApplySpecialDimensions_Patch
    {
        private static readonly GadgetBlueprint Exit = DatabaseHelper.GetDefinition<GadgetBlueprint>("Exit");

        private static readonly GadgetBlueprint ExitMultiple =
            DatabaseHelper.GetDefinition<GadgetBlueprint>("ExitMultiple");

        [UsedImplicitly]
        public static void Postfix(UserGadget __instance, WorldGadget worldGadget)
        {
            var gadgetBlueprint = __instance.gadgetBlueprint;

            if ((gadgetBlueprint != Exit && gadgetBlueprint != ExitMultiple) ||
                __instance.gadgetBlueprint.CustomizableDimensions ||
                Main.Settings.OverridePartySize <= 4)
            {
                return;
            }

            foreach (var gadgetFlowDescription in worldGadget.EnumerateFlowOfListenerType(
                         "VolumetricTrigger"))
            {
                foreach (var boxCollider in gadgetFlowDescription.BoxColliders
                             .Where(boxCollider => boxCollider))
                {
                    boxCollider.size = new Vector3(2, boxCollider.size.y, 2);
                }
            }
        }
    }
}
