using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GameObject;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class SubFeatSelectionModal : GuiGameScreen
{
    private const float ShowDuration = 0.25f;
    private const float HideDuration = 0.1f;
    internal static readonly Color DefaultColor = new(0.407f, 0.431f, 0.443f, 0.752f);
    private static readonly Color NormalColor = new(0.407f, 0.431f, 0.443f, 1f);
    internal static readonly Color HeaderColor = new(0.35f, 0.42f, 0.45f, 1f);
    private static readonly Color DisabledColor = new(0.557f, 0.431f, 0.443f, 1f);

    private static SubFeatSelectionModal _instance;
    private GuiModifierSubMenu animator;
    private RectTransform attachment;
    private Image background;
    private FeatItem baseItem;
    private Button button;
    private RulesetCharacterHero character;
    private RectTransform featTable;
    private Image image;
    private ProficiencyBaseItem.OnItemClickedHandler itemClickHandler;

    private bool localInitialized;

    //TODO: handle controller

    public override bool GrabsCameraControl => true;

    public override string ActionMap => "UI";

    public override bool AutoRecomputeNavigation => true;

    public override void Awake()
    {
        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<GuiTooltip>();
        animator = gameObject.AddComponent<GuiModifierSubMenu>();
        image = gameObject.AddComponent<Image>();
        button = gameObject.AddComponent<Button>();

        base.Awake();
    }

    internal static SubFeatSelectionModal Get()
    {
        return _instance ??= new GameObject().AddComponent<SubFeatSelectionModal>();
    }

    internal void Bind(RulesetCharacterHero inspectedCharacter,
        FeatItem basedOn,
        FeatDefinition feat,
        IGroupedFeat group,
        ProficiencyBaseItem.OnItemClickedHandler onItemClicked,
        RectTransform attachTo)
    {
        character = inspectedCharacter;
        attachment = attachTo;
        baseItem = basedOn;

        Initialize();
        LocalInit();

        Visible = true;
        itemClickHandler = onItemClicked;

        var subFeats = group.GetSubFeats();

        var corners = new Vector3[4];

        attachTo.GetWorldCorners(corners);

        const float H_STEP = FeatsContext.Width + (2 * FeatsContext.Spacing);
        const float V_STEP = -(FeatsContext.Height + FeatsContext.Spacing);

        var position = attachTo.position;
        // ReSharper disable once Unity.UnknownResource
        var featPrefab = Resources.Load<GameObject>("Gui/Prefabs/CharacterInspection/Proficiencies/FeatItem");
        var header = Gui.GetPrefabFromPool(featPrefab, featTable).GetComponent<FeatItem>();

        InitFeatItem(feat, header);

        var headerRect = header.GetComponent<RectTransform>();
        var num = subFeats.Count;
        var columns = (int)Math.Ceiling(num / 6f);
        var d = (1 - columns) / 2f;
        var headerMaxWidth = ((columns + 0.2f) * FeatsContext.Width) + ((columns - 1) * FeatsContext.Spacing * 2);

        var sz = headerRect.sizeDelta;

        sz.x = FeatsContext.Width;
        headerRect.sizeDelta = sz;
        headerRect.position = position;
        header.Refresh(ProficiencyBaseItem.InteractiveMode.Static, HeroDefinitions.PointsPoolType.Feat);
        SetColor(header, HeaderColor);

        position += new Vector3(0, V_STEP, 0);

        var i = 0;

        foreach (var subFeat in subFeats)
        {
            var item = Gui.GetPrefabFromPool(featPrefab, featTable);

            InitFeatItem(subFeat, item.GetComponent<FeatItem>());

            var column = i % columns;
            var row = i / columns;

            item.GetComponent<RectTransform>().position =
                position + new Vector3(H_STEP * (column + d), V_STEP * row, 0);
            item.transform.SetAsFirstSibling();
            i++;
        }

        animator.Init(background, featTable, headerMaxWidth);

        Visible = false;
    }

    private void Unbind()
    {
        character = null;
        attachment = null;
        baseItem = null;
        animator.Clean();
        itemClickHandler = null;

        for (var i = 0; i < featTable.childCount; i++)
        {
            var featItem = featTable.GetChild(i).GetComponent<FeatItem>();

            featItem.Unbind();
        }

        Gui.ReleaseChildrenToPool(featTable);
    }

    internal void Cancel()
    {
        if (localInitialized)
        {
            Hide(true);
        }
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

    private void LocalInit()
    {
        if (localInitialized)
        {
            return;
        }

        var levelUp = Gui.GuiService.GetScreen<CharacterLevelUpScreen>();

        //set sort index to just above levelUp screen so it has input handling priority
        SortIndex = levelUp.SortIndex + 1;

        //put it visually just above levelUp screen
        transform.parent = Find("Application/GUI/BackgroundCanvas/ForegroundCanvas").transform.parent;

        var levelUpIndex = levelUp.transform.GetSiblingIndex();

        transform.SetSiblingIndex(levelUpIndex + 1);

        RectTransform.sizeDelta = new Vector2(200, 200);
        RectTransform.anchorMin = new Vector2(0, 0);
        RectTransform.anchorMax = new Vector2(1, 1);
        RectTransform.pivot = new Vector2(0f, 0f);
        RectTransform.position = new Vector3(0, 0, 0);

        image.sprite = Gui.GuiService.GetScreen<BlackScreen>().GetComponent<Image>().sprite;
        image.color = new Color(0, 0, 0, 0.25f);
        image.alphaHitTestMinimumThreshold = 0;

        button.onClick.AddListener(OnCloseCb);

        GuiTooltip.content = string.Empty;
        GuiTooltip.Disabled = false;
        GuiTooltip.tooltipClass = string.Empty;

        //create background
        var tmp = new GameObject();

        tmp.AddComponent<RectTransform>();

        var rt = tmp.GetComponent<RectTransform>();

        rt.parent = transform;
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0f, 0f);
        rt.position = new Vector3(0, 0, 0);
        rt.sizeDelta = new Vector2(5120, 2160);

        background = tmp.AddComponent<Image>();
        background.sprite = Gui.GuiService.GetScreen<BlackScreen>().GetComponent<Image>().sprite;
        background.color = new Color(0, 0, 0, 0.75f);
        background.alphaHitTestMinimumThreshold = 0;

        //create container for feat items
        tmp = new GameObject();
        tmp.AddComponent<RectTransform>();
        featTable = tmp.GetComponent<RectTransform>();
        featTable.parent = transform;
        featTable.anchorMin = new Vector2(0, 0);
        featTable.anchorMax = new Vector2(1, 1);
        featTable.pivot = new Vector2(0f, 0f);
        featTable.position = new Vector3(0, 0, 0);

        localInitialized = true;
    }

    private void InitFeatItem(BaseDefinition featDefinition, FeatItem component)
    {
        component.GuiFeatDefinition = ServiceRepository.GetService<IGuiWrapperService>()
            .GetGuiFeatDefinition(featDefinition.Name);
        component.Bind(character, featDefinition, OnItemSelected, true);
        component.GuiFeatDefinition.SetupTooltip(component.Tooltip, character);
        // component.Tooltip.Context = inspectedCharacter;
        component.OnItemHoverChanged = null; //baseItem.OnItemHoverChanged;

        component.StageTag = baseItem.StageTag;
        component.PreviousStageTag = baseItem.PreviousStageTag;
        component.CurrentPoolType = baseItem.CurrentPoolType;
        component.gameObject.SetActive(true);

        UpdateFeatState(component);

        component.Tooltip.Anchor = baseItem.Tooltip.Anchor;
        component.Tooltip.AnchorMode = baseItem.Tooltip.AnchorMode;

        component.RectTransform.sizeDelta = new Vector2(FeatsContext.Width, FeatsContext.Height);
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
        var color = item.GuiFeatDefinition.FeatDefinition.HasSubFeatureOfType<IGroupedFeat>()
            ? HeaderColor
            : NormalColor;

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
                var hasRestrictedFeats = restrictedChoices
                    .Any(restrictedChoice => DatabaseHelper.TryGetDefinition<FeatDefinition>(restrictedChoice, out _));

                isRestricted = hasRestrictedFeats && !restrictedChoices.Contains(feat.Name);
            }

            var matchingPrerequisites = service.IsFeatMatchingPrerequisites(buildingData, feat, out isSameFamily);

            if (!isRestricted && matchingPrerequisites)
            {
                if (currentPoolType == HeroDefinitions.PointsPoolType.Feat)
                {
                    interactiveMode = !service.IsFeatKnownOrTrained(buildingData, feat)
                        ? ProficiencyBaseItem.InteractiveMode.InteractiveSingle
                        : service.IsFeatSelectedForTraining(buildingData, feat, item.StageTag)
                            ? ProficiencyBaseItem.InteractiveMode.InteractiveSingle
                            : ProficiencyBaseItem.InteractiveMode.Disabled;
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
                color = DisabledColor;
                interactiveMode = ProficiencyBaseItem.InteractiveMode.Disabled;
            }
        }

        var disabledMode = isSameFamily
            ? ProficiencyBaseItem.DisabledMode.Strikethrough
            : ProficiencyBaseItem.DisabledMode.Default;

        item.Refresh(interactiveMode, currentPoolType, disabledMode);
        SetColor(item, color);
    }

    internal static void SetColor(FeatItem item, Color color)
    {
        item.StaticBackground.GetComponent<Image>().color = color;
        item.canvasGroup.alpha = 1;
    }

    private void OnItemSelected(ProficiencyBaseItem item)
    {
        var handler = itemClickHandler;

        itemClickHandler = null;

        if (item is FeatItem featItem
            && featItem.GuiFeatDefinition.FeatDefinition is { } feat
            && feat.GetFirstSubFeatureOfType<IGroupedFeat>() is { } group)
        {
            var tmpCharacter = character;
            var tmpAttachment = attachment;
            var tmpItem = baseItem;

            Hide(true);
            Bind(tmpCharacter, tmpItem, feat, group, handler, tmpAttachment);
            Show();
        }
        else
        {
            handler?.Invoke(item);
            Hide();
        }
    }

    private void OnCloseCb()
    {
        Hide();
    }

    public override void OnBeginShow(bool instant = false)
    {
        base.OnBeginShow(instant);
        animator.duration = ShowDuration;

        if (Gui.GamepadActive)
        {
            Gui.InputService.ClearCurrentSelectable();
        }

        ServiceRepository.GetService<ITooltipService>().HideTooltip();
    }

    public override void OnBeginHide(bool instant = false)
    {
        animator.duration = HideDuration;
        base.OnBeginHide(instant);
    }

    public override void OnEndHide()
    {
        Unbind();
        base.OnEndHide();
    }

    public override bool HandleInput(InputCommands.Id command)
    {
        if (command == InputCommands.Id.Cancel)
        {
            OnCloseCb();
        }

        return true;
    }

    public void CancelPerformed(InputAction.CallbackContext context)
    {
        if (this == null)
        {
            return;
        }

        OnCloseCb();
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

    public override void RegisterCommands()
    {
        Gui.InputService.InputActionAsset.FindActionMap(ActionMap).FindAction("Cancel").performed +=
            CancelPerformed;
    }

    public override void UnregisterCommands()
    {
        Gui.InputService.InputActionAsset.FindActionMap(ActionMap).FindAction("Cancel").performed -=
            CancelPerformed;
    }
}
