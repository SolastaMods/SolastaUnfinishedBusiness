using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    public enum ClickModifier
    {
        Disabled,
        Shift,
        Ctrl,
        Alt,
        Command
    }

    private static readonly HashSet<KeyCode> allowedMouseButtons =
        new() { KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6 };

    private static GUIStyle _hotkeyStyle;

    public static GUIStyle hotkeyStyle
    {
        get
        {
            if (_hotkeyStyle == null)
            {
                _hotkeyStyle = new GUIStyle(GUI.skin.textArea) { margin = new RectOffset(3, 3, 3, 3), richText = true };
            }

            _hotkeyStyle.fontSize = 11.point();
            _hotkeyStyle.fixedHeight = 17.point();

            return _hotkeyStyle;
        }
    }

    public static bool IsActive(this ClickModifier modifier)
    {
        return modifier switch
        {
            ClickModifier.Disabled => false,
            ClickModifier.Shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
            ClickModifier.Ctrl => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl),
            ClickModifier.Alt => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt),
            ClickModifier.Command => Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand),
            _ => false
        };
    }

    public static bool IsModifier(this KeyCode code)
    {
        return code == KeyCode.LeftControl || code == KeyCode.RightControl
                                           || code == KeyCode.LeftAlt || code == KeyCode.RightAlt
                                           || code == KeyCode.LeftShift || code == KeyCode.RightShift
                                           || code == KeyCode.LeftCommand || code == KeyCode.RightCommand;
    }

    public static bool IsControl(this KeyCode code)
    {
        return code == KeyCode.LeftControl || code == KeyCode.RightControl;
    }

    public static bool IsAlt(this KeyCode code)
    {
        return code == KeyCode.LeftAlt || code == KeyCode.RightAlt;
    }

    public static bool IsCommand(this KeyCode code)
    {
        return code == KeyCode.LeftCommand || code == KeyCode.RightCommand;
    }

    public static bool IsShift(this KeyCode code)
    {
        return code == KeyCode.LeftShift || code == KeyCode.RightShift;
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class KeyBind
    {
        [JsonProperty] public bool Alt;

        [JsonProperty] public bool Cmd;

        [JsonProperty] public bool Ctrl;

        [JsonProperty] public string ID;

        [JsonProperty] public KeyCode Key;

        [JsonProperty] public bool Shift;

        public KeyBind(string identifer, KeyCode key = KeyCode.None, bool ctrl = false, bool alt = false,
            bool cmd = false, bool shift = false)
        {
            ID = identifer;
            Key = key;
            Ctrl = ctrl;
            Alt = alt;
            Cmd = cmd;
            Shift = shift;
        }

        [JsonIgnore] public bool IsEmpty => Key == KeyCode.None;

        [JsonIgnore]
        public bool IsKeyCodeActive
        {
            get
            {
                if (Key == KeyCode.None)
                {
                    return false;
                }

                if (allowedMouseButtons.Contains(Key))
                {
                    return Input.GetKey(Key);
                }

                var active = Key == Event.current.keyCode;
                return active;
            }
        }

        [JsonIgnore]
        public bool IsActive
        {
            get
            {
                if (Event.current == null)
                {
                    return false;
                }

                if (!IsKeyCodeActive)
                {
                    return false;
                }

                var ctrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                var altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
                var cmdDown = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
                var shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                // note we already checked Key above
                var active = ctrlDown == Ctrl
                             && altDown == Alt
                             && cmdDown == Cmd
                             && shiftDown == Shift;
                return active;
            }
        }

        public bool IsModifierActive
        {
            get
            {
                if (Event.current == null)
                {
                    return false;
                }

                return Input.GetKey(Key);
            }
        }

        public string bindCode => ToString();

        public bool Conflicts(KeyBind kb)
        {
            return Key == kb.Key
                   && Ctrl == kb.Ctrl
                   && Alt == kb.Alt
                   && Cmd == kb.Cmd
                   && Shift == kb.Shift;
        }

        public override bool Equals(object o)
        {
            if (o is KeyBind kb)
            {
                return ID == kb.ID && Conflicts(kb);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode()
                   + (int)Key
                   + (Ctrl ? 1 : 0)
                   + (Cmd ? 1 : 0)
                   + (Shift ? 1 : 0);
        }

        public override string ToString()
        {
            // Why can't Unity display these ⌥⌃⇧⌘ ???  ⌗⌃⌥⇧⇑⌂©ăåâÂ
            var result = "";
            if (Ctrl)
            {
                result += "^".cyan();
            }

            if (Shift)
            {
                result += "⇑".cyan();
            }

            if (Alt || Cmd)
            {
                result += "Alt".cyan();
            }

            return result + (Ctrl || Shift || Alt ? "+".cyan() : "") + Key;
        }
    }
}
