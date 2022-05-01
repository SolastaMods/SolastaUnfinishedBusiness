using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static AttributeDefinitions;
using Random = UnityEngine.Random;

namespace SolastaCommunityExpansion.CustomUI
{
    public class CustomFeatureSelectionPanel : CharacterStagePanel
    {
        private static readonly Dictionary<string, CustomFeatureSelectionPanel> _instances = new ();

        public static CharacterStagePanel Get(GameObject[] prefabs, CharacterEditionScreen editor)
        {
            var container = editor.StagesPanelContainer;
            var editorType = editor.gameObject.name;
            var instance = _instances.GetValueOrDefault(editorType);
            
            if (instance == null)
            {
                //TODO: make calculatng which prefab is spell slection more robust
                var gameObject = Gui.GetPrefabFromPool(prefabs[8], container);//create copy of spell selection
                var spells = gameObject.GetComponent<CharacterStageSpellSelectionPanel>();
                instance = gameObject.AddComponent<CustomFeatureSelectionPanel>();
                instance.Setup(gameObject, spells, prefabs);
                _instances.Add(editorType, instance);
            }
            else if(instance.gameObject.transform.parent != container)
            {
                instance.gameObject.transform.SetParent(container, false);
                instance.gameObject.SetActive(true);
            }

            return instance;
        }

        #region Fields from CharacterStageSpellSelectionPanel

        private CharacterStageSpellSelectionPanel spellsPanel; //TODO: do we need it?

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

        private void Setup(GameObject o, CharacterStageSpellSelectionPanel spells, GameObject[] prefabs)
        {
            spellsPanel = spells;
            this.SetField("stageDefinition", spells.StageDefinition);

            spellsByLevelTable = spells.GetField<RectTransform>("spellsByLevelTable");
            spellsByLevelPrefab = spells.GetField<GameObject>("spellsByLevelPrefab");
            spellsScrollRect = spells.GetField<ScrollRect>("spellsScrollRect");
            learnStepsTable = spells.GetField<RectTransform>("learnStepsTable");
            learnStepPrefab = spells.GetField<GameObject>("learnStepPrefab");
            backdropReference = spells.GetField<AssetReferenceSprite>("backdropReference");
            backdrop = spells.GetField<Image>("backdrop");
            curve = spells.GetField<AnimationCurve>("curve");
            levelButtonsTable = spells.GetField<RectTransform>("levelButtonsTable");
            levelButtonPrefab = spells.GetField<GameObject>("levelButtonPrefab");
            stageTitleLabel = spellsPanel.RectTransform.FindChildRecursive("ChoiceTitle").GetComponent<GuiLabel>();
            righrFeaturesLabel = spellsPanel.RectTransform.FindChildRecursive("SpellsInfoTitle").GetComponent<GuiLabel>();
            rightFeaturesDescription = spellsPanel.RectTransform.FindChildRecursive("ProficienciesIntroDescription").GetComponent<GuiLabel>();

            CharacterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            currentHero = spellsPanel.GetField<RulesetCharacterHero>("currentHero");
        }

        private const float ScrollDuration = 0.3f;
        private const float SpellsByLevelMargin = 10.0f;

        //TODO: add proper translation strings
        public override string Name => "CustomFeatureSelection";
        public override string Title => "UI/&CustomFeatureSelectionStageTitle";
        public override string Description => "UI/&CustomFeatureSelectionStageDescription";
        private bool IsFinalStep => this.currentLearnStep >= this.allPools.Count;

        private bool initialized = false;
        private int gainedClassLevel;
        private int gainedCharacterLevel;
        private CharacterClassDefinition gainedClass;
        private CharacterSubclassDefinition gainedSubclass;

        private int currentLearnStep;
        private List<FeaturePool> allPools = new ();
        private Dictionary<PoolId, List<FeatureDefinition>> learnedFeatures = new ();
        private bool wasClicked;

        private readonly Comparison<FeaturePool> poolCompare = (a, b) =>
        {
            var r = String.CompareOrdinal(a.Id.Tag, b.Id.Tag);
            if (r != 0) { return r; }

            if (a.IsReplacer == b.IsReplacer)
            {
                return String.CompareOrdinal(a.Id.Name, b.Id.Name);
            }
            else if (a.IsReplacer)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        };

        public class FeaturePool
        {
            public PoolId Id { get; }
            public int Max { get; set; }
            public int Used { get; set; }
            public int Remaining => Max - Used;
            public CustomFeatureDefinitionSet FeatureSet { get; set; }
            public bool IsReplacer => FeatureSet is ReplaceCustomFeatureDefinitionSet;
            public FeaturePool(PoolId id) { Id = id; }
        }

        public class PoolId
        {
            public PoolId(string name, string tag)
            {
                Name = name;
                Tag = tag;
            }

            public string Name { get;}
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

        private readonly List<(string, CustomFeatureDefinitionSet)> gainedCustomFeatures = new();

        public override void SetScrollSensitivity(float scrollSensitivity)
        {
            this.spellsScrollRect.scrollSensitivity = -scrollSensitivity;
        }

        public override IEnumerator Load()
        {
            Main.Log($"[ENDER] CUSTOM Load");

            yield return base.Load();
            IRuntimeService runtimeService = ServiceRepository.GetService<IRuntimeService>();
            runtimeService.RuntimeLoaded += this.RuntimeLoaded;
        }

        public override IEnumerator Unload()
        {
            Main.Log($"[ENDER] CUSTOM Unload");

            IRuntimeService runtimeService = ServiceRepository.GetService<IRuntimeService>();
            runtimeService.RuntimeLoaded -= this.RuntimeLoaded;

            // this.allCantrips.Clear();
            // this.allSpells.Clear();
            yield return base.Unload();
        }

        private void RuntimeLoaded(Runtime runtime)
        {
            //TODO: collect any relevant info we need
            // SpellDefinition[] allSpellDefinitions = DatabaseRepository.GetDatabase<SpellDefinition>().GetAllElements();
            // this.allCantrips = new List<SpellDefinition>();
            // this.allSpells = new Dictionary<int, List<SpellDefinition>>();
            //
            // foreach (SpellDefinition spellDefinition in allSpellDefinitions)
            // {
            //     if (spellDefinition.SpellLevel == 0)
            //     {
            //         this.allCantrips.Add(spellDefinition);
            //     }
            //     else
            //     {
            //         if (!this.allSpells.ContainsKey(spellDefinition.SpellLevel))
            //         {
            //             this.allSpells.Add(spellDefinition.SpellLevel, new List<SpellDefinition>());
            //         }
            //
            //         this.allSpells[spellDefinition.SpellLevel].Add(spellDefinition);
            //     }
            // }
        }

        public override void UpdateRelevance()
        {
            UpdateGrantedFeatures();
            this.IsRelevant = !gainedCustomFeatures.Empty();
        }

        public override void EnterStage()
        {
            Main.Log($"[ENDER] CUSTOM EnterStage '{this.StageDefinition}'");

            stageTitleLabel.Text = "UI/&CustomFeatureSelectionStageTitle";
            righrFeaturesLabel.Text = "UI/&CustomFeatureSelectionStageFeatures";
            rightFeaturesDescription.Text = "UI/&CustomFeatureSelectionStageDescription";
            
            this.currentLearnStep = 0;
            initialized = false;
            this.CollectTags();

            this.OnEnterStageDone();
        }

        protected override void OnBeginShow(bool instant = false)
        {
            base.OnBeginShow(instant);

            this.backdrop.sprite = Gui.LoadAssetSync<Sprite>(this.backdropReference);

            this.CommonData.CharacterStatsPanel.Show(CharacterStatsPanel.ArmorClassFlag |
                                                     CharacterStatsPanel.InitiativeFlag | CharacterStatsPanel.MoveFlag |
                                                     CharacterStatsPanel.ProficiencyFlag |
                                                     CharacterStatsPanel.HitPointMaxFlag |
                                                     CharacterStatsPanel.HitDiceFlag);

            this.BuildLearnSteps();
            this.spellsScrollRect.normalizedPosition = Vector2.zero;

            this.OnPreRefresh();
            this.RefreshNow();
        }

        protected override void OnEndHide()
        {
            Main.Log($"[ENDER] OnEndHide");
            //TODO: clear all pools/learnings
            for (int i = 0; i < this.spellsByLevelTable.childCount; i++)
            {
                Transform child = this.spellsByLevelTable.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    SpellsByLevelGroup group = child.GetComponent<SpellsByLevelGroup>();
                    group.CustomUnbind();
                }
            }

            Gui.ReleaseChildrenToPool(this.spellsByLevelTable);
            Gui.ReleaseChildrenToPool(this.learnStepsTable);
            Gui.ReleaseChildrenToPool(this.levelButtonsTable);

            base.OnEndHide();

            if (this.backdrop.sprite != null)
            {
                Gui.ReleaseAddressableAsset(this.backdrop.sprite);
                this.backdrop.sprite = null;
            }
        }

        protected override void Refresh()
        {
            int currentPoolIndex = 0;
            for (int i = 0; i < this.learnStepsTable.childCount; i++)
            {
                Transform child = this.learnStepsTable.GetChild(i);

                if (!child.gameObject.activeSelf)
                {
                    continue;
                }

                LearnStepItem stepItem = child.GetComponent<LearnStepItem>();

                LearnStepItem.Status status;
                if (i == this.currentLearnStep)
                {
                    status = LearnStepItem.Status.InProgress;
                }
                else if (i == this.currentLearnStep - 1)
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

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.learnStepsTable);

            if (this.IsFinalStep)
            {
                currentPoolIndex = this.allPools.Count - 1;
            }
            PoolId currentPoolId = allPools[currentPoolIndex].Id;
            var isUnlearnStep = IsUnlearnStep(currentPoolIndex);

            
            var featurePool = GetPoolById(currentPoolId);
            var allLevels = featurePool.FeatureSet.AllLevels;
            int requiredGroups = allLevels.Count;
            
            while (this.spellsByLevelTable.childCount < requiredGroups)
            {
                Gui.GetPrefabFromPool(this.spellsByLevelPrefab, this.spellsByLevelTable);
            }

            float totalWidth = 0;
            float lastWidth = 0;
            HorizontalLayoutGroup layout = this.spellsByLevelTable.GetComponent<HorizontalLayoutGroup>();
            layout.padding.left = (int)SpellsByLevelMargin;
            
            for (int i = 0; i < this.spellsByLevelTable.childCount; i++)
            {
                Transform child = this.spellsByLevelTable.GetChild(i);
                child.gameObject.SetActive(i < requiredGroups);
                if (i < requiredGroups)
                {
                    var group = child.GetComponent<SpellsByLevelGroup>();
                    int featureLevel = allLevels[i];

                    var lowLevel = !isUnlearnStep && featureLevel > (featurePool.FeatureSet.RequireClassLevels
                        ? gainedClassLevel
                        : gainedCharacterLevel);
                    
                    group.Selected = !IsFinalStep && !lowLevel;

                    string levelError = null;
                    if (lowLevel)
                    {
                        levelError = featurePool.FeatureSet.RequireClassLevels
                            ? Gui.Format("Requirement/&FeatureSelectionRequireClassLevel", $"{featureLevel}", gainedClass.GuiPresentation.Title)
                            : Gui.Format("Requirement/&FeatureSelectionRequireCharacterLevel", $"{featureLevel}");
                    }

                    List<FeatureDefinition> unlearnedFeatures = isUnlearnStep
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
                        this.OnFeatureSelected
                    );

                    lastWidth = group.RectTransform.rect.width + layout.spacing;
                    totalWidth += lastWidth;
                }
            }

            // Compute manually the table width, adding a reserve of fluff for the scrollview
            totalWidth += this.spellsScrollRect.GetComponent<RectTransform>().rect.width - lastWidth;
            this.spellsByLevelTable.sizeDelta = new Vector2(totalWidth, this.spellsByLevelTable.sizeDelta.y);

            // Spell Level Buttons
            while (this.levelButtonsTable.childCount < requiredGroups)
            {
                Gui.GetPrefabFromPool(this.levelButtonPrefab, this.levelButtonsTable);
            }

            // Bind the required group, once for each spell level
            for (int i = 0; i < requiredGroups; i++)
            {
                Transform child = this.levelButtonsTable.GetChild(i);
                child.gameObject.SetActive(true);
                var button = child.GetComponent<SpellLevelButton>();
                button.CustomBind(allLevels[i], this.LevelSelected);
            }

            // Hide remaining useless groups
            for (int i = requiredGroups; i < this.levelButtonsTable.childCount; i++)
            {
                Transform child = this.levelButtonsTable.GetChild(i);
                child.gameObject.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(this.spellsByLevelTable);

            base.Refresh();
        }

        private List<FeatureDefinition> GetOrMakeLearnedList(PoolId id)
        {
            if (learnedFeatures.ContainsKey(id))
            {
                return learnedFeatures[id];
            }
            else
            {
                var learned = new List<FeatureDefinition>();
                learnedFeatures.Add(id, learned);
                return learned;
            }
        }
        
        private List<FeatureDefinition> GetOrMakeUnlearnedList(PoolId id)
        {
            var replacerId = allPools.FirstOrDefault(p =>
                p.Id.Tag == id.Tag
                && p.FeatureSet is ReplaceCustomFeatureDefinitionSet r
                && r.ReplacedFeatureSet.Name == id.Name
            )?.Id;

            if (replacerId == null) { return new List<FeatureDefinition>(); }

            return GetOrMakeLearnedList(replacerId)
                .Select(f => f is FeatureDefinitionRemover r ? r.FeatureToRemove : f)
                .ToList();
        }

        private void OnFeatureSelected(SpellBox spellbox)
        {
            if (this.wasClicked)
                return;

            this.wasClicked = true;

            var feature = spellbox.GetFeature();
            var pool = allPools[currentLearnStep];
            var learned = GetOrMakeLearnedList(pool.Id);
            Main.Log($"[ENDER] OnFeatureSelected '{feature.Name}', add: {!learned.Contains(feature)}");

            if (learned.Contains(feature))
            {
                pool.Used--;
                learned.Remove(feature);
                if (pool.FeatureSet is ReplaceCustomFeatureDefinitionSet replacer)
                {
                    GetPoolById(new PoolId(replacer.ReplacedFeatureSet.Name, pool.Id.Tag)).Max--;
                }
            }
            else
            {
                pool.Used++;
                learned.Add(feature);
                if (pool.FeatureSet is ReplaceCustomFeatureDefinitionSet replacer)
                {
                    GetPoolById(new PoolId(replacer.ReplacedFeatureSet.Name, pool.Id.Tag)).Max++;
                }
            }

            GrantAcquiredFeatures(() =>
            {
                this.CommonData.AbilityScoresListingPanel.RefreshNow();
                this.CommonData.CharacterStatsPanel.RefreshNow();
                this.CommonData.AttackModesPanel?.RefreshNow();
                this.CommonData.PersonalityMapPanel?.RefreshNow();
                
                this.OnPreRefresh();
                this.RefreshNow();

                if (pool.Remaining == 0)
                {
                    this.MoveToNextLearnStep();
                }

                this.ResetWasClickedFlag();
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
            CharacterStagePanel classGains = null, levelGains = null;
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
                result.AddRange(levelGains.GetField<List<FeatureDefinition>>("activeFeatures"));
            }
            else if (classGains != null)
            {
                result.AddRange(classGains.GetField<List<FeatureDefinition>>("activeFeatures"));
            }
            return result;
        }

        private void GrantAcquiredFeatures(Action onDone = null)
        {
            var command = ServiceRepository.GetService<IHeroBuildingCommandService>();
            var acquiredFeatures = CollectAcquiredFeatures();
            var classFeatures = GetNormalActiveFeatures();
            var classTag = GetClassTag(gainedClass, gainedClassLevel);

            Main.Log($"[ENDER] GrantAcquiredFeatures - cleaning, acquired: {acquiredFeatures.Count}, class: {classFeatures.Count}");
            command.ClearPrevious(currentHero, classTag);
            if (gainedSubclass != null)
            {
                command.ClearPrevious(currentHero, GetSubclassTag(gainedClass, gainedClassLevel, gainedSubclass));
            }
            
            foreach (var e in acquiredFeatures)
            {
                var currentTag = e.Key;
                var features = e.Value;
                if (currentTag == classTag)
                {
                    classFeatures.AddRange(features);
                    features = classFeatures;
                }

                Main.Log($"[ENDER] GrantAcquiredFeatures tag: [{currentTag}] features: {features.Count}");
                features.RemoveAll(f => f is CustomFeatureDefinitionSet);
                // GrantFeatures behaves weirdly - if it encounters spellcasting definition, it stops
                // So we separate spellcasting from other features and then grant them in sequence
                var spellcasting = features.Where(f => f is FeatureDefinitionCastSpell).ToList();
                var other = features.Where(f => f is not FeatureDefinitionCastSpell).ToList();
                command.GrantFeatures(currentHero, spellcasting, currentTag, false);
                command.GrantFeatures(currentHero, other, currentTag, false);
            }

            command.RefreshHero(currentHero);
            command.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
            {
                RemoveInvalidFeatures(onDone);
            });
        }

        private void RemoveInvalidFeatures(Action onDone = null)
        {
            bool dirty = false;
            foreach (var e in learnedFeatures)
            {
                var id = e.Key;
                var learned = e.Value;
                var pool = GetPoolById(id);
                for (var i = 0; i < learned.Count;)
                {
                    List<string> q = new ();
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
            failureString = string.Empty;

            if (!this.IsFinalStep 
                || !initialized 
                || (!allPools.Empty() && allPools[allPools.Count - 1].Remaining > 0)
            )
            {
                //TODO: pick better text
                failureString = Gui.Localize("Stage/&SpellSelectionStageFailLearnSpellsDescription");
                return false;
            }

            failureString = "";
            return true;
        }

        public void MoveToNextLearnStep()
        {
            this.currentLearnStep++;

            this.LevelSelected(0);

            this.OnPreRefresh();
            this.RefreshNow();
        }

        public void MoveToPreviousLearnStep(bool refresh = true, Action onDone = null)
        {
            var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

            if (this.currentLearnStep > 0)
            {
                if (!this.IsFinalStep)
                {
                    this.ResetLearnings(this.currentLearnStep);
                }

                this.currentLearnStep--;
                this.ResetLearnings(this.currentLearnStep);
                if (this.IsUnlearnStep(this.currentLearnStep))
                {
                    heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
                    {
                        this.CollectTags();
                        this.BuildLearnSteps();
                    });
                }
            }

            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
            {
                this.LevelSelected(0);
                this.OnPreRefresh();
                this.RefreshNow();
                this.ResetWasClickedFlag();
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
            
            if (gainedClass == null) { return; }
            // Was there already a subclass?
            gainedSubclass = null;
            if (currentHero.ClassesAndSubclasses.ContainsKey(gainedClass))
            {
                gainedSubclass = currentHero.ClassesAndSubclasses[gainedClass];
            }
        }

        private void UpdateGrantedFeatures()
        {
            UpdateGanedClassDetails();
            
            gainedCustomFeatures.Clear();

            if (gainedClass == null) { return; }

            var poolTag = GetClassTag(gainedClass, gainedClassLevel);

            gainedCustomFeatures.AddRange(gainedClass.FeatureUnlocks
                .Where(f => f.Level == gainedClassLevel)
                .Select(f => f.FeatureDefinition as CustomFeatureDefinitionSet)
                .Where(f => f != null)
                .Select(f => (poolTag, f))
            );

            if (gainedSubclass != null)
            {
                poolTag =  GetSubclassTag(gainedClass, gainedClassLevel, gainedSubclass);
                gainedCustomFeatures.AddRange(gainedSubclass.FeatureUnlocks
                    .Where(f => f.Level == gainedClassLevel)
                    .Select(f => f.FeatureDefinition as CustomFeatureDefinitionSet)
                    .Where(f => f != null)
                    .Select(f => (poolTag, f))
                );
            }
        }

        private void CollectTags()
        {
            UpdateGrantedFeatures();
            
            //TODO: sort features properly - make sure replacers are above their regular pool
            
            allPools.Clear();
            Dictionary<PoolId, FeaturePool> tags = new();
            foreach (var (poolTag, featureSet) in gainedCustomFeatures)
            {
                var poolId = new PoolId(featureSet.Name, poolTag);
                if(!tags.ContainsKey(poolId))
                {
                    var pool = new FeaturePool(poolId) {Max = 1, Used = 0, FeatureSet = featureSet};
                    tags.Add(poolId, pool);
                    allPools.Add(pool);
                }
                else
                {
                    tags[poolId].Max++;
                }

                if (featureSet is ReplaceCustomFeatureDefinitionSet removerSet)
                {
                    poolId = new PoolId(removerSet.ReplacedFeatureSet.Name, poolTag);
                    if(!tags.ContainsKey(poolId))
                    {
                        var pool = new FeaturePool(poolId) {Max = 0, Used = 0, FeatureSet = removerSet.ReplacedFeatureSet};
                        tags.Add(poolId, pool);
                        allPools.Add(pool);
                    }
                }
            }

            allPools.Sort(poolCompare);

            initialized = true;
        }

        private FeaturePool GetPoolById(PoolId id)
        {
            return allPools.FirstOrDefault(p => p.Id.Equals(id));
        }

        private void BuildLearnSteps()
        {
            // Register all steps
            if (this.allPools != null && this.allPools.Count > 0)
            {
                while (this.learnStepsTable.childCount < this.allPools.Count)
                {
                    Gui.GetPrefabFromPool(this.learnStepPrefab, this.learnStepsTable);
                }

                for (int i = 0; i < this.learnStepsTable.childCount; i++)
                {
                    Transform child = this.learnStepsTable.GetChild(i);

                    if (i < this.allPools.Count)
                    {
                        child.gameObject.SetActive(true);
                        LearnStepItem learnStepItem = child.GetComponent<LearnStepItem>();
                        learnStepItem.CustomBind(i, this.allPools[i], 
                            this.OnLearnBack, 
                            this.OnLearnReset,
                             this.OnLearnAuto
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
            Main.Log($"[ENDER] CancelStage");
            initialized = false;
            int stepNumber = this.currentLearnStep;
            if (this.IsFinalStep)
            {
                stepNumber--;
            }

            for (int i = stepNumber; i >= 0; i--)
            {
                this.ResetLearnings(i);
            }

            var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();
            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(this.OnCancelStageDone);
        }

        public void OnLearnBack()
        {
            Main.Log($"[ENDER] OnLearnBack");
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            this.MoveToPreviousLearnStep(true, this.ResetWasClickedFlag);
        }

        public void OnLearnReset()
        {
            Main.Log($"[ENDER] OnLearnReset");
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            if (this.IsFinalStep)
            {
                this.currentLearnStep = this.allPools.Count - 1;
            }

            this.ResetLearnings(this.currentLearnStep,
                () =>
                {
                    this.OnPreRefresh();
                    this.RefreshNow();
                    this.ResetWasClickedFlag();
                });
        }

        private void ResetLearnings(int stepNumber, Action onDone = null)
        {
            var pool = this.allPools[stepNumber];
            Main.Log($"[ENDER] ResetLearnings step: {stepNumber}, pool: [{pool.Id.Tag}] {pool.Id.Name}");
            pool.Used = 0;
            GetOrMakeLearnedList(pool.Id).Clear();

            GrantAcquiredFeatures(onDone);
        }

        #region UI helpers

        private void ResetWasClickedFlag()
        {
            wasClicked = false;
        }

        public void LevelSelected(int level)
        {
            this.StartCoroutine(this.BlendToLevelGroup(level));
        }

        private IEnumerator BlendToLevelGroup(int level)
        {
            float duration = ScrollDuration;
            SpellsByLevelGroup group = this.spellsByLevelTable.GetChild(0).GetComponent<SpellsByLevelGroup>();
            foreach (Transform child in this.spellsByLevelTable)
            {
                SpellsByLevelGroup spellByLevelGroup = child.GetComponent<SpellsByLevelGroup>();
                if (spellByLevelGroup.SpellLevel == level)
                {
                    group = spellByLevelGroup;
                }
            }

            float initialX = this.spellsByLevelTable.anchoredPosition.x;
            float finalX = -group.RectTransform.anchoredPosition.x + SpellsByLevelMargin;

            while (duration > 0)
            {
                this.spellsByLevelTable.anchoredPosition = new Vector2(
                    Mathf.Lerp(initialX, finalX, this.curve.Evaluate((ScrollDuration - duration) / ScrollDuration)), 0);
                duration -= Gui.SystemDeltaTime;
                yield return null;
            }

            this.spellsByLevelTable.anchoredPosition = new Vector2(finalX, 0);
        }

        #endregion


        #region autoselect stuff

        public override void AutotestAutoValidate() => this.OnLearnAuto();

        public void OnLearnAuto()
        {
            if (this.wasClicked)
            {
                return;
            }

            this.wasClicked = true;

            this.OnLearnAutoImpl();
        }

        private void OnLearnAutoImpl(System.Random rng = null)
        {
            //TODO: implement auto-selection of stuff
        }

        #endregion
    }

    internal static class LearnStepItemExtension
    {
        
        public static void CustomBind(this LearnStepItem instance, int rank,
            CustomFeatureSelectionPanel.FeaturePool pool,
            LearnStepItem.ButtonActivatedHandler onBackOneStepActivated,
            LearnStepItem.ButtonActivatedHandler onResetActivated,
            LearnStepItem.ButtonActivatedHandler onAutoSelectActivated)
        {
            instance.Tag = pool.Id.Tag;
            instance.PoolType = pool.IsReplacer
                ? HeroDefinitions.PointsPoolType.SpellUnlearn
                : HeroDefinitions.PointsPoolType.Irrelevant;
            instance.SetField("rank", rank);
            instance.SetField("ignoreAvailable", pool.IsReplacer);
            instance.SetField("autoLearnAvailable", false);
            string header = Gui.Localize(pool.FeatureSet.GuiPresentation.Title);//Gui.Localize($"CustomStage/&Step{pool.Tag}Title");
            instance.GetField<GuiLabel>("headerLabelActive").Text = header;
            instance.GetField<GuiLabel>("headerLabelInactive").Text = header;
            instance.OnBackOneStepActivated = onBackOneStepActivated;
            instance.OnResetActivated = onResetActivated;
            instance.OnAutoSelectActivated = onAutoSelectActivated;
        }

        public static void CustomRefresh(this LearnStepItem instance, LearnStepItem.Status status, CustomFeatureSelectionPanel.FeaturePool pool)
        {
            int usedPoints = pool.Used;
            int maxPoints = pool.Max;
            var ignoreAvailable = instance.GetField<bool>("ignoreAvailable");
            var choiceLabel = instance.GetField<GuiLabel>("choicesLabel");
            var activeGroup = instance.GetField<RectTransform>("activeGroup");
            var inactiveGroup = instance.GetField<RectTransform>("inactiveGroup");
            var autoButton = instance.GetField<Button>("autoButton");
            var resetButton = instance.GetField<Button>("resetButton");
            
            // Main.Log($"Step.CustomRefresh used:{usedPoints}, max:{maxPoints}, status: {status}");
            
            activeGroup.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
            inactiveGroup.gameObject.SetActive( status !=  LearnStepItem.Status.InProgress);
            instance.GetField<Image>("activeBackground").gameObject.SetActive(status != LearnStepItem.Status.Locked);
            instance.GetField<Image>("inactiveBackground").gameObject.SetActive(status == LearnStepItem.Status.Locked);
            instance.GetField<Button>("backOneStepButton").gameObject.SetActive(status == LearnStepItem.Status.Previous);
            resetButton.gameObject.SetActive(status == LearnStepItem.Status.InProgress);
            autoButton.gameObject.SetActive(status == LearnStepItem.Status.InProgress && !ignoreAvailable);
            instance.GetField<Button>("ignoreButton").gameObject.SetActive(ignoreAvailable);

            if (status == LearnStepItem.Status.InProgress)
            {
                instance.GetField<GuiLabel>("pointsLabelActive").Text = Gui.FormatCurrentOverMax(usedPoints, maxPoints);
                instance.GetField<Image>("remainingPointsGaugeActive").fillAmount = (float) usedPoints /  maxPoints;
                choiceLabel.Text = Gui.Localize(pool.FeatureSet.GuiPresentation.Description);////$"CustomStage/&Step{pool.Tag}Description";
                LayoutRebuilder.ForceRebuildLayoutImmediate(choiceLabel.RectTransform);
                activeGroup.sizeDelta = new Vector2(activeGroup.sizeDelta.x, (float) (choiceLabel.RectTransform.rect.height - choiceLabel.RectTransform.anchoredPosition.y + 12.0));
                instance.RectTransform.sizeDelta = activeGroup.sizeDelta;
                resetButton.interactable = usedPoints < maxPoints;
                autoButton.interactable = instance.GetField<bool>("autoLearnAvailable") && usedPoints > 0;
            }
            else
            {
                instance.GetField<GuiLabel>("pointsLabelInactive").Text = Gui.FormatCurrentOverMax(usedPoints, maxPoints);
                instance.GetField<Image>("remainingPointsGaugeInactive").fillAmount =  (float) usedPoints / maxPoints;
                instance.RectTransform.sizeDelta = inactiveGroup.sizeDelta;
                instance.GetField<Button>("backOneStepButton").interactable = true;
            }
        }
    }

    internal static class SpellLevelButtonExtension
    {
        public static void CustomBind(this SpellLevelButton instance,
            int level, SpellLevelButton.LevelSelectedHandler levelSelected)
        {
            instance.Level = level;
            instance.LevelSelected = levelSelected;
            instance.GetField<GuiLabel>("label").Text = $"{level}";
        }
    }

    internal static class SpellsByLevelGroupExtensions
    {
        public static RectTransform GetSpellsTable(this SpellsByLevelGroup instance)
        {
            return instance.GetField<RectTransform>("spellsTable");
        }

        public static void CustomFeatureBind(this SpellsByLevelGroup instance,
            CustomFeatureSelectionPanel.FeaturePool pool, List<FeatureDefinition> learned,
            int featureLevel, string lowLevelError,
            List<FeatureDefinition> unlearned,
            bool canAcquireFeatures,
            bool unlearn, SpellBox.SpellBoxChangedHandler spellBoxChanged)
        {
            var featureSet = pool.FeatureSet;
            instance.name = $"Feature[{featureSet.Name}]";
            instance.SpellLevel = featureLevel;

            // instance.CommonBind( null, unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning, spellBoxChanged, new List<SpellDefinition>(), null, new List<SpellDefinition>(), new List<SpellDefinition>(), "");
            var allFeatures = featureSet.GetLevelFeatures(featureLevel);
            // Main.Log($"[ENDER] CustomFeatureBind {featureSet.Name}, features: {allFeatures.Count}");
            //TODO: implement proper sorting
            //allSpells.Sort((IComparer<SpellDefinition>) instance);

            var spellsTable = instance.GetSpellsTable();
            var spellPrefab = instance.GetField<GameObject>("spellPrefab");

            while (spellsTable.childCount < allFeatures.Count)
            {
                Gui.GetPrefabFromPool(spellPrefab, spellsTable);
            }

            GridLayoutGroup component1 = spellsTable.GetComponent<GridLayoutGroup>();
            component1.constraintCount = Mathf.Max(3, Mathf.CeilToInt(allFeatures.Count / 4f));
            for (int index = 0; index < allFeatures.Count; ++index)
            {
                var feature = allFeatures[index];
                spellsTable.GetChild(index).gameObject.SetActive(true);
                var box = spellsTable.GetChild(index).GetComponent<SpellBox>();
                bool isUnlearned = unlearned.Contains(feature);
                SpellBox.BindMode bindMode = unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning;

                // box.Bind(guiSpellDefinition1, null, false, null, isUnlearned, bindMode, spellBoxChanged);
                box.CustomFeatureBind(feature, featureSet, isUnlearned, bindMode, spellBoxChanged);
            }

            //disable unneeded spell boxes
            for (int count = allFeatures.Count; count < spellsTable.childCount; ++count)
                spellsTable.GetChild(count).gameObject.SetActive(false);

            float x = (float)((double)component1.constraintCount * (double)component1.cellSize.x +
                              (double)(component1.constraintCount - 1) * (double)component1.spacing.x);
            spellsTable.sizeDelta = new Vector2(x, spellsTable.sizeDelta.y);
            instance.RectTransform.sizeDelta = new Vector2(x, instance.RectTransform.sizeDelta.y);

            instance.GetField<SlotStatusTable>("slotStatusTable")
                .Bind(null, featureLevel, null, false);

            if (unlearn)
            {
                instance.RefreshUnlearning(pool, lowLevelError, unlearned, canAcquireFeatures);
            }
            else
            {
                instance.RefreshLearning(pool, learned, lowLevelError, unlearned, canAcquireFeatures);
            }
        }
        
        public static void RefreshLearning(this SpellsByLevelGroup instance,
            CustomFeatureSelectionPanel.FeaturePool pool, List<FeatureDefinition> learned,
            string lowLevelError,
            List<FeatureDefinition> unlearnedFetures,
            bool canAcquireFeatures)
        {
            foreach (Transform transform in instance.GetSpellsTable())
            {
                if (transform.gameObject.activeSelf)
                {
                    SpellBox box = transform.GetComponent<SpellBox>();
                    var boxFeature = box.GetFeature();
                    var alreadyHas = Global.ActiveLevelUpHeroHasFeature(boxFeature);
                    bool selected = learned.Contains(boxFeature);
                    bool isUnlearned = unlearnedFetures != null && unlearnedFetures.Contains(boxFeature);
                    var errors = new List<string>();
                    bool canLearn =
                        !selected
                        && !alreadyHas
                        && (boxFeature is not IFeatureDefinitionWithPrerequisites prerequisites
                            || CustomFeaturesContext.GetValidationErrors(prerequisites.Validators, out errors));


                    if (lowLevelError != null)
                    {
                        errors.Add(lowLevelError);
                    }

                    box.SetupUI(pool.FeatureSet.GuiPresentation, errors);
                    
                    if (canAcquireFeatures)
                        box.CustomRefreshLearningInProgress((canLearn || selected) && !isUnlearned, selected, alreadyHas);
                    else
                        box.RefreshLearningInactive(selected && !isUnlearned);
                }
            }
        }
        
        public static void RefreshUnlearning(this SpellsByLevelGroup instance,
            CustomFeatureSelectionPanel.FeaturePool pool,
            string lowLevelError,
            List<FeatureDefinition> unlearnedSpells,
            bool canUnlearnSpells)
        {
            List<FeatureDefinition> knownFeatures = pool.FeatureSet.AllFeatures;
            foreach (Transform transform in instance.GetSpellsTable())
            {
                if (transform.gameObject.activeSelf)
                {
                    SpellBox box = transform.GetComponent<SpellBox>();
                    var boxFeature = box.GetFeature();
                    var removerFeature = boxFeature as FeatureDefinitionRemover;
                    bool isUnlearned = unlearnedSpells != null && unlearnedSpells.Contains(boxFeature);
                    bool canUnlearn =  Global.ActiveLevelUpHeroHasFeature(removerFeature != null
                                          ? removerFeature.FeatureToRemove
                                          : boxFeature) 
                                      && !isUnlearned;
                    
                    box.SetupUI(pool.FeatureSet.GuiPresentation);
                    
                    if (canUnlearnSpells)
                        box.RefreshUnlearnInProgress(canUnlearn || isUnlearned, isUnlearned);
                    else
                        box.RefreshUnlearnInactive(isUnlearned);
                }
            }
        }

        public static void CustomUnbind(this SpellsByLevelGroup instance)
        {
            instance.SpellRepertoire = null;
            var spellsTable = instance.GetSpellsTable();
            foreach (Component component in spellsTable)
                component.GetComponent<SpellBox>().CustomUnbind();
            Gui.ReleaseChildrenToPool(spellsTable);
            instance.GetField<SlotStatusTable>("slotStatusTable").Unbind(); //TODO: probably would need custom unbind  
        }
    }

    internal static class SpellBoxExtensions
    {
        private static readonly Dictionary<SpellBox, FeatureDefinition> Features = new();

        public static FeatureDefinition GetFeature(this SpellBox box)
        {
            return Features.GetValueOrDefault(box);
        }

        public static void CustomFeatureBind(this SpellBox instance, FeatureDefinition feature, CustomFeatureDefinitionSet setFeature, bool unlearned,
            SpellBox.BindMode bindMode, SpellBox.SpellBoxChangedHandler spellBoxChanged)
        {
            Features.AddOrReplace(instance, feature);

            //instance.GuiSpellDefinition = guiSpellDefinition;

            //instance.Caster = null; //TODO: do we need to set it to null?
            var image = instance.GetField<Image>("spellImage");

            instance.SetField("bindMode", bindMode);
            instance.SpellBoxChanged = spellBoxChanged;
            instance.SetField("hovered", false);
            instance.SetField("ritualSpell", false);
            instance.SetField("autoPrepared", false);
            instance.SetField("unlearnedSpell", unlearned);
            image.color = Color.white;
            instance.transform.localScale = new Vector3(1f, 1f, 1f);

            GuiModifier component =
                instance.GetField<RectTransform>("availableToLearnGroup").GetComponent<GuiModifier>();
            component.ForwardStartDelay = Random.Range(0.0f, component.Duration);

            instance.SetField("prepared", false);
            instance.SetField("canLearn", false);
            instance.SetField("selectedToLearn", false);
            instance.SetField("canPrepare", false);
            instance.SetField("known", false);
            instance.SetField("canUnlearn", false);

            instance.name = feature.Name;
        }

        public static void CustomRefreshLearningInProgress(this SpellBox instance, bool canLearn, bool selected, bool known)
        {
            var auroPrepard = instance.GetField<bool>("autoPrepared");
            instance.SetField("interactive", canLearn && !auroPrepard);
            instance.SetField("canLearn", canLearn && !auroPrepard);
            instance.SetField("selectedToLearn", selected);
            instance.SetField("selectedToLearn", selected);
            instance.SetField("known", known);//TODO: try to experimant with auto prepared tags to signify known features
            instance.InvokeMethod("Refresh");
        }

        public static void SetupSprite(Image imageComponent, GuiPresentation presentation)
        {
            if (imageComponent.sprite != null)
            {
                Gui.ReleaseAddressableAsset(imageComponent.sprite);
                imageComponent.sprite = null;
            }

            if (presentation.SpriteReference != null && presentation.SpriteReference.RuntimeKeyIsValid())
            {
                imageComponent.gameObject.SetActive(true);
                imageComponent.sprite = Gui.LoadAssetSync<Sprite>(presentation.SpriteReference);
            }
            else
                imageComponent.gameObject.SetActive(false);
        }

        public static void SetupUI(this SpellBox instance, GuiPresentation setPresentation, List<string> errors = null)
        {
            GuiLabel title = instance.GetField<GuiLabel>("titleLabel");
            Image image = instance.GetField<Image>("spellImage");
            GuiTooltip tooltip = instance.GetField<GuiTooltip>("tooltip");
            FeatureDefinition feature = instance.GetFeature();

            var gui = new GuiPresentationBuilder(feature.GuiPresentation).Build();
            var hasErrors = errors != null && !errors.Empty();

            if (!hasErrors && feature is FeatureDefinitionPower power)
            {
                var powerGui = ServiceRepository.GetService<IGuiWrapperService>().GetGuiPowerDefinition(power.Name);
                powerGui.SetupTooltip(tooltip);
            }
            else
            {
                tooltip.TooltipClass = "";
                tooltip.Content = feature.GuiPresentation.Description;
                tooltip.Context = null;
                tooltip.DataProvider = null;

                if (hasErrors)
                {
                    var error = String.Join("\n", errors.Select(e => Gui.Localize(e)));
                    tooltip.Content = $"{Gui.Localize(tooltip.Content)}\n\n{Gui.Colorize(error, Gui.ColorNegative)}";
                }
            }


            if (gui.SpriteReference == null || gui.SpriteReference == GuiPresentationBuilder.EmptySprite)
            {
                gui.SetSpriteReference(setPresentation.SpriteReference);
            }

            title.Text = gui.Title;
            SetupSprite(image, gui);
        }

        public static void CustomUnbind(this SpellBox instance)
        {
            Features.Remove(instance);
            instance.Unbind();
        }
    }
}
