using HarmonyLib;
using System.Collections.Generic;

namespace SolastaDungeonMakerPro.Patches.DungeonEditor
{
    //[HarmonyPatch(typeof(UserLocationEditorScreen), "HandleInput")]
    //internal static class UserLocationEditorScreenHandleInput
    //{
    //    public static void Prefix(UserLocationEditorScreen __instance, InputCommands.Id command)
    //    {
    //        if (__instance == null)
    //            return;

    //        var panel = __instance.UserLocationViewPanel;
    //        var location = panel.UserLocation;
    //        var rooms = location.UserRooms;

    //        if (!UserLocationDefinitions.CellsBySize.TryGetValue(location.Size, out var size))
    //        {
    //            Main.Error($"Unknown room size: {location.Size}");
    //            return;
    //        }

    //        // get extents
    //        var minx = rooms.Min(ur => (int?)ur.Position.x) ?? 0;
    //        var maxx = rooms.Max(ur => (int?)(ur.Position.x + ur.OrientedWidth)) ?? 0;
    //        var miny = rooms.Min(ur => (int?)ur.Position.y) ?? 0;
    //        var maxy = rooms.Max(ur => (int?)(ur.Position.y + ur.OrientedHeight)) ?? 0;

    //        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    //        int offset = shiftPressed ? 5 : 1;

    //        switch (command)
    //        {
    //            case InputCommands.Id.SelectCharacter1:
    //                if (maxx + offset <= size)
    //                {
    //                    MoveAll(offset, 0);
    //                }
    //                else
    //                {
    //                    MoveAll(size - maxx, 0);
    //                }
    //                break;

    //            case InputCommands.Id.SelectCharacter2:
    //                if (minx - offset > 0)
    //                {
    //                    MoveAll(-offset, 0);
    //                }
    //                else if (minx > 0)
    //                {
    //                    MoveAll(-minx, 0);
    //                }
    //                break;

    //            case InputCommands.Id.SelectCharacter3:
    //                if (maxy + offset <= size)
    //                {
    //                    MoveAll(0, offset);
    //                }
    //                else if (size - maxy > 0)
    //                {
    //                    MoveAll(0, size - maxy);
    //                }
    //                break;

    //            case InputCommands.Id.SelectCharacter4:
    //                if (miny - offset > 0)
    //                {
    //                    MoveAll(0, -offset);
    //                }
    //                else if (miny > 0)
    //                {
    //                    MoveAll(0, -miny);
    //                }
    //                break;

    //            case InputCommands.Id.RotateCCW:
    //                Rotate(-90f);
    //                break;

    //            case InputCommands.Id.RotateCW:
    //                Rotate(90f);
    //                break;
    //        }

    //        #region Local functions

    //        void Rotate(float rotationAngle)
    //        {
    //            if (rotationAngle == 0f)
    //            {
    //                return;
    //            }

    //            NotifyBeforeModification();

    //            var rotation = Quaternion.Euler(0f, 0f, -rotationAngle);
    //            Main.Log($"angle={rotationAngle}, rotation={rotation}");

    //            var dungeonCenter = new Vector3(size / 2f, size / 2f);
    //            Main.Log($"dungeon center ({dungeonCenter.x}, {dungeonCenter.y})");

    //            foreach (var ur in rooms)
    //            {
    //                Main.Log($"room orientation before ({ur.Orientation}, {ur.OrientedWidth}, {ur.OrientedHeight})");
    //                Main.Log($"current room position ({ur.Position.x}, {ur.Position.y})");

    //                var currentRoomCenter = new Vector3(ur.Position.x + ur.OrientedWidth / 2f, ur.Position.y + ur.OrientedHeight / 2f);
    //                Main.Log($"current room center ({currentRoomCenter.x}, {currentRoomCenter.y})");

    //                var newRoomCenter = rotation * (currentRoomCenter - dungeonCenter) + dungeonCenter;
    //                Main.Log($"new room center ({newRoomCenter.x}, {newRoomCenter.y})");

    //                ur.Rotate(rotationAngle);
    //                Main.Log($"room orientation after {ur.Orientation}, {ur.OrientedWidth}, {ur.OrientedHeight}");

    //                ur.Position = new Vector2Int(Mathf.RoundToInt(newRoomCenter.x - ur.OrientedWidth / 2f), Mathf.RoundToInt(newRoomCenter.y - ur.OrientedHeight / 2f));
    //                Main.Log($"new room position ({ur.Position.x}, {ur.Position.y})");
    //            }

    //            panel.RefreshRooms();
    //        }

    //        void MoveAll(int xOffset, int yOffset)
    //        {
    //            if (xOffset != 0 || yOffset != 0)
    //            {
    //                NotifyBeforeModification();

    //                Main.Log($"xmin={minx}, ymin={miny}, xmax={maxx}, ymax={maxy}, size={size}, xoff={xOffset}, yoff={yOffset}");

    //                foreach (var ur in rooms)
    //                {
    //                    Main.Log($"{ur.Position.x}, {ur.Position.y}");
    //                    ur.Position = new Vector2Int(ur.Position.x + xOffset, ur.Position.y + yOffset);
    //                    Main.Log($"{ur.Position.x}, {ur.Position.y}");
    //                }

    //                panel.RefreshRooms();
    //            }
    //        }

    //        void NotifyBeforeModification()
    //        {
    //            // NOTE: NotifyBeforeModification
    //            // sets anythingModified = true, calls RefreshButtons() and stores current dungeon in undo manager.

    //            var rb = typeof(UserLocationEditorScreen).GetMethod("NotifyBeforeModification", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //            Main.Log($"calling notifyBeforeModification {rb != null}");
    //            rb?.Invoke(__instance, null);
    //        }

    //        #endregion
    //    }
    //}
}