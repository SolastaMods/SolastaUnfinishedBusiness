using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionProficiencyBuilder : FeatureDefinitionBuilder<FeatureDefinitionProficiency,
        FeatureDefinitionProficiencyBuilder>
    {
        public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type,
            params string[] proficiencies)
        {
            return SetProficiencies(type, proficiencies.AsEnumerable());
        }

        public FeatureDefinitionProficiencyBuilder SetProficiencies(RuleDefinitions.ProficiencyType type,
            IEnumerable<string> proficiencies)
        {
            Definition.SetProficiencyType(type);
            Definition.SetProficiencies(proficiencies);
            Definition.Proficiencies.Sort();

            return this;
        }

        #region Constructors

        protected FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
