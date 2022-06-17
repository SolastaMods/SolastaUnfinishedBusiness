using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static AttributeDefinitions;
using Random = UnityEngine.Random;

namespace SolastaCommunityExpansion.CustomUI;

public class CustomFeatureSelectionPanel : CharacterStagePanel
{
    private const float ScrollDuration = 0.3f;
    private const float SpellsByLevelMargin = 10.0f;
    private readonly List<FeaturePool> allPools = new();

    private readonly List<(string, FeatureDefinitionFeatureSetCustom)> gainedCustomFeatures = new();
    private readonly Dictionary<PoolId, List<FeatureDefinition>> learnedFeatures = new();

    private readonly Comparison<FeaturePool> poolCompare = (a, b) =>
    {
        var r = String.CompareOrdinal(a.Id.Tag, b.Id.Tag);

        if (r != 0)
        {
            return r;
        }

        if (a.IsReplacer == b.IsReplacer)
        {
            return String.CompareOrdinal(a.Id.Name, b.Id.Name);
        }

        if (a.IsReplacer)
        {
            return -1;
        }

        return 1;
    };

    private int currentLearnStep;
    private int gainedCharacterLevel;
    private CharacterClassDefinition gainedClass;
    private int gainedClassLevel;
    private CharacterSubclassDefinition gainedSubclass;
    private bool wasClicked;

    public override string Name => "CustomFeatureSelection";
    public override string Title => "UI/&CustomFeatureSelectionStageTitle";
    public override string Description => "UI/&CustomFeatureSelectionStageDescription";
    private bool IsFinalStep => currentLearnStep >= allPools.Count;

    internal void Setup(CharacterStageSpellSelectionPanel spells)
    {
        spellsPanel = spells;
        stageDefinition = spells.StageDefinition;

        spellsByLevelTable = spells.spellsByLevelTable;
        spellsByLevelPrefab = spells.spellsByLevelPrefab;
        spellsScrollRect = spells.spellsScrollRect;
        learnStepsTable = spells.learnStepsTable;
        learnStepPrefab = spells.learnStepPrefab;
        backdropReference = spells.backdropReference;
        backdrop = spells.backdrop;
        curve = spells.curve;
        levelButtonsTable = spells.levelButtonsTable;
        levelButtonPrefab = spells.levelButtonPrefab;
        stageTitleLabel = spellsPanel.RectTransform.FindChildRecursive("ChoiceTitle").GetComponent<GuiLabel>();
        righrFeaturesLabel =
            spellsPanel.RectTransform.FindChildRecursive("SpellsInfoTitle").GetComponent<GuiLabel>();
        rightFeaturesDescription = spellsPanel.RectTransform.FindChildRecursive("ProficienciesIntroDescription")
            .GetComponent<GuiLabel>();

        CharacterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
        currentHero = spellsPanel.currentHero;
    }

    public override void SetScrollSensitivity(float scrollSensitivity)
    {
        spellsScrollRect.scrollSensitivity = -scrollSensitivity;
    }

    public override void UpdateRelevance()
    {
        UpdateGrantedFeatures();
        IsRelevant = !gainedCustomFeatures.Empty();
    }

    public override void EnterStage()
    {
        stageTitleLabel.Text = "UI/&CustomFeatureSelectionStageTitle";
        righrFeaturesLabel.Text = "UI/&CustomFeatureSelectionStageFeatures";
        rightFeaturesDescription.Text = "UI/&CustomFeatureSelectionStageDescription";
        currentLearnStep = 0;
        initialized = false;
        CollectTags();
        OnEnterStageDone();
    }

    public override void OnBeginShow(bool instant = false)
    {
        base.OnBeginShow(instant);

        backdrop.sprite = Gui.LoadAssetSync<Sprite>(backdropReference);

        CommonData.CharacterStatsPanel.Show(CharacterStatsPanel.ArmorClassFlag |
                                            CharacterStatsPanel.InitiativeFlag | CharacterStatsPanel.MoveFlag |
                                            CharacterStatsPanel.ProficiencyFlag |
                                            CharacterStatsPanel.HitPointMaxFlag |
                                            CharacterStatsPanel.HitDiceFlag);

        BuildLearnSteps();
        spellsScrollRect.normalizedPosition = Vector2.zero;
        OnPreRefresh();
        RefreshNow();
    }

    public override void OnEndHide()
    {
        learnedFeatures.Clear();
        allPools.Clear();

        for (var i = 0; i < spellsByLevelTable.childCount; i++)
        {
            var child = spellsByLevelTable.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                var group = child.GetComponent<SpellsByLevelGroup>();
                group.CustomUnbind();
            }
        }

        Gui.ReleaseChildrenToPool(spellsByLevelTable);
        Gui.ReleaseChildrenToPool(learnStepsTable);
        Gui.ReleaseChildrenToPool(levelButtonsTable);

        base.OnEndHide();

        if (backdrop.sprite != null)
        {
            Gui.ReleaseAddressableAsset(backdrop.sprite);
            backdrop.sprite = null;
        }
    }

    public override void Refresh()
    {
        var currentPoolIndex = 0;

        for (var i = 0; i < learnStepsTable.childCount; i++)
        {
            var child = learnStepsTable.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            var stepItem = child.GetComponent<LearnStepItem>();

            LearnStepItem.Status status;

            if (i == currentLearnStep)
            {
                status = LearnStepItem.Status.InProgress;
            }
            else if (i == currentLearnStep - 1)
            {
                status = LearnStepItem.Status.Previous;
            }
            else
            {
                status = LearnStepItem.Status.Locked;
            }

            stepItem.CustomRefresh(status, allPools[i]);

            if (status == LearnStepItem.Status.InProgress)
            {
                currentPoolIndex = i;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(learnStepsTable);

        if (IsFinalStep)
        {
            currentPoolIndex = allPools.Count - 1;
        }

        var currentPoolId = allPools[currentPoolIndex].Id;
        var isUnlearnStep = IsUnlearnStep(currentPoolIndex);
        var featurePool = GetPoolById(currentPoolId);
        var allLevels = featurePool.FeatureSet.AllLevels;
        var requiredGroups = allLevels.Count;

        while (spellsByLevelTable.childCount < requiredGroups)
        {
            Gui.GetPrefabFromPool(spellsByLevelPrefab, spellsByLevelTable);
        }

        float totalWidth = 0;
        float lastWidth = 0;
        var layout = spellsByLevelTable.GetComponent<HorizontalLayoutGroup>();
        layout.padding.left = (int)SpellsByLevelMargin;

        for (var i = 0; i < spellsByLevelTable.childCount; i++)
        {
            var child = spellsByLevelTable.GetChild(i);
            child.gameObject.SetActive(i < requiredGroups);

            if (i < requiredGroups)
            {
                var group = child.GetComponent<SpellsByLevelGroup>();
                var featureLevel = allLevels[i];

                var lowLevel = !isUnlearnStep && featureLevel > (featurePool.FeatureSet.RequireClassLevels
                    ? gainedClassLevel
                    : gainedCharacterLevel);

                group.Selected = !IsFinalStep && !lowLevel;

                var levelError = string.Empty;
                if (lowLevel)
                {
                    levelError = featurePool.FeatureSet.RequireClassLevels
                        ? Gui.Format("Requirement/&FeatureSelectionRequireClassLevel", $"{featureLevel}",
                            gainedClass.GuiPresentation.Title)
                        : Gui.Format("Requirement/&FeatureSelectionRequireCharacterLevel", $"{featureLevel}");
                }

                var unlearnedFeatures = isUnlearnStep
                    ? GetOrMakeLearnedList(featurePool.Id)
                    // .Select(f => f is FeatureDefinitionRemover r ? r.FeatureToRemove : f)
                    // .ToList()
                    : GetOrMakeUnlearnedList(featurePool.Id);

                group.CustomFeatureBind(
                    featurePool,
                    GetOrMakeLearnedList(featurePool.Id),
                    featureLevel,
                    levelError,
                    unlearnedFeatures,
                    group.Selected,
                    isUnlearnStep,
                    OnFeatureSelected
                );

                lastWidth = group.RectTransform.rect.width + layout.spacing;
                totalWidth += lastWidth;
            }
        }

        // Compute manually the table width, adding a reserve of fluff for the scrollview
        totalWidth += spellsScrollRect.GetComponent<RectTransform>().rect.width - lastWidth;
        spellsByLevelTable.sizeDelta = new Vector2(totalWidth, spellsByLevelTable.sizeDelta.y);

        // Spell Level Buttons
        while (levelButtonsTable.childCount < requiredGroups)
        {
            Gui.GetPrefabFromPool(levelButtonPrefab, levelButtonsTable);
        }

        // Bind the required group, once for each spell level
        for (var i = 0; i < requiredGroups; i++)
        {
            var child = levelButtonsTable.GetChild(i);
            child.gameObject.SetActive(true);
            var button = child.GetComponent<SpellLevelButton>();
            button.CustomBind(allLevels[i], LevelSelected);
        }

        // Hide remaining useless groups
        for (var i = requiredGroups; i < levelButtonsTable.childCount; i++)
        {
            var child = levelButtonsTable.GetChild(i);
            child.gameObject.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(spellsByLevelTable);

        base.Refresh();
    }

    private List<FeatureDefinition> GetOrMakeLearnedList(PoolId id)
    {
        if (learnedFeatures.ContainsKey(id))
        {
            return learnedFeatures[id];
        }

        var learned = new List<FeatureDefinition>();
        learnedFeatures.Add(id, learned);
        return learned;
    }

    private List<FeatureDefinition> GetOrMakeUnlearnedList(PoolId id)
    {
        var replacerId = allPools.FirstOrDefault(p =>
            p.Id.Tag == id.Tag
            && p.FeatureSet is FeatureDefinitionFeatureSetReplaceCustom r
            && r.ReplacedFeatureSet.Name == id.Name
        )?.Id;

        if (replacerId == null) { return new List<FeatureDefinition>(); }

        return GetOrMakeLearnedList(replacerId)
            .Select(f => f is FeatureDefinitionRemover r ? r.FeatureToRemove : f)
            .ToList();
    }

    private void OnFeatureSelected(SpellBox spellbox)
    {
        if (wasClicked)
        {
            return;
        }

        wasClicked = true;

        var feature = spellbox.GetFeature();
        var pool = allPools[currentLearnStep];
        var learned = GetOrMakeLearnedList(pool.Id);

        if (learned.Contains(feature))
        {
            pool.Used--;
            learned.Remove(feature);

            if (pool.FeatureSet is FeatureDefinitionFeatureSetReplaceCustom replacer)
            {
                var poolById = GetPoolById(new PoolId(replacer.ReplacedFeatureSet.Name, pool.Id.Tag));

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

            if (pool.FeatureSet is FeatureDefinitionFeatureSetReplaceCustom replacer)
            {
                GetOrAddPoolFeatureAndTag(replacer.ReplacedFeatureSet, pool.Id.Tag).Max++;
            }
        }

        GrantAcquiredFeatures(() =>
        {
            CommonData.AbilityScoresListingPanel.RefreshNow();
            CommonData.CharacterStatsPanel.RefreshNow();

            // don't use ? on Unity Objects
            if (CommonData.AttackModesPanel != null)
            {
                CommonData.AttackModesPanel.RefreshNow();
            }

            // don't use ? on Unity Objects
            if (CommonData.PersonalityMapPanel != null)
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

    private Dictionary<string, List<FeatureDefinition>> CollectAcquiredFeatures()
    {
        var result = new Dictionary<string, List<FeatureDefinition>>();

        foreach (var e in learnedFeatures)
        {
            var poolTag = e.Key.Tag;

            if (!result.ContainsKey(poolTag))
            {
                result.Add(poolTag, new List<FeatureDefinition>());
            }

            result[poolTag].AddRange(e.Value);
        }

        return result;
    }

    private List<FeatureDefinition> GetNormalActiveFeatures()
    {
        var parent = gameObject.transform.parent;
        var result = new List<FeatureDefinition>();
        CharacterStageClassSelectionPanel classGains = null;
        CharacterStageLevelGainsPanel levelGains = null;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name.Contains(CharacterStageDefinitions.StageLevelGains))
            {
                levelGains = child.gameObject.GetComponent<CharacterStageLevelGainsPanel>();
            }
            else if (child.name.Contains(CharacterStageDefinitions.StageClassSelection))
            {
                classGains = child.gameObject.GetComponent<CharacterStageClassSelectionPanel>();
            }
        }

        if (levelGains != null)
        {
            result.AddRange(levelGains.activeFeatures);
        }
        else if (classGains != null)
        {
            result.AddRange(classGains.activeFeatures);
        }

        return result;
    }

    private void GrantAcquiredFeatures(Action onDone = null)
    {
        var command = ServiceRepository.GetService<IHeroBuildingCommandService>();
        var acquiredFeatures = CollectAcquiredFeatures();
        var classFeatures = GetNormalActiveFeatures(); //TODO: remove custom feture sets from acitve features

        command.ClearPrevious(currentHero, GetCustomClassTag());
        command.ClearPrevious(currentHero, GetCustomSubClassTag()); // skips cleaning if tag in null or empty

        foreach (var e in acquiredFeatures)
        {
            var currentTag = e.Key;
            var features = e.Value;

            features.RemoveAll(f => f is FeatureDefinitionFeatureSetCustom);
            command.GrantFeatures(currentHero, features, currentTag, false);
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
        foreach (var e in learnedFeatures)
        {
            var id = e.Key;
            var learned = e.Value;
            var pool = GetPoolById(id);

            for (var i = 0; i < learned.Count;)
            {
                List<string> q = new();
                if (learned[i] is IFeatureDefinitionWithPrerequisites feature
                    && !CustomFeaturesContext.GetValidationErrors(feature.Validators, out q))
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

    public override bool CanProceedToNextStage(out string failureString)
    {
        if (!IsFinalStep
            || !initialized
            || (!allPools.Empty() && allPools[allPools.Count - 1].Remaining > 0)
           )
        {
            failureString = Gui.Localize("UI/&CustomFeatureSelectionStageNotDone");
            return false;
        }

        failureString = string.Empty;
        return true;
    }

    public void MoveToNextLearnStep()
    {
        currentLearnStep++;

        while (!IsFinalStep && allPools[currentLearnStep].Remaining == 0)
        {
            currentLearnStep++;
        }

        LevelSelected(0);
        OnPreRefresh();
        RefreshNow();
    }

    public void MoveToPreviousLearnStep(bool refresh = true, Action onDone = null)
    {
        var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

        if (currentLearnStep > 0)
        {
            if (!IsFinalStep)
            {
                ResetLearnings(currentLearnStep);
            }

            currentLearnStep--;
            ResetLearnings(currentLearnStep);
            if (IsUnlearnStep(currentLearnStep))
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

    private bool IsUnlearnStep(int step)
    {
        return allPools[step].IsReplacer;
    }

    private void UpdateGanedClassDetails()
    {
        // Determine the last class and level
        CharacterBuildingService.GetLastAssignedClassAndLevel(currentHero, out gainedClass, out gainedClassLevel);
        gainedCharacterLevel = currentHero.GetAttribute(CharacterLevel).CurrentValue;

        if (gainedClass == null)
        {
            return;
        }

        // Was there already a subclass?
        gainedSubclass = null;

        if (currentHero.ClassesAndSubclasses.ContainsKey(gainedClass))
        {
            gainedSubclass = currentHero.ClassesAndSubclasses[gainedClass];
        }
    }

    private string GetCustomClassTag()
    {
        return CustomFeaturesContext.CustomizeTag(GetClassTag(gainedClass, gainedClassLevel));
    }

    private string GetCustomSubClassTag()
    {
        if (gainedSubclass == null)
        {
            return null;
        }

        return CustomFeaturesContext.CustomizeTag(GetSubclassTag(gainedClass, gainedClassLevel, gainedSubclass));
    }

    private void UpdateGrantedFeatures()
    {
        UpdateGanedClassDetails();

        gainedCustomFeatures.Clear();

        if (gainedClass == null)
        {
            return;
        }

        var poolTag = GetCustomClassTag();

        gainedCustomFeatures.AddRange(gainedClass.FeatureUnlocks
            .Where(f => f.Level == gainedClassLevel)
            .Select(f => f.FeatureDefinition as FeatureDefinitionFeatureSetCustom)
            .Where(f => f != null)
            .Select(f => (poolTag, f))
        );

        poolTag = GetCustomSubClassTag();

        if (poolTag != null)
        {
            gainedCustomFeatures.AddRange(gainedSubclass.FeatureUnlocks
                .Where(f => f.Level == gainedClassLevel)
                .Select(f => f.FeatureDefinition as FeatureDefinitionFeatureSetCustom)
                .Where(f => f != null)
                .Select(f => (poolTag, f))
            );
        }
    }

    private void CollectTags()
    {
        Dictionary<PoolId, FeaturePool> tags = new();

        UpdateGrantedFeatures();
        allPools.Clear();

        foreach (var (poolTag, featureSet) in gainedCustomFeatures)
        {
            var poolId = new PoolId(featureSet.Name, poolTag);

            if (!tags.ContainsKey(poolId))
            {
                var pool = new FeaturePool(poolId) {Max = 1, Used = 0, FeatureSet = featureSet};
                tags.Add(poolId, pool);
                allPools.Add(pool);
            }
            else
            {
                tags[poolId].Max++;
            }
        }

        allPools.Sort(poolCompare);

        initialized = true;
    }

    private FeaturePool GetPoolById(PoolId id)
    {
        return allPools.FirstOrDefault(p => p.Id.Equals(id));
    }


    private FeaturePool GetOrAddPoolFeatureAndTag(FeatureDefinitionFeatureSetCustom set, string featureTag)
    {
        return GetOrAddPoolById(new PoolId(set.Name, featureTag), set);
    }

    private FeaturePool GetOrAddPoolById(PoolId id, FeatureDefinitionFeatureSetCustom set)
    {
        var pool = GetPoolById(id);

        if (pool == null)
        {
            pool = new FeaturePool(id) {FeatureSet = set, Max = 0, Used = 0};
            allPools.Add(pool);
            allPools.Sort(poolCompare);
            BuildLearnSteps();
        }

        return pool;
    }

    private void BuildLearnSteps()
    {
        // Register all steps
        if (allPools != null && allPools.Count > 0)
        {
            while (learnStepsTable.childCount < allPools.Count)
            {
                Gui.GetPrefabFromPool(learnStepPrefab, learnStepsTable);
            }

            for (var i = 0; i < learnStepsTable.childCount; i++)
            {
                var child = learnStepsTable.GetChild(i);

                if (i < allPools.Count)
                {
                    var learnStepItem = child.GetComponent<LearnStepItem>();

                    child.gameObject.SetActive(true);
                    learnStepItem.CustomBind(i, allPools[i],
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
    }

    public override void CancelStage()
    {
        initialized = false;

        while (IsFinalStep)
        {
            currentLearnStep--;
        }

        for (var i = currentLearnStep; i >= 0; i--)
        {
            ResetLearnings(i);
        }

        var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

        heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(OnCancelStageDone);
    }

    public void OnLearnBack()
    {
        if (wasClicked)
        {
            return;
        }

        wasClicked = true;

        MoveToPreviousLearnStep(true, ResetWasClickedFlag);
    }

    public void OnLearnReset()
    {
        if (wasClicked)
        {
            return;
        }

        wasClicked = true;

        if (IsFinalStep)
        {
            currentLearnStep = allPools.Count - 1;
        }

        ResetLearnings(currentLearnStep, () =>
        {
            OnPreRefresh();
            RefreshNow();
            ResetWasClickedFlag();
        });
    }

    public void OnSkipRemaining()
    {
        if (wasClicked)
        {
            return;
        }

        wasClicked = true;

        if (IsUnlearnStep(currentLearnStep))
        {
            allPools[currentLearnStep].Skipped = true;
            MoveToNextLearnStep();
        }

        ResetWasClickedFlag();
    }

    private void ResetLearnings(int stepNumber, Action onDone = null)
    {
        var pool = allPools[stepNumber];

        pool.Used = 0;
        pool.Skipped = false;
        GetOrMakeLearnedList(pool.Id).Clear();

        GrantAcquiredFeatures(onDone);
    }

    public class FeaturePool
    {
        public bool Skipped;
        public FeaturePool(PoolId id) { Id = id; }
        public PoolId Id { get; }
        public int Max { get; set; }
        public int Used { get; set; }
        public int Remaining => Skipped ? 0 : Max - Used;
        public FeatureDefinitionFeatureSetCustom FeatureSet { get; set; }
        public bool IsReplacer => FeatureSet is FeatureDefinitionFeatureSetReplaceCustom;
    }

    public class PoolId
    {
        public PoolId(string name, string tag)
        {
            Name = name;
            Tag = tag;
        }

        public string Name { get; }
        public string Tag { get; }

        public override bool Equals(object obj)
        {
            if (obj is not PoolId pool)
            {
                return false;
            }

            return Name == pool.Name && Tag == pool.Tag;
        }

        public override int GetHashCode()
        {
            return $"{Name}[{Tag}]".GetHashCode();
        }
    }

    #region Fields from CharacterStageSpellSelectionPanel

    private CharacterStageSpellSelectionPanel spellsPanel;

    private RectTransform spellsByLevelTable;
    private GameObject spellsByLevelPrefab;
    private ScrollRect spellsScrollRect;
    private RectTransform learnStepsTable;
    private GameObject learnStepPrefab;
    private AssetReferenceSprite backdropReference;
    private Image backdrop;
    private AnimationCurve curve;
    private RectTransform levelButtonsTable;
    private GameObject levelButtonPrefab;
    private GuiLabel stageTitleLabel;
    private GuiLabel righrFeaturesLabel;
    private GuiLabel rightFeaturesDescription;

    #endregion

    #region UI helpers

    private void ResetWasClickedFlag()
    {
        wasClicked = false;
    }

    public void LevelSelected(int level)
    {
        StartCoroutine(BlendToLevelGroup(level));
    }

    private IEnumerator BlendToLevelGroup(int level)
    {
        var duration = ScrollDuration;
        var group = spellsByLevelTable.GetChild(0).GetComponent<SpellsByLevelGroup>();

        foreach (Transform child in spellsByLevelTable)
        {
            var spellByLevelGroup = child.GetComponent<SpellsByLevelGroup>();

            if (spellByLevelGroup.SpellLevel == level)
            {
                group = spellByLevelGroup;
            }
        }

        var initialX = spellsByLevelTable.anchoredPosition.x;
        var finalX = -group.RectTransform.anchoredPosition.x + SpellsByLevelMargin;

        while (duration > 0)
        {
            spellsByLevelTable.anchoredPosition = new Vector2(
                Mathf.Lerp(initialX, finalX, curve.Evaluate((ScrollDuration - duration) / ScrollDuration)), 0);
            duration -= Gui.SystemDeltaTime;
            yield return null;
        }

        spellsByLevelTable.anchoredPosition = new Vector2(finalX, 0);
    }

    #endregion
}

internal static class LearnStepItemExtension
{
    public static void CustomBind(
        this LearnStepItem instance,
        int rank,
        CustomFeatureSelectionPanel.FeaturePool pool,
        LearnStepItem.ButtonActivatedHandler onBackOneStepActivated,
        LearnStepItem.ButtonActivatedHandler onResetActivated,
        LearnStepItem.ButtonActivatedHandler onAutoSelectActivated)
    {
        instance.Tag = pool.Id.Tag;
        instance.PoolType = pool.IsReplacer
            ? HeroDefinitions.PointsPoolType.SpellUnlearn
            : HeroDefinitions.PointsPoolType.Irrelevant;
        instance.rank = rank;
        instance.ignoreAvailable = pool.IsReplacer;
        instance.autoLearnAvailable = false;
        var header = pool.FeatureSet.FormatTitle();
        instance.headerLabelActive.Text = header;
        instance.headerLabelInactive.Text = header;
        instance.OnBackOneStepActivated = onBackOneStepActivated;
        instance.OnResetActivated = onResetActivated;
        instance.OnAutoSelectActivated = onAutoSelectActivated;
    }

    public static void CustomRefresh(
        this LearnStepItem instance,
        LearnStepItem.Status status,
        CustomFeatureSelectionPanel.FeaturePool pool)
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
            choiceLabel.Text = pool.FeatureSet.FormatDescription();
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
    public static void CustomBind(
        this SpellLevelButton instance,
        int level,
        SpellLevelButton.LevelSelectedHandler levelSelected)
    {
        instance.Level = level;
        instance.LevelSelected = levelSelected;
        instance.label.Text = $"{level}";
    }
}

internal static class SpellsByLevelGroupExtensions
{
    public static RectTransform GetSpellsTable(this SpellsByLevelGroup instance)
    {
        return instance.spellsTable;
    }

    public static void CustomFeatureBind(
        this SpellsByLevelGroup instance,
        CustomFeatureSelectionPanel.FeaturePool pool,
        List<FeatureDefinition> learned,
        int featureLevel,
        string lowLevelError,
        List<FeatureDefinition> unlearned,
        bool canAcquireFeatures,
        bool unlearn,
        SpellBox.SpellBoxChangedHandler spellBoxChanged)
    {
        var featureSet = pool.FeatureSet;

        instance.name = $"Feature[{featureSet.Name}]";
        instance.SpellLevel = featureLevel;

        var allFeatures = featureSet.GetLevelFeatures(featureLevel);

        allFeatures.Sort((a, b) => string.CompareOrdinal(a.FormatTitle(), b.FormatTitle()));

        var spellsTable = instance.GetSpellsTable();
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

            // box.Bind(guiSpellDefinition1, null, false, null, isUnlearned, bindMode, spellBoxChanged);
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

        instance.slotStatusTable.Bind(null, featureLevel, null, false);

        if (unlearn)
        {
            instance.RefreshUnlearning(pool, unlearned, canAcquireFeatures);
        }
        else
        {
            instance.RefreshLearning(pool, learned, lowLevelError, unlearned, canAcquireFeatures);
        }
    }

    public static void RefreshLearning(
        this SpellsByLevelGroup instance,
        CustomFeatureSelectionPanel.FeaturePool pool,
        List<FeatureDefinition> learned,
        string lowLevelError,
        List<FeatureDefinition> unlearnedFetures,
        bool canAcquireFeatures)
    {
        foreach (Transform transform in instance.GetSpellsTable())
        {
            if (transform.gameObject.activeSelf)
            {
                var box = transform.GetComponent<SpellBox>();
                var boxFeature = box.GetFeature();
                var alreadyHas = Global.ActiveLevelUpHeroHasFeature(boxFeature);
                var selected = learned.Contains(boxFeature);
                var isUnlearned = unlearnedFetures != null && unlearnedFetures.Contains(boxFeature);
                var errors = new List<string>();
                var canLearn =
                    !selected
                    && !alreadyHas
                    && (boxFeature is not IFeatureDefinitionWithPrerequisites prerequisites
                        || CustomFeaturesContext.GetValidationErrors(prerequisites.Validators, out errors));


                if (!string.IsNullOrEmpty(lowLevelError))
                {
                    errors.Add(lowLevelError);
                }

                box.SetupUI(pool.FeatureSet.GuiPresentation, errors);

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
    }

    public static void RefreshUnlearning(
        this SpellsByLevelGroup instance,
        CustomFeatureSelectionPanel.FeaturePool pool,
        List<FeatureDefinition> unlearnedSpells,
        bool canUnlearnSpells)
    {
        foreach (Transform transform in instance.GetSpellsTable())
        {
            if (transform.gameObject.activeSelf)
            {
                var box = transform.GetComponent<SpellBox>();
                var boxFeature = box.GetFeature();
                var removerFeature = boxFeature as FeatureDefinitionRemover;
                var isUnlearned = unlearnedSpells != null && unlearnedSpells.Contains(boxFeature);
                var canUnlearn =
                    Global.ActiveLevelUpHeroHasFeature(removerFeature != null
                        ? removerFeature.FeatureToRemove
                        : boxFeature)
                    && !isUnlearned;

                box.SetupUI(pool.FeatureSet.GuiPresentation, null);

                if (canUnlearnSpells)
                {
                    box.RefreshUnlearnInProgress(canUnlearn || isUnlearned, isUnlearned);
                }
                else
                {
                    box.RefreshUnlearnInactive(isUnlearned);
                }
            }
        }
    }

    public static void CustomUnbind(this SpellsByLevelGroup instance)
    {
        instance.SpellRepertoire = null;

        var spellsTable = instance.GetSpellsTable();

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
    private static readonly Dictionary<SpellBox, FeatureDefinition> Features = new();

    public static FeatureDefinition GetFeature(this SpellBox box)
    {
        return Features.GetValueOrDefault(box);
    }

    public static void CustomFeatureBind(
        this SpellBox instance,
        FeatureDefinition feature,
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

    public static void CustomRefreshLearningInProgress(this SpellBox instance, bool canLearn, bool selected,
        bool known)
    {
        var autoPrepared = instance.autoPrepared;

        instance.interactive = canLearn && !autoPrepared;
        instance.canLearn = canLearn && !autoPrepared;
        instance.selectedToLearn = selected;
        instance.selectedToLearn = selected;
        instance.known = known; //TODO: try to experiment with auto prepared tags to signify known features
        instance.Refresh();
    }

    public static void SetupUI(this SpellBox instance, GuiPresentation setPresentation, List<string> errors)
    {
        var title = instance.titleLabel;
        var image = instance.spellImage;
        var tooltip = instance.tooltip;
        var feature = instance.GetFeature();
        var gui = new GuiPresentationBuilder(feature.GuiPresentation).Build();
        var hasErrors = errors != null && !errors.Empty();

        if (!hasErrors && feature is FeatureDefinitionPower power)
        {
            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiPowerDefinition(power.Name)
                .SetupTooltip(tooltip);
        }
        else if (!hasErrors && feature is FeatureDefinitionBonusCantrips cantrips &&
                 cantrips.BonusCantrips.Count == 1)
        {
            ServiceRepository.GetService<IGuiWrapperService>()
                .GetGuiSpellDefinition(cantrips.BonusCantrips[0].Name)
                .SetupTooltip(tooltip, Global.ActiveLevelUpHero);
        }
        else
        {
            var dataProvider = new CustomTooltipProvider(feature, gui);

            dataProvider.SetPrerequisites(errors);
            tooltip.TooltipClass = "FeatDefinition";
            tooltip.Content = feature.GuiPresentation.Description;
            tooltip.Context = Global.ActiveLevelUpHero;
            tooltip.DataProvider = dataProvider;
        }

        if (gui.SpriteReference == null || gui.SpriteReference == GuiPresentationBuilder.EmptySprite)
        {
            gui.spriteReference = setPresentation.SpriteReference;
        }

        title.Text = gui.Title;
        image.SetupSprite(gui, true);
    }

    public static void CustomUnbind(this SpellBox instance)
    {
        Features.Remove(instance);
        instance.Unbind();
    }
}
