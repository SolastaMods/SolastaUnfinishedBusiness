using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
/*        private FeatureDefinitionProficiencyBuilder(string name, string guid)
            : base(name, guid)
        {
        }
        private FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }
*/
        private FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, string guid)
            : base(original, name, guid)
        {
        }

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
            Definition.Proficiencies.SetRange(proficiencies);
            return this;
        }
    }
}
