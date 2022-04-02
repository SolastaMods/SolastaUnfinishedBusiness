using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionSubclassChoiceBuilder
        : FeatureDefinitionBuilder<FeatureDefinitionSubclassChoice, FeatureDefinitionSubclassChoiceBuilder>
    {
        #region Constructors
        protected FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original) : base(original)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionSubclassChoiceBuilder(FeatureDefinitionSubclassChoice original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionSubclassChoiceBuilder SetFilterByDeity(bool requireDeity)
        {
            Definition.SetFilterByDeity(requireDeity);
            return this;
        }

        public FeatureDefinitionSubclassChoiceBuilder SetSubclassSuffix(string subclassSuffix)
        {
            Definition.SetSubclassSuffix(subclassSuffix);
            return this;
        }

        public FeatureDefinitionSubclassChoiceBuilder SetSubclasses(params CharacterSubclassDefinition[] subclasses)
        {
            return SetSubclasses(subclasses.AsEnumerable());
        }

        public FeatureDefinitionSubclassChoiceBuilder SetSubclasses(IEnumerable<CharacterSubclassDefinition> subclasses)
        {
            Definition.Subclasses.SetRange(subclasses.Select(sc => sc.Name));
            return this;
        }
    }
}
