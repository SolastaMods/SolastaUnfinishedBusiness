using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SolastaUnfinishedBusiness.CustomUI;

public class CustomDropDown
{
    public delegate void OnValueChanged(TMP_Dropdown.OptionData selected);

    public readonly GuiDropdown DropList;
    public readonly GuiGamepadSelector Selector;

    public OnValueChanged OnValueChangedHandler;

    public int Selected { get; private set; }

    private bool active = true;

    public readonly List<TMP_Dropdown.OptionData> Options = new();

    public CustomDropDown(GuiDropdown dropList, GuiGamepadSelector selector)
    {
        this.DropList = dropList;
        this.Selector = selector;

        ClearOptions();

        dropList.onValueChanged.AddListener(OnDropdownValueChanged);
        selector.SelectionChanged += OnSelectorSelectionChanged;
    }

    public void SetActive(bool value)
    {
        active = value;
        UpdateControls();
    }

    public void UpdateControls()
    {
        var gamepadActive = Gui.GamepadActive;

        DropList.gameObject.SetActive(active && !gamepadActive);
        Selector.gameObject.SetActive(active && gamepadActive);

        if (gamepadActive)
        {
            Selector.OnEnable();
        }
        else
        {
            Selector.OnDisable();
        }
    }

    public void ClearOptions()
    {
        Selected = 0;
        Options.Clear();
        DropList.ClearOptions();
        Selector.Texts = new List<string>();
    }

    public void AddOptions(IEnumerable<TMP_Dropdown.OptionData> values)
    {
        Options.AddRange(values);
        DropList.AddOptions(Options);
        Selector.Texts.AddRange(Options.Select(o => o.text));
        Selector.RefreshCurrent();
    }

    public void SetSelected(int newValue)
    {
        Selected = newValue;
        DropList.SetValueWithoutNotify(newValue);
        Selector.currentSelection = newValue;
        NotifyValueChange();
    }

    private void NotifyValueChange()
    {
        OnValueChangedHandler?.Invoke(Options[Selected]);
    }

    private void OnDropdownValueChanged(int newValue)
    {
        Selected = newValue;
        Selector.currentSelection = newValue;
        NotifyValueChange();
    }

    private void OnSelectorSelectionChanged()
    {
        Selected = Selector.currentSelection;
        DropList.SetValueWithoutNotify(Selected);
        NotifyValueChange();
    }

    internal static GuiDropdown MakeDropdown(string name, Transform transform)
    {
        // ReSharper disable once Unity.UnknownResource
        var gameObject = Object.Instantiate(Resources.Load<GameObject>("GUI/Prefabs/Component/Dropdown"), transform);
        gameObject.name = name;
        return gameObject.GetComponent<GuiDropdown>();
    }


    internal static GuiGamepadSelector MakeSelector(string name, Transform transform)
    {
        // ReSharper disable once Unity.UnknownResource
        var gameObject =
            Object.Instantiate(Resources.Load<GameObject>("Gui/Prefabs/Component/GamepadSelector"), transform);
        gameObject.name = name;
        var component = gameObject.GetComponent<GuiGamepadSelector>();
        component.actionMapName = "ModalListBrowse";
        component.actionName = "GamepadSelector";

        return component;
    }
}
