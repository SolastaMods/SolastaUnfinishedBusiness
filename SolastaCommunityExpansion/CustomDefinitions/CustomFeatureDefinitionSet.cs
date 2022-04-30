using System;
using System.Collections.Generic;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class CustomFeatureDefinitionSet : FeatureDefinition
    {
        private bool _fullSetIsDirty;
        private readonly List<FeatureDefinition> _allFeatureSet = new();
        private Dictionary<int, List<FeatureDefinition>> FeaturesByLevel  = new();
        /**Are level requirements in character levels or class levels?*/
        public bool RequireClassLevels { get; set; }
        public List<int> AllLevels => FeaturesByLevel.Select(e=>e.Key).ToList();

        public List<FeatureDefinition> AllFeatures
        {
            get
            {
                if (_fullSetIsDirty)
                {
                    _allFeatureSet.SetRange(FeaturesByLevel.SelectMany(e => e.Value));
                    _fullSetIsDirty = false;
                }

                return _allFeatureSet;
            }
        }

        private List<FeatureDefinition> GetOrMakeLevelFeatures(int level)
        {
            List<FeatureDefinition> levelFeatures;
            if (!FeaturesByLevel.ContainsKey(level))
            {
                levelFeatures = new List<FeatureDefinition>();
                FeaturesByLevel.Add(level, levelFeatures);
            }
            else
            {
                levelFeatures = FeaturesByLevel[level];
            }

            return levelFeatures;
        }

        public void AddLevelFeatures(int level, params FeatureDefinition[] features)
        {
            GetOrMakeLevelFeatures(level).AddRange(features);
            _fullSetIsDirty = true;
        }
        
        public void AddLevelFeatures(int level, List<FeatureDefinition> features)
        {
            GetOrMakeLevelFeatures(level).AddRange(features);
            _fullSetIsDirty = true;
        }
        
        public void SetLevelFeatures(int level, params FeatureDefinition[] features)
        {
            GetOrMakeLevelFeatures(level).SetRange(features);
            _fullSetIsDirty = true;
        }
        
        public void SetLevelFeatures(int level, List<FeatureDefinition> features)
        {
            GetOrMakeLevelFeatures(level).SetRange(features);
            _fullSetIsDirty = true;
        }

        public List<FeatureDefinition> GetLevelFeatures(int level)
        {
            //TODO: decide if we want to wrap this into new list, to be sure htis one is immutable
            return FeaturesByLevel.GetValueOrDefault(level);
        }

    }

    public class CustomFeatureDefinitionSetBuilder : FeatureDefinitionBuilder<CustomFeatureDefinitionSet,
        CustomFeatureDefinitionSetBuilder>
    {
        public CustomFeatureDefinitionSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public CustomFeatureDefinitionSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public CustomFeatureDefinitionSetBuilder(CustomFeatureDefinitionSet original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public CustomFeatureDefinitionSetBuilder(CustomFeatureDefinitionSet original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        public CustomFeatureDefinitionSetBuilder AddLevelFeatures(int level, params FeatureDefinition[] features)
        {
            Definition.AddLevelFeatures(level, features);
            return this;
        }

        public CustomFeatureDefinitionSetBuilder AddLevelFeatures(int level, List<FeatureDefinition> features)
        {
            Definition.AddLevelFeatures(level, features);
            return this;
        }
        
        public CustomFeatureDefinitionSetBuilder SetLevelFeatures(int level, params FeatureDefinition[] features)
        {
            Definition.SetLevelFeatures(level, features);
            return this;
        }

        public CustomFeatureDefinitionSetBuilder SetLevelFeatures(int level, List<FeatureDefinition> features)
        {
            Definition.SetLevelFeatures(level, features);
            return this;
        }

        public CustomFeatureDefinitionSetBuilder SetRequireClassLevels(bool value)
        {
            Definition.RequireClassLevels = value;
            return this;
        }
    }

    public class FeatureDefinitionRemover : FeatureDefinition, IFeatureDefinitionCustomCode
    {
        public FeatureDefinition FeatureToRemove { get; set; }

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            CustomFeaturesContext.RecursiveRemoveFeature(hero, FeatureToRemove);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            //technically we return feature not where we took it from
            ServiceRepository.GetService<ICharacterBuildingService>()
                .GrantFeatures(hero, new List<FeatureDefinition> {FeatureToRemove}, tag);
        }
    }

    public class FeatureDefinitionRemoverBuilder 
        : FeatureDefinitionBuilder<FeatureDefinitionRemover, FeatureDefinitionRemoverBuilder>
    {
        #region Constructors

        public FeatureDefinitionRemoverBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionRemoverBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, Guid namespaceGuid) : base(
            original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionRemoverBuilder(FeatureDefinitionRemover original, string name, string definitionGuid) :
            base(original, name, definitionGuid)
        {
        }

        #endregion

        public static FeatureDefinitionRemoverBuilder CreateFrom(FeatureDefinition feature)
        {
            return Create($"{feature.Name}Remover", CENamespaceGuid)
                .SetGuiPresentation(feature.GuiPresentation)//TODO: add 'removed' to the title in the character info screen
                .SetFeatureToRemove(feature);
        }

        public FeatureDefinitionRemoverBuilder SetFeatureToRemove(FeatureDefinition feature)
        {
            Definition.FeatureToRemove = feature;
            return this;
        }
    }
}
