using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
#if false
    public enum ClickModifier
    {
        Disabled,
        Shift,
        Ctrl,
        Alt,
        Command
    }
#endif

    private static readonly HashSet<KeyCode> AllowedMouseButtons =
        [KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6];

    private static GUIStyle _hotkeyStyle;

    private static GUIStyle HotkeyStyle
    {
        get
        {
            _hotkeyStyle ??= new GUIStyle(GUI.skin.textArea) { margin = new RectOffset(3, 3, 3, 3), richText = true };

            _hotkeyStyle.fontSize = 11.Point();
            _hotkeyStyle.fixedHeight = 17.Point();

            return _hotkeyStyle;
        }
    }

#if false
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
#endif

    [UsedImplicitly]
    private static bool IsModifier(this KeyCode code)
    {
        return code is KeyCode.LeftControl or KeyCode.RightControl or KeyCode.LeftAlt or KeyCode.RightAlt
            or KeyCode.LeftShift or KeyCode.RightShift or KeyCode.LeftCommand or KeyCode.RightCommand;
    }

    [UsedImplicitly]
    public static bool IsControl(this KeyCode code)
    {
        return code is KeyCode.LeftControl or KeyCode.RightControl;
    }

    [UsedImplicitly]
    public static bool IsAlt(this KeyCode code)
    {
        return code is KeyCode.LeftAlt or KeyCode.RightAlt;
    }

    [UsedImplicitly]
    public static bool IsCommand(this KeyCode code)
    {
        return code is KeyCode.LeftCommand or KeyCode.RightCommand;
    }

    [UsedImplicitly]
    public static bool IsShift(this KeyCode code)
    {
        return code is KeyCode.LeftShift or KeyCode.RightShift;
    }

    [UsedImplicitly]
    [JsonObject(MemberSerialization.OptIn)]
    public class KeyBind(
        string identifier,
        KeyCode key = KeyCode.None,
        bool ctrl = false,
        bool alt = false,
        bool cmd = false,
        bool shift = false)
    {
        [JsonProperty] public readonly bool Cmd = cmd;

        [JsonProperty] public readonly bool Ctrl = ctrl;
        [JsonProperty] public readonly string ID = identifier;

        [JsonProperty] public readonly KeyCode Key = key;

        [JsonProperty] public readonly bool Shift = shift;
        [JsonProperty] public bool Alt = alt;

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

                if (AllowedMouseButtons.Contains(Key))
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

        public bool IsModifierActive => Event.current != null && Input.GetKey(Key);

        public string BindCode => ToString();

        public bool DoesConflict(KeyBind kb)
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
                return ID == kb.ID && DoesConflict(kb);
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
                result += "^".Cyan();
            }

            if (Shift)
            {
                result += "⇑".Cyan();
            }

            if (Alt || Cmd)
            {
                result += "Alt".Cyan();
            }

            return result + (Ctrl || Shift || Alt ? "+".Cyan() : "") + Key;
        }
    }
}
