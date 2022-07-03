using UnityEngine;

namespace SolastaCommunityExpansion.Api.ModKit.Private;

public static class UI
{
    private static readonly GUIContent _LabelContent = new();
    public static readonly GUIContent CheckOn = new(global::ModKit.UI.CheckGlyphOn);
    public static readonly GUIContent CheckOff = new(global::ModKit.UI.CheckGlyphOff);
    public static readonly GUIContent DisclosureOn = new(global::ModKit.UI.DisclosureGlyphOn);
    public static readonly GUIContent DisclosureOff = new(global::ModKit.UI.DisclosureGlyphOff);
    public static readonly GUIContent DisclosureEmpty = new(global::ModKit.UI.DisclosureGlyphEmpty);

    private static readonly int SButtonHint = "MyGUI.Button".GetHashCode();

    private static GUIContent LabelContent(string text)
    {
        _LabelContent.text = text;
        _LabelContent.image = null;
        _LabelContent.tooltip = null;
        return _LabelContent;
    }

    private static bool Toggle(Rect rect, GUIContent label, bool value, bool isEmpty, GUIContent on, GUIContent off,
        GUIStyle stateStyle, GUIStyle labelStyle)
    {
        var controlID = GUIUtility.GetControlID(SButtonHint, FocusType.Passive, rect);
        var result = false;

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

    // Button Control
    private static bool Toggle(GUIContent label, bool value, GUIContent on, GUIContent off, GUIStyle stateStyle,
        GUIStyle labelStyle, bool isEmpty = false, params GUILayoutOption[] options)
    {
        var state = value ? on : off;
        var sStyle = new GUIStyle(stateStyle);
        var lStyle = new GUIStyle(labelStyle) {wordWrap = false};
        var stateSize = sStyle.CalcSize(state);
        lStyle.fixedHeight = stateSize.y - 2;
        var padding = new RectOffset(0, (int)stateSize.x + 5, 0, 0);
        lStyle.padding = padding;
        var rect = GUILayoutUtility.GetRect(label, lStyle, options);

        return Toggle(rect, label, value, isEmpty, on, off, stateStyle, labelStyle);
    }

    // Disclosure Toggles
    private static bool DisclosureToggle(string label, bool value, GUIStyle stateStyle, GUIStyle labelStyle,
        bool isEmpty = false, params GUILayoutOption[] options)
    {
        return Toggle(LabelContent(label), value, DisclosureOn, DisclosureOff, stateStyle, labelStyle, isEmpty,
            options);
    }

    public static bool DisclosureToggle(string label, bool value, bool isEmpty = false,
        params GUILayoutOption[] options)
    {
        return DisclosureToggle(label, value, GUI.skin.box, GUI.skin.label, isEmpty, options);
    }

    // CheckBox
    public static bool CheckBox(string label, bool value, bool isEmpty, GUIStyle style,
        params GUILayoutOption[] options)
    {
        return Toggle(LabelContent(label), value, CheckOn, CheckOff, GUI.skin.box, style, isEmpty, options);
    }

    public static bool CheckBox(string label, bool value, bool isEmpty, params GUILayoutOption[] options)
    {
        return CheckBox(label, value, isEmpty, GUI.skin.label, options);
    }
}
