using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class CustomInvocationSelectionPanel : CharacterStagePanel
{
    private const float ScrollDuration = 0.3f;
    private const float SpellsByLevelMargin = 10.0f;

    private readonly List<FeaturePool> _allPools = [];
    private readonly List<(string, FeatureDefinitionCustomInvocationPool)> _gainedCustomFeatures = [];

    private readonly Dictionary<PoolId, List<InvocationDefinitionCustom>> _learnedInvocations = [];

    private readonly Comparison<FeaturePool> _poolCompare = (a, b) =>
    {
        var r = String.CompareOrdinal(a.Id.Tag, b.Id.Tag);

        if (r != 0)
        {
            return r;
        }

        if (a.IsUnlearn == b.IsUnlearn)
        {
            return String.CompareOrdinal(a.Id.Name, b.Id.Name);
        }

        if (a.IsUnlearn)
        {
            return -1;
        }

        return 1;
    };

    private int _currentLearnStep;
    private int _gainedCharacterLevel;
    private CharacterClassDefinition _gainedClass;
    private int _gainedClassLevel;
    private CharacterSubclassDefinition _gainedSubclass;
    private bool _wasClicked;

    private bool IsFinalStep => _currentLearnStep >= _allPools.Count;

    private void OnFeatureSelected(SpellBox spellBox)
    {
        if (_wasClicked)
        {
            return;
        }

        _wasClicked = true;

        var feature = spellBox.GetFeature();
        var pool = _allPools[_currentLearnStep];
        var learned = GetOrMakeLearnedList(pool.Id);

        if (learned.Contains(feature))
        {
            pool.Used--;
            learned.Remove(feature);

            if (pool.IsUnlearn)
            {
                var poolById = GetPoolById(new PoolId(pool.Id.Name, pool.Id.Tag, false));

                if (poolById != null)
                {
                    poolById.Max--;
                }
            }
        }
        else
        {
            pool.Used++;
            learned.Add(feature);

            if (pool.IsUnlearn)
            {
                GetOrAddPoolById(new PoolId(pool.Id.Name, pool.Id.Tag, false), pool.Type).Max++;
            }
        }

        GrantAcquiredFeatures(() =>
        {
            CommonData.AbilityScoresListingPanel.RefreshNow();
            CommonData.CharacterStatsPanel.RefreshNow();

            // don't use ? on Unity Objects
#pragma warning disable IDE0031
            if (CommonData.AttackModesPanel != null)
#pragma warning restore IDE0031
            {
                CommonData.AttackModesPanel.RefreshNow();
            }

            // don't use ? on Unity Objects
#pragma warning disable IDE0031
            if (CommonData.PersonalityMapPanel != null)
#pragma warning restore IDE0031
            {
                CommonData.PersonalityMapPanel.RefreshNow();
            }

            OnPreRefresh();
            RefreshNow();

            if (pool.Remaining == 0)
            {
                MoveToNextLearnStep();
            }

            ResetWasClickedFlag();
        });
    }

    private void BuildLearnSteps()
    {
        // Register all steps
        if (_allPools is not { Count: > 0 })
        {
            return;
        }

        while (_learnStepsTable.childCount < _allPools.Count)
        {
            Gui.GetPrefabFromPool(_learnStepPrefab, _learnStepsTable);
        }

        for (var i = 0; i < _learnStepsTable.childCount; i++)
        {
            var child = _learnStepsTable.GetChild(i);

            if (i < _allPools.Count)
            {
                var learnStepItem = child.GetComponent<LearnStepItem>();

                child.gameObject.SetActive(true);
                learnStepItem.CustomBind(i, _allPools[i],
                    OnLearnBack,
                    OnLearnReset,
                    OnSkipRemaining
                );
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }


    private void OnLearnBack()
    {
        if (_wasClicked)
        {
            return;
        }

        _wasClicked = true;

        MoveToPreviousLearnStep();
    }

    private void OnLearnReset()
    {
        if (_wasClicked)
        {
            return;
        }

        _wasClicked = true;

        if (IsFinalStep)
        {
            _currentLearnStep = _allPools.Count - 1;
        }

        ResetLearnings(_currentLearnStep, () =>
        {
            OnPreRefresh();
            RefreshNow();
            ResetWasClickedFlag();
        });
    }

    private void OnSkipRemaining()
    {
        if (_wasClicked)
        {
            return;
        }

        _wasClicked = true;

        if (IsUnlearnStep(_currentLearnStep))
        {
            _allPools[_currentLearnStep].Skipped = true;
            MoveToNextLearnStep();
        }

        ResetWasClickedFlag();
    }

    private void ResetLearnings(int stepNumber, Action onDone = null)
    {
        // this happens when users go back on selection if UI is offered on 1st level [i.e.: Draconic Choices]
        if (IsFinalStep)
        {
            stepNumber = 0;
        }

        var pool = _allPools[stepNumber];

        pool.Used = 0;
        pool.Skipped = false;
        GetOrMakeLearnedList(pool.Id).Clear();
        GrantAcquiredFeatures(onDone);
    }

    private void MoveToNextLearnStep()
    {
        _currentLearnStep++;

        while (!IsFinalStep && _allPools[_currentLearnStep].Remaining == 0)
        {
            _currentLearnStep++;
        }

        LevelSelected(0);
        OnPreRefresh();
        RefreshNow();
    }

    private void MoveToPreviousLearnStep()
    {
        var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

        if (_currentLearnStep > 0)
        {
            if (!IsFinalStep)
            {
                ResetLearnings(_currentLearnStep);
            }

            _currentLearnStep--;
            ResetLearnings(_currentLearnStep);

            if (IsUnlearnStep(_currentLearnStep))
            {
                heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
                {
                    CollectTags();
                    BuildLearnSteps();
                });
            }
        }

        heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
        {
            LevelSelected(0);
            OnPreRefresh();
            RefreshNow();
            ResetWasClickedFlag();
        });
    }


    private void GrantAcquiredFeatures(Action onDone = null)
    {
        var hero = currentHero;
        var heroBuildingData = hero.GetHeroBuildingData();
        var command = ServiceRepository.GetService<IHeroBuildingCommandService>();

        //remove all trained custom invocations
        var entries = heroBuildingData.levelupTrainedInvocations.ToList();

        foreach (var entry in entries)
        {
            var currentTag = entry.Key;
            var invocations = new List<InvocationDefinition>(entry.Value);

            foreach (var invocation in invocations)
            {
                if (invocation is not InvocationDefinitionCustom custom)
                {
                    continue;
                }

                command.UntrainCharacterFeature(hero, currentTag, custom.Name,
                    HeroDefinitions.PointsPoolType.Invocation);
            }
        }

        //remove all unlearned custom invocations
        entries = heroBuildingData.unlearnedInvocations.ToList();

        foreach (var entry in entries)
        {
            var currentTag = entry.Key;
            var invocations = new List<InvocationDefinition>(entry.Value);

            foreach (var invocation in invocations)
            {
                if (invocation is not InvocationDefinitionCustom custom)
                {
                    continue;
                }

                command.UndoUnlearnInvocation(hero, currentTag, custom.Name);
            }
        }

        //add all learned/unlearned custom invocations
        foreach (var keyValuePair in _learnedInvocations)
        {
            var poolId = keyValuePair.Key;

            foreach (var invocation in keyValuePair.Value)
            {
                if (poolId.Unlearn)
                {
                    command.UnlearnInvocation(hero, poolId.Tag, invocation.Name);
                }
                else
                {
                    command.TrainCharacterFeature(hero, poolId.Tag, invocation.Name,
                        HeroDefinitions.PointsPoolType.Invocation);
                }
            }
        }

        command.RefreshHero(currentHero);
        command.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
        {
            RemoveInvalidFeatures(onDone);
        });
    }

    private void RemoveInvalidFeatures(Action onDone = null)
    {
        var dirty = false;

        foreach (var e in _learnedInvocations)
        {
            var id = e.Key;
            var learned = e.Value;
            var pool = GetPoolById(id);

            for (var i = 0; i < learned.Count;)
            {
                var feature = learned[i];

                if (!PowerBundle.ValidatePrerequisites(currentHero, feature, feature.Validators, out _))
                {
                    dirty = true;
                    learned.RemoveAt(i);
                    pool.Used--;
                }
                else
                {
                    i++;
                }
            }
        }

        if (dirty)
        {
            GrantAcquiredFeatures(onDone);
        }
        else
        {
            onDone?.Invoke();
        }
    }

    private FeaturePool GetPoolById(PoolId id)
    {
        return _allPools.FirstOrDefault(p => p.Id.Equals(id));
    }

    private FeaturePool GetOrAddPoolById(PoolId id, InvocationPoolTypeCustom type)
    {
        var pool = GetPoolById(id);

        if (pool != null)
        {
            return pool;
        }

        pool = new FeaturePool(id) { Type = type, Max = 0, Used = 0 };

        _allPools.Add(pool);
        _allPools.Sort(_poolCompare);
        BuildLearnSteps();

        return pool;
    }

    private bool IsUnlearnStep(int step)
    {
        return _allPools[step].IsUnlearn;
    }

    private List<InvocationDefinitionCustom> GetOrMakeLearnedList(PoolId id)
    {
        if (_learnedInvocations.TryGetValue(id, out var value))
        {
            return value;
        }

        var learned = new List<InvocationDefinitionCustom>();

        _learnedInvocations.Add(id, learned);

        return learned;
    }

    private List<InvocationDefinitionCustom> GetOrMakeUnlearnedList(PoolId id)
    {
        return GetOrMakeLearnedList(new PoolId(id.Name, id.Tag, true));
    }


    private void UpdateGainedClassDetails()
    {
        // Determine the last class and level
        CharacterBuildingService.GetLastAssignedClassAndLevel(currentHero, out _gainedClass, out _gainedClassLevel);
        _gainedCharacterLevel = currentHero.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);

        if (_gainedClass == null)
        {
            return;
        }

        // Was there already a subclass?
        _gainedSubclass = null;

        if (currentHero.ClassesAndSubclasses.TryGetValue(_gainedClass, out var subclass))
        {
            _gainedSubclass = subclass;
        }
    }

    private string GetClassTag()
    {
        return AttributeDefinitions.GetClassTag(_gainedClass, _gainedClassLevel);
    }

    private string GetSubClassTag()
    {
        return _gainedSubclass == null
            ? null
            : AttributeDefinitions.GetSubclassTag(_gainedClass, _gainedClassLevel, _gainedSubclass);
    }

    private void CollectTags()
    {
        Dictionary<PoolId, FeaturePool> tags = [];

        UpdateGrantedFeatures();
        _allPools.Clear();

        foreach (var (poolTag, featureSet) in _gainedCustomFeatures)
        {
            var poolId = new PoolId(featureSet.PoolType.Name, poolTag, featureSet.IsUnlearn);

            if (!tags.TryGetValue(poolId, out var value))
            {
                value = new FeaturePool(poolId) { Max = featureSet.Points, Used = 0, Type = featureSet.PoolType };

                tags.Add(poolId, value);
                _allPools.Add(value);
            }
            else
            {
                value.Max += featureSet.Points;
            }
        }

        _allPools.Sort(_poolCompare);

        initialized = true;
    }

    private void UpdateGrantedFeatures()
    {
        UpdateGainedClassDetails();

        _gainedCustomFeatures.Clear();

        if (_gainedClass == null)
        {
            return;
        }

        var poolTag = GetClassTag();
        var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
        var hero = characterBuildingService.CurrentLocalHeroCharacter;

        if (hero == null)
        {
            return;
        }

        // note we assume pools from feats are merged on class tags
        var heroBuildingData = hero.GetHeroBuildingData();

        // add from feats
        _gainedCustomFeatures.AddRange(heroBuildingData.LevelupTrainedFeats
            .SelectMany(x => x.Value)
            .SelectMany(f => f.Features)
            .OfType<FeatureDefinitionCustomInvocationPool>()
            .Where(x => x.PoolType != null)
            .Select(f => (poolTag, f)));

        if (poolTag != null)
        {
            _gainedCustomFeatures.AddRange(RulesetActorExtensions.FlattenFeatureList(_gainedClass.FeatureUnlocks
                    .Where(f => f.Level == _gainedClassLevel)
                    .Select(f => f.FeatureDefinition))
                .OfType<FeatureDefinitionCustomInvocationPool>()
                .Where(x => x.PoolType != null)
                .Select(f => (poolTag, f))
            );

            // add from class FS
            if (hero.TrainedFightingStyles.Count > 0 &&
                hero.ActiveFeatures.TryGetValue(poolTag, out var featureDefinitions) &&
                featureDefinitions
                    .OfType<FeatureDefinitionFightingStyleChoice>()
                    .Any())
            {
                _gainedCustomFeatures.AddRange(
                    hero.TrainedFightingStyles.Last().Features
                        .OfType<FeatureDefinitionCustomInvocationPool>()
                        .Where(x => x.PoolType != null)
                        .Select(f => (poolTag, f))
                );
            }
        }

        poolTag = GetSubClassTag();

        if (poolTag != null)
        {
            _gainedCustomFeatures.AddRange(RulesetActorExtensions.FlattenFeatureList(_gainedSubclass.FeatureUnlocks
                    .Where(f => f.Level == _gainedClassLevel)
                    .Select(f => f.FeatureDefinition))
                .OfType<FeatureDefinitionCustomInvocationPool>()
                .Where(x => x.PoolType != null)
                .Select(f => (poolTag, f))
            );

            // add from subclass FS
            if (hero.TrainedFightingStyles.Count > 0 &&
                hero.ActiveFeatures.TryGetValue(poolTag, out var featureDefinitions) &&
                featureDefinitions
                    .OfType<FeatureDefinitionFightingStyleChoice>()
                    .Any())
            {
                _gainedCustomFeatures.AddRange(
                    hero.TrainedFightingStyles.Last().Features
                        .OfType<FeatureDefinitionCustomInvocationPool>()
                        .Where(x => x.PoolType != null)
                        .Select(f => (poolTag, f))
                );
            }
        }

        var gainedRace = currentHero.RaceDefinition;

        if (gainedRace != null)
        {
            _gainedCustomFeatures.AddRange(RulesetActorExtensions.FlattenFeatureList(gainedRace.FeatureUnlocks
                    .Where(f => f.Level == currentHero.ClassesHistory.Count)
                    .Select(f => f.FeatureDefinition))
                .OfType<FeatureDefinitionCustomInvocationPool>()
                .Where(x => x.PoolType != null)
                .Select(f => (AttributeDefinitions.TagRace, f))
            );
        }

        InvocationPoolTypeCustom.RefreshAll();
    }

    internal class FeaturePool
    {
        internal bool Skipped;
        internal FeaturePool(PoolId id) { Id = id; }
        internal PoolId Id { get; }
        internal int Max { get; set; }
        internal int Used { get; set; }
        internal int Remaining => Skipped ? 0 : Max - Used;
        internal InvocationPoolTypeCustom Type { get; set; }
        internal bool IsUnlearn => Id.Unlearn;
    }

    internal class PoolId
    {
        internal PoolId(string name, string tag, bool unlearn)
        {
            Name = name;
            Tag = tag;
            Unlearn = unlearn;
        }

        internal string Name { get; }
        internal string Tag { get; }
        internal bool Unlearn { get; }

        public override bool Equals(object obj)
        {
            if (obj is not PoolId pool)
            {
                return false;
            }

            return Name == pool.Name && Tag == pool.Tag && Unlearn == pool.Unlearn;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"<{Name}:[{Tag}]:{Unlearn}>";
        }
    }

    #region Fields from CharacterStageSpellSelectionPanel

    private RectTransform _spellsByLevelTable;
    private GameObject _spellsByLevelPrefab;
    private ScrollRect _spellsScrollRect;
    private RectTransform _learnStepsTable;
    private GameObject _learnStepPrefab;
    private AssetReferenceSprite _backdropReference;
    private Image _backdrop;
    private AnimationCurve _curve;
    private RectTransform _levelButtonsTable;
    private GameObject _levelButtonPrefab;
    private GuiLabel _stageTitleLabel;
    private GuiLabel _rightFeaturesLabel;
    private GuiLabel _rightFeaturesDescription;

    #endregion

    #region Initial Setup

    internal static void InsertPanel(CharacterEditionScreen screen)
    {
        switch (screen)
        {
            case CharacterCreationScreen:
            {
                var customFeatureSelection = GetPanel(screen);
                var last = screen.stagePanelsByName.ElementAt(screen.stagePanelsByName.Count - 1);

                screen.stagePanelsByName.Remove(last.Key);
                screen.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                screen.stagePanelsByName.Add(last.Key, last.Value);
                break;
            }
            case CharacterLevelUpScreen:
            {
                var customFeatureSelection = GetPanel(screen);

                screen.stagePanelsByName.Add(customFeatureSelection.Name, customFeatureSelection);
                break;
            }
        }
    }

    [NotNull]
    private static CustomInvocationSelectionPanel GetPanel([NotNull] CharacterEditionScreen screen)
    {
        var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
        var stagePanelPrefabs = characterCreationScreen.stagePanelPrefabs;
        var gameObject = Gui.GetPrefabFromPool(stagePanelPrefabs[8], screen.StagesPanelContainer);
        var customFeatureSelection = gameObject.AddComponent<CustomInvocationSelectionPanel>();

        customFeatureSelection.Setup();

        return customFeatureSelection;
    }

    private void Setup()
    {
        var spellsPanel = GetComponent<CharacterStageSpellSelectionPanel>();

        stageDefinition = spellsPanel.StageDefinition;
        _spellsByLevelTable = spellsPanel.spellsByLevelTable;
        _spellsByLevelPrefab = spellsPanel.spellsByLevelPrefab;
        _spellsScrollRect = spellsPanel.spellsScrollRect;
        _learnStepsTable = spellsPanel.learnStepsTable;
        _learnStepPrefab = spellsPanel.learnStepPrefab;
        _backdropReference = spellsPanel.backdropReference;
        _backdrop = spellsPanel.backdrop;
        _curve = spellsPanel.curve;
        _levelButtonsTable = spellsPanel.levelButtonsTable;
        _levelButtonPrefab = spellsPanel.levelButtonPrefab;
        _stageTitleLabel = spellsPanel.RectTransform.FindChildRecursive("ChoiceTitle").GetComponent<GuiLabel>();
        _rightFeaturesLabel =
            spellsPanel.RectTransform.FindChildRecursive("SpellsInfoTitle").GetComponent<GuiLabel>();
        _rightFeaturesDescription = spellsPanel.RectTransform.FindChildRecursive("ProficienciesIntroDescription")
            .GetComponent<GuiLabel>();
        CharacterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
        currentHero = spellsPanel.currentHero;

        Destroy(spellsPanel);
    }

    #endregion

    #region Base Class Overrides

    public override string Name => "CustomFeatureSelection";
    public override string Title => "UI/&CustomFeatureSelectionStageTitle";
    public override string Description => "UI/&CustomFeatureSelectionStageDescription";

    public override void SetScrollSensitivity(float scrollSensitivity)
    {
        _spellsScrollRect.scrollSensitivity = -scrollSensitivity;
    }

    public override void UpdateRelevance()
    {
        UpdateGrantedFeatures();
        IsRelevant = _gainedCustomFeatures.Count != 0;
    }

    public override void EnterStage()
    {
        _stageTitleLabel.Text = Title;
        _rightFeaturesLabel.Text = "UI/&CustomFeatureSelectionStageFeatures";
        _rightFeaturesDescription.Text = Description;
        _currentLearnStep = 0;
        initialized = false;
        CollectTags();
        OnEnterStageDone();
    }

    public override void OnBeginShow(bool instant = false)
    {
        base.OnBeginShow(instant);

        _backdrop.sprite = Gui.LoadAssetSync<Sprite>(_backdropReference);

        CommonData.CharacterStatsPanel.Show(CharacterStatsPanel.ArmorClassFlag |
                                            CharacterStatsPanel.InitiativeFlag | CharacterStatsPanel.MoveFlag |
                                            CharacterStatsPanel.ProficiencyFlag |
                                            CharacterStatsPanel.HitPointMaxFlag |
                                            CharacterStatsPanel.HitDiceFlag);

        for (var i = _currentLearnStep; i >= 0; i--)
        {
            ResetLearnings(i);
        }

        BuildLearnSteps();
        _spellsScrollRect.normalizedPosition = Vector2.zero;
        OnPreRefresh();
        RefreshNow();
    }

    public override void OnEndHide()
    {
        _learnedInvocations.Clear();

        for (var i = 0; i < _spellsByLevelTable.childCount; i++)
        {
            var child = _spellsByLevelTable.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            var group = child.GetComponent<SpellsByLevelGroup>();
            group.CustomUnbind();
        }

        Gui.ReleaseChildrenToPool(_spellsByLevelTable);
        Gui.ReleaseChildrenToPool(_learnStepsTable);
        Gui.ReleaseChildrenToPool(_levelButtonsTable);

        base.OnEndHide();

        if (_backdrop.sprite == null)
        {
            return;
        }

        Gui.ReleaseAddressableAsset(_backdrop.sprite);
        _backdrop.sprite = null;
    }

    public override bool CanProceedToNextStage(out string failureString)
    {
        if (!IsFinalStep ||
            !initialized ||
            (_allPools.Count != 0 &&
             _allPools[_allPools.Count - 1].Remaining > 0))
        {
            failureString = Gui.Localize("UI/&CustomFeatureSelectionStageNotDone");
            return false;
        }

        failureString = string.Empty;

        return true;
    }

    public override void CancelStage()
    {
        initialized = false;

        while (IsFinalStep)
        {
            _currentLearnStep--;
        }

        for (var i = _currentLearnStep; i >= 0; i--)
        {
            ResetLearnings(i);
        }

        var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

        heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(OnCancelStageDone);
    }

    public override void Refresh()
    {
        var currentPoolIndex = 0;

        for (var i = 0; i < _learnStepsTable.childCount; i++)
        {
            var child = _learnStepsTable.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            var stepItem = child.GetComponent<LearnStepItem>();

            LearnStepItem.Status status;

            if (i == _currentLearnStep)
            {
                status = LearnStepItem.Status.InProgress;
            }
            else if (i == _currentLearnStep - 1)
            {
                status = LearnStepItem.Status.Previous;
            }
            else
            {
                status = LearnStepItem.Status.Locked;
            }

            stepItem.CustomRefresh(status, _allPools[i]);

            if (status == LearnStepItem.Status.InProgress)
            {
                currentPoolIndex = i;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_learnStepsTable);

        if (IsFinalStep)
        {
            currentPoolIndex = _allPools.Count - 1;
        }

        var currentPoolId = _allPools[currentPoolIndex].Id;
        var isUnlearnStep = IsUnlearnStep(currentPoolIndex);
        var featurePool = GetPoolById(currentPoolId);
        var allLevels = featurePool.Type.AllLevels;
        var requiredGroups = allLevels.Count;

        while (_spellsByLevelTable.childCount < requiredGroups)
        {
            Gui.GetPrefabFromPool(_spellsByLevelPrefab, _spellsByLevelTable);
        }

        float totalWidth = 0;
        float lastWidth = 0;
        var layout = _spellsByLevelTable.GetComponent<HorizontalLayoutGroup>();

        layout.padding.left = (int)SpellsByLevelMargin;

        for (var i = 0; i < _spellsByLevelTable.childCount; i++)
        {
            var child = _spellsByLevelTable.GetChild(i);

            child.gameObject.SetActive(i < requiredGroups);

            if (i >= requiredGroups)
            {
                continue;
            }

            var group = child.GetComponent<SpellsByLevelGroup>();
            var featureLevel = allLevels[i];
            var lowLevel = !isUnlearnStep && featureLevel > (featurePool.Type.RequireClassLevels != null
                ? Math.Max(1,
                    InvocationPoolTypeCustom.GetClassOrSubclassLevel(currentHero, featurePool.Type.RequireClassLevels))
                : _gainedCharacterLevel);

            group.Selected = !IsFinalStep && !lowLevel;

            var unlearnedFeatures = isUnlearnStep
                ? GetOrMakeLearnedList(featurePool.Id)
                : GetOrMakeUnlearnedList(featurePool.Id);

            group.CustomFeatureBind(
                currentHero,
                featurePool.Type,
                GetOrMakeLearnedList(featurePool.Id),
                featureLevel,
                unlearnedFeatures,
                group.Selected,
                isUnlearnStep,
                OnFeatureSelected
            );

            lastWidth = group.RectTransform.rect.width + layout.spacing;
            totalWidth += lastWidth;
        }

        // Compute manually the table width, adding a reserve of fluff for the scroll view
        totalWidth += _spellsScrollRect.GetComponent<RectTransform>().rect.width - lastWidth;
        _spellsByLevelTable.sizeDelta = new Vector2(totalWidth, _spellsByLevelTable.sizeDelta.y);

        // Spell Level Buttons
        while (_levelButtonsTable.childCount < requiredGroups)
        {
            Gui.GetPrefabFromPool(_levelButtonPrefab, _levelButtonsTable);
        }

        // Bind the required group, once for each spell level
        for (var i = 0; i < requiredGroups; i++)
        {
            var child = _levelButtonsTable.GetChild(i);
            child.gameObject.SetActive(true);
            var button = child.GetComponent<SpellLevelButton>();
            button.CustomBind(allLevels[i], LevelSelected);
        }

        // Hide remaining useless groups
        for (var i = requiredGroups; i < _levelButtonsTable.childCount; i++)
        {
            var child = _levelButtonsTable.GetChild(i);
            child.gameObject.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_spellsByLevelTable);

        base.Refresh();
    }

    #endregion

    #region UI helpers

    private void ResetWasClickedFlag()
    {
        _wasClicked = false;
    }

    private void LevelSelected(int level)
    {
        StartCoroutine(BlendToLevelGroup(level));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator BlendToLevelGroup(int level)
    {
        SpellsByLevelGroup group = null;
        var duration = ScrollDuration;
        var shouldAssignGroup = true;

        foreach (Transform child in _spellsByLevelTable)
        {
            var spellByLevelGroup = child.GetComponent<SpellsByLevelGroup>();

            if (shouldAssignGroup)
            {
                shouldAssignGroup = false;
                group = spellByLevelGroup;
            }

            if (spellByLevelGroup.SpellLevel == level)
            {
                group = spellByLevelGroup;
            }
        }

        var initialX = _spellsByLevelTable.anchoredPosition.x;
        var finalX = -group!.RectTransform.anchoredPosition.x + SpellsByLevelMargin;

        while (duration > 0)
        {
            _spellsByLevelTable.anchoredPosition = new Vector2(
                Mathf.Lerp(initialX, finalX, _curve.Evaluate((ScrollDuration - duration) / ScrollDuration)), 0);
            duration -= Gui.SystemDeltaTime;
            yield return null;
        }

        _spellsByLevelTable.anchoredPosition = new Vector2(finalX, 0);
    }

    #endregion
}

#region Widget Extensions

internal static class LearnStepItemExtension
{
    internal static void CustomBind(
        this LearnStepItem instance,
        int rank,
        CustomInvocationSelectionPanel.FeaturePool pool,
        LearnStepItem.ButtonActivatedHandler onBackOneStepActivated,
        LearnStepItem.ButtonActivatedHandler onResetActivated,
        LearnStepItem.ButtonActivatedHandler onAutoSelectActivated)
    {
        instance.Tag = pool.Id.Tag;
        instance.PoolType = pool.IsUnlearn
            ? HeroDefinitions.PointsPoolType.SpellUnlearn
            : HeroDefinitions.PointsPoolType.Irrelevant;
        instance.rank = rank;
        instance.ignoreAvailable = pool.IsUnlearn;
        instance.autoLearnAvailable = false;

        var header = pool.Type.FormatTitle(pool.IsUnlearn);

        instance.headerLabelActive.Text = header;
        instance.headerLabelInactive.Text = header;
        instance.OnBackOneStepActivated = onBackOneStepActivated;
        instance.OnResetActivated = onResetActivated;
        instance.OnAutoSelectActivated = onAutoSelectActivated;
    }

    internal static void CustomRefresh(
        this LearnStepItem instance,
        LearnStepItem.Status status,
        CustomInvocationSelectionPanel.FeaturePool pool)
    {
        var usedPoints = pool.Used;
        var maxPoints = pool.Max;
        var ignoreAvailable = instance.ignoreAvailable;
        var choiceLabel = instance.choicesLabel;
        var activeGroup = instance.activeGroup;
        var inactiveGroup = instance.inactiveGroup;
        var autoButton = instance.autoButton;
        var ignoreButton = instance.ignoreButton;
        var resetButton = instance.resetButton;

        activeGroup.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
        inactiveGroup.gameObject.SetActive(status != LearnStepItem.Status.InProgress);
        instance.activeBackground.gameObject.SetActive(status != LearnStepItem.Status.Locked);
        instance.inactiveBackground.gameObject.SetActive(status == LearnStepItem.Status.Locked);
        instance.backOneStepButton.gameObject.SetActive(status == LearnStepItem.Status.Previous);
        resetButton.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
        autoButton.gameObject.SetActive(false);
        ignoreButton.gameObject.SetActive(ignoreAvailable);
        instance.ignoreButton.gameObject.SetActive(ignoreAvailable);

        if (status == LearnStepItem.Status.InProgress)
        {
            instance.pointsLabelActive.Text = Gui.FormatCurrentOverMax(usedPoints, maxPoints);
            instance.remainingPointsGaugeActive.fillAmount = (float)usedPoints / maxPoints;
            choiceLabel.Text = pool.Type.FormatDescription(pool.IsUnlearn);
            LayoutRebuilder.ForceRebuildLayoutImmediate(choiceLabel.RectTransform);
            var sizeDelta = new Vector2(activeGroup.sizeDelta.x,
                (float)(choiceLabel.RectTransform.rect.height - choiceLabel.RectTransform.anchoredPosition.y +
                        12.0));
            activeGroup.sizeDelta = sizeDelta;
            instance.RectTransform.sizeDelta = sizeDelta;
            resetButton.interactable = usedPoints < maxPoints;
            autoButton.interactable = false;
        }
        else
        {
            instance.pointsLabelInactive.Text = Gui.FormatCurrentOverMax(usedPoints, maxPoints);
            instance.remainingPointsGaugeInactive.fillAmount = (float)usedPoints / maxPoints;
            instance.RectTransform.sizeDelta = inactiveGroup.sizeDelta;
            instance.backOneStepButton.interactable = true;
        }
    }
}

internal static class SpellLevelButtonExtension
{
    internal static void CustomBind(
        this SpellLevelButton instance,
        int level,
        SpellLevelButton.LevelSelectedHandler levelSelected)
    {
        instance.Level = level;
        instance.LevelSelected = levelSelected;
        instance.label.Text = level > 0 ? Gui.ToRoman(level) : $"{level}";
    }
}

internal static class SpellsByLevelGroupExtensions
{
    internal static void CustomFeatureBind(this SpellsByLevelGroup instance,
        RulesetCharacterHero hero,
        InvocationPoolTypeCustom pool,
        List<InvocationDefinitionCustom> learned,
        int featureLevel,
        List<InvocationDefinitionCustom> unlearned,
        bool canAcquireFeatures,
        bool unlearn,
        SpellBox.SpellBoxChangedHandler spellBoxChanged)
    {
        instance.name = $"Feature[{pool.Name}]";
        instance.SpellLevel = featureLevel;

        var allFeatures = pool.GetLevelFeatures(featureLevel);

        allFeatures.Sort((a, b) => string.CompareOrdinal(a.FormatTitle(), b.FormatTitle()));

        var spellsTable = instance.spellsTable;
        var spellPrefab = instance.spellPrefab;

        while (spellsTable.childCount < allFeatures.Count)
        {
            Gui.GetPrefabFromPool(spellPrefab, spellsTable);
        }

        var component1 = spellsTable.GetComponent<GridLayoutGroup>();

        component1.constraintCount = Mathf.Max(3, Mathf.CeilToInt(allFeatures.Count / 4f));

        for (var index = 0; index < allFeatures.Count; ++index)
        {
            var feature = allFeatures[index];

            spellsTable.GetChild(index).gameObject.SetActive(true);

            var box = spellsTable.GetChild(index).GetComponent<SpellBox>();
            var isUnlearned = unlearned.Contains(feature);
            var bindMode = unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning;

            box.CustomFeatureBind(feature, isUnlearned, bindMode, spellBoxChanged);
        }

        //disable unneeded spell boxes
        for (var count = allFeatures.Count; count < spellsTable.childCount; ++count)
        {
            spellsTable.GetChild(count).gameObject.SetActive(false);
        }

        var x = (float)((component1.constraintCount * (double)component1.cellSize.x) +
                        ((component1.constraintCount - 1) * (double)component1.spacing.x));

        spellsTable.sizeDelta = new Vector2(x, spellsTable.sizeDelta.y);
        instance.RectTransform.sizeDelta = new Vector2(x, instance.RectTransform.sizeDelta.y);

        instance.slotStatusTable.Bind(null, featureLevel, false, null, false);

        if (unlearn)
        {
            instance.RefreshUnlearning(hero, pool, unlearned, canAcquireFeatures);
        }
        else
        {
            instance.RefreshLearning(hero, pool, learned, unlearned, canAcquireFeatures);
        }
    }

    private static void RefreshLearning(this SpellsByLevelGroup instance,
        RulesetCharacterHero hero,
        InvocationPoolTypeCustom pool,
        // ReSharper disable once SuggestBaseTypeForParameter
        List<InvocationDefinitionCustom> learned,
        // ReSharper disable once SuggestBaseTypeForParameter
        List<InvocationDefinitionCustom> unlearnedFeatures,
        bool canAcquireFeatures)
    {
        foreach (Transform transform in instance.spellsTable)
        {
            if (!transform.gameObject.activeSelf)
            {
                continue;
            }

            var box = transform.GetComponent<SpellBox>();
            var boxFeature = box.GetFeature();
            var alreadyHas = hero.TrainedInvocations.Contains(boxFeature);
            var selected = learned.Contains(boxFeature);
            var isUnlearned = unlearnedFeatures != null && unlearnedFeatures.Contains(boxFeature);
            var isValid = PowerBundle.ValidatePrerequisites(
                hero, boxFeature, boxFeature.Validators, out var requirements);
            var canLearn = !selected && !alreadyHas && isValid;

            box.SetupUI(hero, pool.Sprite, requirements);

            if (canAcquireFeatures)
            {
                box.CustomRefreshLearningInProgress((canLearn || selected) && !isUnlearned, selected,
                    alreadyHas);
            }
            else
            {
                box.RefreshLearningInactive((selected || alreadyHas) && !isUnlearned);
            }
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void RefreshUnlearning(
        this SpellsByLevelGroup instance,
        RulesetCharacterHero hero,
        InvocationPoolTypeCustom pool,
        List<InvocationDefinitionCustom> unlearnedSpells,
        bool canUnlearnInvocations)
    {
        foreach (Transform transform in instance.spellsTable)
        {
            if (!transform.gameObject.activeSelf)
            {
                continue;
            }

            var box = transform.GetComponent<SpellBox>();
            var boxFeature = box.GetFeature();
            var isUnlearned = unlearnedSpells != null && unlearnedSpells.Contains(boxFeature);
            var alreadyHas = hero.TrainedInvocations.Contains(boxFeature);
            var canUnlearn = !isUnlearned && alreadyHas;

            PowerBundle.ValidatePrerequisites(hero, boxFeature, boxFeature.Validators, out var requirements);

            box.SetupUI(hero, pool.Sprite, requirements);

            if (canUnlearnInvocations)
            {
                box.RefreshUnlearnInProgress(canUnlearn || isUnlearned, isUnlearned);
            }
            else
            {
                box.RefreshUnlearnInactive(isUnlearned);
            }
        }
    }

    internal static void CustomUnbind(this SpellsByLevelGroup instance)
    {
        instance.SpellRepertoire = null;

        var spellsTable = instance.spellsTable;

        foreach (Component component in spellsTable)
        {
            component.GetComponent<SpellBox>().CustomUnbind();
        }

        Gui.ReleaseChildrenToPool(spellsTable);
        instance.slotStatusTable.Unbind();
    }
}

internal static class SpellBoxExtensions
{
    private static readonly Dictionary<SpellBox, InvocationDefinitionCustom> Features = new();

    internal static InvocationDefinitionCustom GetFeature(this SpellBox box)
    {
        return Features.TryGetValue(box, out var result) ? result : null;
    }

    internal static void CustomFeatureBind(
        this SpellBox instance,
        InvocationDefinitionCustom feature,
        bool unlearned,
        SpellBox.BindMode bindMode,
        SpellBox.SpellBoxChangedHandler spellBoxChanged)
    {
        Features.AddOrReplace(instance, feature);

        instance.bindMode = bindMode;
        instance.SpellBoxChanged = spellBoxChanged;
        instance.hovered = false;
        instance.ritualSpell = false;
        instance.autoPrepared = false;
        instance.extraSpell = false;
        instance.unlearnedSpell = unlearned;
        instance.spellImage.color = Color.white;
        instance.transform.localScale = new Vector3(1f, 1f, 1f);

        var component = instance.availableToLearnGroup.GetComponent<GuiModifier>();

        component.ForwardStartDelay = Random.Range(0.0f, component.Duration);

        instance.prepared = false;
        instance.canLearn = false;
        instance.selectedToLearn = false;
        instance.canPrepare = false;
        instance.known = false;
        instance.canUnlearn = false;

        instance.name = feature.Name;
    }

    internal static void CustomRefreshLearningInProgress(
        this SpellBox instance, bool canLearn, bool selected, bool known)
    {
        var autoPrepared = instance.autoPrepared;

        instance.interactive = canLearn && !autoPrepared;
        instance.canLearn = canLearn && !autoPrepared;
        instance.selectedToLearn = selected;
        instance.selectedToLearn = selected;
        instance.known = known; //TODO: try to experiment with auto prepared tags to signify known features
        instance.Refresh();
    }

    internal static void SetupUI(
        this SpellBox instance, RulesetCharacterHero hero, AssetReferenceSprite sprite, List<string> requirements)
    {
        var title = instance.titleLabel;
        var image = instance.spellImage;
        var tooltip = instance.tooltip;
        var feature = instance.GetFeature();
        var gui = new GuiPresentationBuilder(feature.GuiPresentation).Build();
        var item = feature.Item;
        var dataProvider = item == null
            ? new CustomTooltipProvider(feature, gui)
            : new CustomItemTooltipProvider(feature, gui, item);

        dataProvider.SetPrerequisites(requirements);
        tooltip.TooltipClass = dataProvider.TooltipClass;
        tooltip.Content = feature.GuiPresentation.Description;
        tooltip.Context = hero;
        tooltip.DataProvider = dataProvider;

        if (gui.SpriteReference == null || gui.SpriteReference == GuiPresentationBuilder.EmptySprite)
        {
            gui.spriteReference = sprite;
        }

        title.Text = gui.Title;

        image.SetupSprite(gui.spriteReference);
    }

    internal static void CustomUnbind(this SpellBox instance)
    {
        Features.Remove(instance);
        instance.Unbind();
    }
}

#endregion
