using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit.Utility.Private;

internal static class UI
{
    // Helper functionality

    private static readonly GUIContent Content = new();

    internal static readonly GUIContent CheckOn = new(ModKit.UI.CheckGlyphOn);

    private static readonly GUIContent CheckOff = new(ModKit.UI.CheckGlyphOff);
    private static readonly GUIContent DisclosureOn = new(ModKit.UI.DisclosureGlyphOn);
    private static readonly GUIContent DisclosureOff = new(ModKit.UI.DisclosureGlyphOff);
    private static readonly GUIContent DisclosureEmpty = new(ModKit.UI.DisclosureGlyphEmpty);

    private static readonly int ButtonHint = "MyGUI.Button".GetHashCode();

    private static GUIContent LabelContent(string text)
    {
        Content.text = text;
        Content.image = null;
        Content.tooltip = null;
        return Content;
    }

    [UsedImplicitly]
    public static bool Toggle(
        Rect rect, GUIContent label, bool value, bool isEmpty, GUIContent on, GUIContent off, GUIStyle stateStyle,
        GUIStyle labelStyle)
    {
        var controlID = GUIUtility.GetControlID(ButtonHint, FocusType.Passive, rect);
        var result = false;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (GUI.enabled && rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl = controlID;
                    Event.current.Use();
                }

                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    Event.current.Use();
                }

                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;

                    if (rect.Contains(Event.current.mousePosition))
                    {
                        result = true;
                        Event.current.Use();
                    }
                }

                break;

            case EventType.KeyDown:
                if (GUIUtility.hotControl == controlID)
                {
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                }

                break;

            case EventType.Repaint:
            {
                //bool leftAlign = stateStyle.alignment == TextAnchor.MiddleLeft
                //                || stateStyle.alignment == TextAnchor.UpperLeft
                //                || stateStyle.alignment == TextAnchor.LowerLeft
                //                ;
                var rightAlign =
                        stateStyle.alignment is TextAnchor.MiddleRight or TextAnchor.UpperRight or TextAnchor.LowerRight
                    ;
                // stateStyle.alignment determines position of state element
                var state = isEmpty ? DisclosureEmpty : value ? on : off;
                var stateSize =
                    stateStyle.CalcSize(value
                        ? on
                        : off); // don't use the empty content to calculate size so titles line up in lists
                var x = rightAlign ? rect.xMax - stateSize.x : rect.x;
                Rect stateRect = new(x, rect.y, stateSize.x, stateSize.y);

                // layout state before or after following alignment
                var labelSize = labelStyle.CalcSize(label);
                x = rightAlign ? stateRect.x - stateSize.x - 5 : stateRect.xMax + 5;
                Rect labelRect = new(x, rect.y, labelSize.x, labelSize.y);

                stateStyle.Draw(stateRect, state, controlID);
                labelStyle.Draw(labelRect, label, controlID);
            }
                break;
        }

        return result;
    }

    // Button Control - Layout Version

    [UsedImplicitly]
    public static bool Toggle(GUIContent label, bool value, GUIContent on, GUIContent off, GUIStyle stateStyle,
        GUIStyle labelStyle, bool isEmpty = false, params GUILayoutOption[] options)
    {
        var state = value ? on : off;
        var sStyle = new GUIStyle(stateStyle);
        var lStyle = new GUIStyle(labelStyle) { wordWrap = false };
        var stateSize = sStyle.CalcSize(state);
        lStyle.fixedHeight = stateSize.y - 2;
        var padding = new RectOffset(0, (int)stateSize.x + 5, 0, 0);
        lStyle.padding = padding;

        var rect = GUILayoutUtility.GetRect(label, lStyle, options);

        return Toggle(rect, label, value, isEmpty, on, off, stateStyle, labelStyle);
    }

    [UsedImplicitly]
    public static bool Toggle(string label, bool value, string on, string off, GUIStyle stateStyle, GUIStyle labelStyle,
        params GUILayoutOption[] options)
    {
        return Toggle(LabelContent(label), value, new GUIContent(on), new GUIContent(off), stateStyle, labelStyle,
            false, options);
    }

    // Disclosure Toggles
    [UsedImplicitly]
    public static bool DisclosureToggle(GUIContent label, bool value, bool isEmpty = false,
        params GUILayoutOption[] options)
    {
        return Toggle(label, value, DisclosureOn, DisclosureOff, GUI.skin.textArea, GUI.skin.label, isEmpty,
            options);
    }

    [UsedImplicitly]
    public static bool DisclosureToggle(string label, bool value, GUIStyle stateStyle, GUIStyle labelStyle,
        bool isEmpty = false, params GUILayoutOption[] options)
    {
        return Toggle(LabelContent(label), value, DisclosureOn, DisclosureOff, stateStyle, labelStyle, isEmpty,
            options);
    }

    [UsedImplicitly]
    public static bool DisclosureToggle(string label, bool value, bool isEmpty = false,
        params GUILayoutOption[] options)
    {
        return DisclosureToggle(label, value, GUI.skin.box, GUI.skin.label, isEmpty, options);
    }

    // CheckBox 
    [UsedImplicitly]
    public static bool CheckBox(GUIContent label, bool value, bool isEmpty, params GUILayoutOption[] options)
    {
        return Toggle(label, value, CheckOn, CheckOff, GUI.skin.textArea, GUI.skin.label, isEmpty, options);
    }

    [UsedImplicitly]
    public static bool CheckBox(string label, bool value, bool isEmpty, GUIStyle style,
        params GUILayoutOption[] options)
    {
        return Toggle(LabelContent(label), value, CheckOn, CheckOff, GUI.skin.box, style, isEmpty, options);
    }

    [UsedImplicitly]
    public static bool CheckBox(string label, bool value, bool isEmpty, params GUILayoutOption[] options)
    {
        return CheckBox(label, value, isEmpty, GUI.skin.label, options);
    }
}
