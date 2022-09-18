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
            InitFeatItem(inspectedCharacter, baseItem, onItemClicked, subFeat, qwe.GetComponent<FeatItem>());
            qwe.GetComponent<RectTransform>().position = position + new Vector3(0, step * i, 0);
            i++;
        }

        Visible = false;
    }

    public override void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        // Load();

        base.Initialize();

        // alphaModifier.endAlpha = 1f;
        // alphaModifier.startAlpha = 0f;
        // alphaModifier.duration = 0.25f;

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

    private void InitFeatItem(RulesetCharacterHero inspectedCharacter, FeatItem baseItem,
        ProficiencyBaseItem.OnItemClickedHandler onItemClicked, FeatDefinition featDefinition, FeatItem component)
    {
        Main.Log2($"SFSM [{inspectedCharacter.Name}] Bind feat <{featDefinition.Name}>");
        component.Bind(inspectedCharacter, featDefinition, onItemClicked, null, true);
        component.StageTag = baseItem.StageTag;
        component.PreviousStageTag = baseItem.PreviousStageTag;
        component.gameObject.SetActive(true);
        component.Refresh(ProficiencyBaseItem.InteractiveMode.InteractiveSingle, HeroDefinitions.PointsPoolType.Feat,
            ProficiencyBaseItem.DisabledMode.Default);
        component.transform.localScale = baseItem.RectTransform.lossyScale;

        component.Tooltip.Anchor = baseItem.RectTransform;
        component.Tooltip.AnchorMode = TooltipDefinitions.AnchorMode.LEFT_FREE;

        component.RectTransform.sizeDelta = new Vector2(300, 44);
    }

    public void Show2(bool instant)
    {
        Main.Log2($"SFSM Show instant: {instant}");
        Show(instant);
    }

    public override bool GrabsCameraControl => true;

    public override string ActionMap => "UI";

    public override bool AutoRecomputeNavigation => true;

    public override bool HandleInput(InputCommands.Id command)
    {
        Main.Log2($"HandleInput <{command}>");
        if (command == InputCommands.Id.Cancel)
            OnCloseCb();
        return true;
    }

    public void OnCloseCb()
    {
        //TODO: implement cancel callback
        // SubpowerSelectionModal.SubpowerCanceledHandler subpowerCanceled = this.subpowerCanceled;
        // if (subpowerCanceled != null)
        //     subpowerCanceled();
        Hide();
    }

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
        // this.Unbind();
        // this.mainPanel.Hide(instant);
        base.OnBeginHide(instant);
    }

    public override void OnEndHide() => base.OnEndHide();

    public void OnActivate(int index)
    {
        //TODO: implement activation handling
        // if (this.subpowerEngaged != null)
        // {
        //   for (int index1 = 0; index1 < this.caster.UsablePowers.Count; ++index1)
        //   {
        //     RulesetUsablePower usablePower = this.caster.UsablePowers[index1];
        //     if ((BaseDefinition) usablePower.PowerDefinition == (BaseDefinition) this.powerDefinitions[index])
        //     {
        //       this.subpowerEngaged(usablePower, index1);
        //       break;
        //     }
        //   }
        // }
        Hide();
    }

    public void CancelPerformed(InputAction.CallbackContext context)
    {
        Main.Log2($"CancelPerformed");
        if ((Object) this == (Object) null)
            return;
        OnCloseCb();
    }
}