using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionFeatureSetBuilder : BaseDefinitionBuilder<FeatureDefinitionFeatureSet>
    {
        public FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, string guid, GuiPresentation guiPresentation)
            : base(original, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionFeatureSetBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionFeatureSetBuilder ClearFeatures()
        {
            Definition.FeatureSet.Clear();
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder AddFeature(FeatureDefinition featureDefinition)
        {
            Definition.FeatureSet.Add(featureDefinition);
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder SetFeatures(params FeatureDefinition[] featureDefinitions)
        {
            return SetFeatures(featureDefinitions.AsEnumerable());
        }

        public FeatureDefinitionFeatureSetBuilder SetFeatures(IEnumerable<FeatureDefinition> featureDefinitions)
        {
            Definition.FeatureSet.SetRange(featureDefinitions);
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
        {
            Definition.SetMode(mode);
            return this;
        }

        public FeatureDefinitionFeatureSetBuilder SetUniqueChoices(bool uniqueChoice)
        {
            Definition.SetUniqueChoices(uniqueChoice);
            return this;
        }
    }
}
