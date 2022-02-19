using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionProficiencyBuilder : FeatureDefinitionBuilder<FeatureDefinitionProficiency, FeatureDefinitionProficiencyBuilder>
    {
        #region Constructors
        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original) : base(original)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public static FeatureDefinitionProficiencyBuilder Create(FeatureDefinitionProficiency original, string name, string guid)
        {
            return new FeatureDefinitionProficiencyBuilder(original, name, guid);
        }

        public static FeatureDefinitionProficiencyBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionProficiencyBuilder(name, namespaceGuid);
        }

        public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type, params string[] proficiencies)
        {
            return SetProficiencies(type, proficiencies.AsEnumerable());
        }

        public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type, IEnumerable<string> proficiencies)
        {
            Definition.SetProficiencyType(type);
            Definition.SetProficiencies(proficiencies);
            return this;
        }
    }
}
