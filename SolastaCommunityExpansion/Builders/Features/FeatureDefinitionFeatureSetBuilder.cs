using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder> : FeatureDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionFeatureSet
        where TBuilder : FeatureDefinitionFeatureSetBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionFeatureSetBuilder(TDefinition original) : base(original) { }

        protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder ClearFeatureSet()
        {
            Definition.ClearFeatureSet();
            return (TBuilder)this;
        }

        public TBuilder SetFeatureSet(params FeatureDefinition[] featureDefinitions)
        {
            return SetFeatureSet(featureDefinitions.AsEnumerable());
        }

        public TBuilder SetFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            Definition.SetFeatureSet(featureDefinitions);
            return (TBuilder)this;
        }

        public TBuilder AddFeatureSet(params FeatureDefinition[] featureDefinitions)
        {
            return AddFeatureSet(featureDefinitions.AsEnumerable());
        }

        public TBuilder AddFeatureSet(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            Definition.AddFeatureSet(featureDefinitions);
            return (TBuilder)this;
        }

        public TBuilder SetEnumerateInDescription(bool value)
        {
            Definition.SetEnumerateInDescription(value);
            return (TBuilder)this;
        }

        public TBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
        {
            Definition.SetMode(mode);
            return (TBuilder)this;
        }

        public TBuilder SetUniqueChoices(bool uniqueChoice)
        {
            Definition.SetUniqueChoices(uniqueChoice);
            return (TBuilder)this;
        }
    }

    public class FeatureDefinitionFeatureSetBuilder : FeatureDefinitionFeatureSetBuilder<FeatureDefinitionFeatureSet, FeatureDefinitionFeatureSetBuilder>
    {
        protected FeatureDefinitionFeatureSetBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
    }
}
