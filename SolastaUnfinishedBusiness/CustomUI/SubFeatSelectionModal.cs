using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GameObject;

namespace SolastaUnfinishedBusiness.CustomUI;

public class SubFeatSelectionModal : GuiGameScreen
{
    private static readonly Color DEFAULT_COLOR = new(0.407f, 0.431f, 0.443f, 0.752f);
    private static readonly Color HEADER_COLOR = new(0.35f, 0.38f, 0.4f, 1f);
    private static readonly Color DISABLED_COLOR = new(0.557f, 0.431f, 0.443f, 0.752f);

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

        var corners = new Vector3[4];
        attachment.GetWorldCorners(corners);
        var step = corners[0].y - corners[1].y - 4f;
        var position = attachment.position;

        var featPrefab = Resources.Load<GameObject>("Gui/Prefabs/CharacterInspection/Proficiencies/FeatItem");

        var header = Gui.GetPrefabFromPool(featPrefab, transform).GetComponent<FeatItem>();
        InitFeatItem(inspectedCharacter, baseItem, baseItem.GuiFeatDefinition.FeatDefinition, header);
        header.GetComponent<RectTransform>().position = position;
        header.Refresh(ProficiencyBaseItem.InteractiveMode.Static, HeroDefinitions.PointsPoolType.Feat);
        SetColor(header, HEADER_COLOR);

        position += new Vector3(0, step, 0);

        var i = 0;
        foreach (var subFeat in subFeats)
        {
            var item = Gui.GetPrefabFromPool(featPrefab, transform);
            InitFeatItem(inspectedCharacter, baseItem, subFeat, item.GetComponent<FeatItem>());
            item.GetComponent<RectTransform>().position = position + new Vector3(0, step * i, 0);
            i++;
        }

        Visible = false;
    }

    private void Unbind()
    {
        itemClickHandler = null;
        for (var i = 0; i < transform.childCount; i++)
        {
            var featItem = transform.GetChild(i).GetComponent<FeatItem>();
            SetColor(featItem, DEFAULT_COLOR);
            featItem.Unbind();
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
        component.Bind(inspectedCharacter, featDefinition, OnItemSelected, null, true);
        component.StageTag = baseItem.StageTag;
        component.PreviousStageTag = baseItem.PreviousStageTag;
        component.CurrentPoolType = baseItem.CurrentPoolType;
        component.gameObject.SetActive(true);

        UpdateFeatState(component);

        component.Tooltip.Anchor = baseItem.Tooltip.Anchor;
        component.Tooltip.AnchorMode = baseItem.Tooltip.AnchorMode;

        //TODO: respect feat resize settings
        component.RectTransform.sizeDelta = new Vector2(300, 44);
    }

    private static void UpdateFeatState(FeatItem item)
    {
        var feat = item.GuiFeatDefinition.FeatDefinition;
        var currentPoolType = item.CurrentPoolType;
        var service = ServiceRepository.GetService<ICharacterBuildingService>();
        var localHeroCharacter = service.CurrentLocalHeroCharacter;
        var buildingData = localHeroCharacter?.GetHeroBuildingData();

        var pool = service.GetPointPoolOfTypeAndTag(buildingData, item.CurrentPoolType, item.StageTag);
        var restrictedChoices = pool.RestrictedChoices;

        ProficiencyBaseItem.InteractiveMode interactiveMode;
        var isSameFamily = false;
        if (localHeroCharacter != null
            && (service.IsFeatKnownOrTrained(buildingData, feat) || localHeroCharacter.TrainedFeats.Contains(feat)))
        {
            interactiveMode = ProficiencyBaseItem.InteractiveMode.Static;
        }
        else
        {
            var isRestricted = restrictedChoices.Count != 0;
            if (isRestricted)
            {
                var hasRestrictedfeats = false;
                foreach (var restrictedChoice in restrictedChoices)
                {
                    if (DatabaseRepository.GetDatabase<FeatDefinition>()
                            .GetElement(restrictedChoice, true) != null)
                    {
                        hasRestrictedfeats = true;
                        break;
                    }
                }

                isRestricted = hasRestrictedfeats && !restrictedChoices.Contains(feat.Name);
            }

            var matchingPrerequisites = service.IsFeatMatchingPrerequisites(buildingData, feat, out isSameFamily);
            if (!isRestricted && matchingPrerequisites)
            {
                if (currentPoolType == HeroDefinitions.PointsPoolType.Feat)
                {
                    interactiveMode = !service.IsFeatKnownOrTrained(buildingData, feat)
                        ? ProficiencyBaseItem.InteractiveMode.InteractiveSingle
                        : (service.IsFeatSelectedForTraining(buildingData, feat, item.StageTag)
                            ? ProficiencyBaseItem.InteractiveMode.InteractiveSingle
                            : ProficiencyBaseItem.InteractiveMode.Disabled);
                }
                else if (service.IsFeatKnownOrTrained(buildingData, feat))
                {
                    interactiveMode = ProficiencyBaseItem.InteractiveMode.InteractiveSingle;
                }
                else
                {
                    interactiveMode = ProficiencyBaseItem.InteractiveMode.Disabled;
                }
            }
            else
            {
                SetColor(item, DISABLED_COLOR);
                interactiveMode = ProficiencyBaseItem.InteractiveMode.Disabled;
            }
        }

        var disabledMode = isSameFamily
            ? ProficiencyBaseItem.DisabledMode.Strikethrough
            : ProficiencyBaseItem.DisabledMode.Default;

        item.Refresh(interactiveMode, currentPoolType, disabledMode);
    }

    private static void SetColor(FeatItem item, Color color)
    {
        item.StaticBackground.GetComponent<Image>().color = color;
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