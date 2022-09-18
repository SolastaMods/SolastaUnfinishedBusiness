using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GameObject;

namespace SolastaUnfinishedBusiness.CustomUI;

public class SubFeatSelectionModal : GuiGameScreen
{
    private static SubFeatSelectionModal instance;

    private bool localInitialized;
    private Button buton;
    private Image image;
    private ProficiencyBaseItem.OnItemClickedHandler itemClickHandler;

    public static SubFeatSelectionModal Get()
    {
        return instance ??= new GameObject().AddComponent<SubFeatSelectionModal>();
    }

    public void Bind(RulesetCharacterHero inspectedCharacter,
        FeatItem baseItem,
        IGroupedFeat group,
        ProficiencyBaseItem.OnItemClickedHandler onItemClicked,
        RectTransform attachment)
    {
        Initialize();
        LocalInit();

        Visible = true;
        itemClickHandler = onItemClicked;
        var subFeats = group.GetSubFeats();
        Main.Log2($"SFSM [{inspectedCharacter.Name}] Bind subFeats: {subFeats.Count}");

        var corners = new Vector3[4];
        attachment.GetWorldCorners(corners);
        var step = corners[0].y - corners[1].y - 4f;
        var position = attachment.position + new Vector3(0, step, 0);

        var featPrefab = Resources.Load<GameObject>("Gui/Prefabs/CharacterInspection/Proficiencies/FeatItem");

        var i = 0;
        foreach (var subFeat in subFeats)
        {
            var qwe = Gui.GetPrefabFromPool(featPrefab, transform);
            InitFeatItem(inspectedCharacter, baseItem, subFeat, qwe.GetComponent<FeatItem>());
            qwe.GetComponent<RectTransform>().position = position + new Vector3(0, step * i, 0);
            i++;
        }

        Visible = false;
    }
    
    private void Unbind()
    {
        itemClickHandler = null;
        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<FeatItem>().Unbind();
        }

        Gui.ReleaseChildrenToPool(transform);
    }

    public override void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        base.Initialize();

        name = "SubFeatSelector";

        //register this screen
        var guiMgr = Gui.GuiService as GuiManager;
        if (guiMgr != null)
        {
            guiMgr.screensByType.Add(GetType(), this);
        }

        //mark it as loaded and hope we don't need to actually call `Load`, because that one is async
        loaded = true;
    }

    public override void Awake()
    {
        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<GuiTooltip>();
        gameObject.AddComponent<CanvasGroup>();
        image = gameObject.AddComponent<Image>();
        buton = gameObject.AddComponent<Button>();

        base.Awake();
    }

    private void LocalInit()
    {
        if (localInitialized)
        {
            return;
        }

        var levelup = Gui.GuiService.GetScreen<CharacterLevelUpScreen>();

        //set sort index to just above levelup screen so it has input handling priority
        SortIndex = levelup.SortIndex + 1;

        //put it visually just above levelup screen
        transform.parent = Find("Application/GUI/BackgroundCanvas/ForegroundCanvas").transform;
        var levelupIndex = levelup.transform.GetSiblingIndex();
        transform.SetSiblingIndex(levelupIndex + 1);

        RectTransform.sizeDelta = new Vector2(200, 200);
        RectTransform.anchorMin = new Vector2(0, 0);
        RectTransform.anchorMax = new Vector2(1, 1);
        RectTransform.pivot = new Vector2(0f, 0f);
        RectTransform.position = new Vector3(0, 0, 0);

        image.sprite = Gui.GuiService.GetScreen<BlackScreen>().GetComponent<Image>().sprite;
        image.color = new Color(0, 0, 0, 0.85f);
        image.alphaHitTestMinimumThreshold = 0;

        buton.onClick.AddListener(OnCloseCb);

        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;

        GuiTooltip.content = string.Empty;
        GuiTooltip.Disabled = false;
        GuiTooltip.tooltipClass = string.Empty;

        localInitialized = true;
    }

    private void InitFeatItem(RulesetCharacterHero inspectedCharacter, FeatItem baseItem, FeatDefinition featDefinition,
        FeatItem component)
    {
        Main.Log2($"SFSM [{inspectedCharacter.Name}] Bind feat <{featDefinition.Name}>");
        component.Bind(inspectedCharacter, featDefinition, OnItemSelected, null, true);
        component.StageTag = baseItem.StageTag;
        component.PreviousStageTag = baseItem.PreviousStageTag;
        component.gameObject.SetActive(true);
        component.Refresh(ProficiencyBaseItem.InteractiveMode.InteractiveSingle, HeroDefinitions.PointsPoolType.Feat,
            ProficiencyBaseItem.DisabledMode.Default);

        component.Tooltip.Anchor = baseItem.Tooltip.Anchor;
        component.Tooltip.AnchorMode = baseItem.Tooltip.AnchorMode;

        component.RectTransform.sizeDelta = new Vector2(300, 44);
    }

    private void OnItemSelected(ProficiencyBaseItem item)
    {
        var handler = itemClickHandler;
        itemClickHandler = null;
        handler?.Invoke(item);
        Hide();
    }
    
    public void OnCloseCb()
    {
        //TODO: implement cancel callback
        // SubpowerSelectionModal.SubpowerCanceledHandler subpowerCanceled = this.subpowerCanceled;
        // if (subpowerCanceled != null)
        //     subpowerCanceled();
        Hide();
    }
    
    public override void OnBeginShow(bool instant = false)
    {
        base.OnBeginShow(instant);
        // this.mainPanel.Show(instant);
        if (Gui.GamepadActive)
            Gui.InputService.ClearCurrentSelectable();
        ServiceRepository.GetService<ITooltipService>().HideTooltip();
    }

    public override void OnBeginHide(bool instant = false)
    {
        Unbind();
        base.OnBeginHide(instant);
    }

    public override bool HandleInput(InputCommands.Id command)
    {
        if (command == InputCommands.Id.Cancel)
            OnCloseCb();
        return true;
    }
    
    public void CancelPerformed(InputAction.CallbackContext context)
    {
        if (this == null)
            return;
        OnCloseCb();
    }

    //TODO: handle controller
    
    public override bool GrabsCameraControl => true;

    public override string ActionMap => "UI";

    public override bool AutoRecomputeNavigation => true;
    public override void SelectDefaultControl()
    {
        //TODO: implement default control selection
        // if (!Gui.GamepadActive || !((UnityEngine.Object) Gui.InputService.CurrentSelectedGameObject == (UnityEngine.Object) null))
        //   return;
        // for (int index = 0; index < this.subpowersTable.childCount; ++index)
        // {
        //   Transform child = this.subpowersTable.GetChild(index);
        //   if (child.gameObject.activeSelf)
        //   {
        //     SubpowerItem component = child.GetComponent<SubpowerItem>();
        //     if (component.Button.IsInteractable())
        //     {
        //       Gui.InputService.SelectCurrentSelectable((Selectable) component.Button);
        //       break;
        //     }
        //   }
        // }
    }

    public override void RegisterCommands() =>
        Gui.InputService.InputActionAsset.FindActionMap(ActionMap, false).FindAction("Cancel").performed +=
            CancelPerformed;

    public override void UnregisterCommands() =>
        Gui.InputService.InputActionAsset.FindActionMap(ActionMap, false).FindAction("Cancel").performed -=
            CancelPerformed;
}