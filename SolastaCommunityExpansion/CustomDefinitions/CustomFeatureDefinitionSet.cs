using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public class CustomFeatureDefinitionSet : FeatureDefinition, IFeatureDefinitionCustomCode//TODO: do we actually need custom code?
    {
        public List<FeatureDefinition> FeatureSet { get; set; } = new();

        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            //TODO: do nothing, since we will actually grant features in the levelup screen
            // ServiceRepository.GetService<ICharacterBuildingService>()
            //     .GrantFeatures(hero, FeatureSet, tag);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            //TODO: do nothing, since we will actually remove features in the levelup screen
            // CustomFeaturesContext.RecursiveRemoveFeatures(hero, FeatureSet, tag);
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

        public CustomFeatureDefinitionSetBuilder SetFeatureSet(params FeatureDefinition[] features)
        {
            Definition.FeatureSet.SetRange(features);
            return this;
        }
        
        public CustomFeatureDefinitionSetBuilder SetFeatureSet(IEnumerable<FeatureDefinition> features)
        {
            Definition.FeatureSet.SetRange(features);
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
