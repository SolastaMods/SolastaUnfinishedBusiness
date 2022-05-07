#if false
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Editor
{
    [HarmonyPatch(typeof(UserLocationEditorScreen), "HandleInput")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UserLocationEditorScreen_HandleInput
    {
        public static void Postfix(UserLocationEditorScreen __instance, SelectedGadgetPanel ___selectedGadgetPanel, InputCommands.Id command)
        {
            if (!Main.Settings.EnableDungeonMakerRotationHotkeys || __instance == null)
            {
                return;
            }

            var panel = __instance.UserLocationViewPanel;
            var location = panel.UserLocation;
            var rooms = location.UserRooms;

            if (!UserLocationDefinitions.CellsBySize.TryGetValue(location.Size, out var size))
            {
                Main.Error($"Unknown room size: {location.Size}");
                return;
            }

            switch (command)
            {
                case InputCommands.Id.RotateCCW:
                    // NOTE: don't use ___selectedGadgetPanel?. which bypasses Unity object lifetime check
                    if (___selectedGadgetPanel && !___selectedGadgetPanel.IsTextFieldFocused())
                    {
                        Rotate(-90f);
                    }

                    break;

                case InputCommands.Id.RotateCW:
                    // NOTE: don't use ___selectedGadgetPanel?. which bypasses Unity object lifetime check
                    if (___selectedGadgetPanel && !___selectedGadgetPanel.IsTextFieldFocused())
                    {
                        Rotate(90f);
                    }

                    break;
            }

            #region Local functions
            void Rotate(float rotationAngle)
            {
                if (rotationAngle == 0f)
                {
                    return;
                }

                NotifyBeforeModification();

                var rotation = Quaternion.Euler(0f, 0f, -rotationAngle);
                Main.Log($"angle={rotationAngle}, rotation={rotation}");

                var dungeonCenter = new Vector3(size / 2f, size / 2f);
                Main.Log($"dungeon center ({dungeonCenter.x}, {dungeonCenter.y})");

                foreach (var ur in rooms)
                {
                    Main.Log($"room orientation before ({ur.Orientation}, {ur.OrientedWidth}, {ur.OrientedHeight})");
                    Main.Log($"current room position ({ur.Position.x}, {ur.Position.y})");

                    var currentRoomCenter = new Vector3(ur.Position.x + (ur.OrientedWidth / 2f), ur.Position.y + (ur.OrientedHeight / 2f));
                    Main.Log($"current room center ({currentRoomCenter.x}, {currentRoomCenter.y})");

                    var newRoomCenter = (rotation * (currentRoomCenter - dungeonCenter)) + dungeonCenter;
                    Main.Log($"new room center ({newRoomCenter.x}, {newRoomCenter.y})");

                    ur.Rotate(rotationAngle);
                    Main.Log($"room orientation after {ur.Orientation}, {ur.OrientedWidth}, {ur.OrientedHeight}");

                    ur.Position = new Vector2Int(Mathf.RoundToInt(newRoomCenter.x - (ur.OrientedWidth / 2f)), Mathf.RoundToInt(newRoomCenter.y - (ur.OrientedHeight / 2f)));
                    Main.Log($"new room position ({ur.Position.x}, {ur.Position.y})");
                }

                panel.RefreshRooms();
            }

            void NotifyBeforeModification()
            {
                // NOTE: NotifyBeforeModification
                // sets anythingModified = true, calls RefreshButtons() and stores current dungeon in undo manager

#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
                var rb = typeof(UserLocationEditorScreen).GetMethod("NotifyBeforeModification", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

                rb?.Invoke(__instance, null);
            }
            #endregion
        }
    }
}
#endif
