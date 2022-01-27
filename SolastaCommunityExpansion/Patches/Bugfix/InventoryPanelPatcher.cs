using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using TA;

namespace SolastaCommunityExpansion.Patches.Bugfix
{
    [HarmonyPatch(typeof(InventoryPanel), "OnPointerDown")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryPanel_OnPointerDown
    {
        public static int3 MyLocationPosition(GameLocationCharacter gameLocationCharacter)
        {
            return gameLocationCharacter?.LocationPosition ?? int3.zero;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var locationPositionGetter = typeof(GameLocationCharacter).GetMethod("get_LocationPosition");
            var myLocationPositionMethod = typeof(InventoryPanel_OnPointerDown).GetMethod("MyLocationPosition");

            foreach (CodeInstruction instruction in instructions)
            {
                if (Main.Settings.BugFixMainScreenInventoryPanelGround && instruction.Calls(locationPositionGetter))
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

    [HarmonyPatch(typeof(InventoryPanel), "EndInteraction")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryPanel_EndInteraction
    {
        public static int3 MyLocationPosition(GameLocationCharacter gameLocationCharacter)
        {
            return gameLocationCharacter?.LocationPosition ?? int3.zero;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var locationPositionGetter = typeof(GameLocationCharacter).GetMethod("get_LocationPosition");
            var myLocationPositionMethod = typeof(InventoryPanel_EndInteraction).GetMethod("MyLocationPosition");

            foreach (CodeInstruction instruction in instructions)
            {
                if (Main.Settings.BugFixMainScreenInventoryPanelGround && instruction.Calls(locationPositionGetter))
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

    [HarmonyPatch(typeof(InventoryPanel), "OnItemDroppedOnGround")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class InventoryPanel_OnItemDroppedOnGround
    {
        public static int3 MyLocationPosition(GameLocationCharacter gameLocationCharacter)
        {
            return gameLocationCharacter?.LocationPosition ?? int3.zero;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var locationPositionGetter = typeof(GameLocationCharacter).GetMethod("get_LocationPosition");
            var myLocationPositionMethod = typeof(InventoryPanel_OnItemDroppedOnGround).GetMethod("MyLocationPosition");

            foreach (CodeInstruction instruction in instructions)
            {
                if (Main.Settings.BugFixMainScreenInventoryPanelGround && instruction.Calls(locationPositionGetter))
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
