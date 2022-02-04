using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        // TODO: remove these ctors
        public FeatureDefinitionProficiencyBuilder(string name, string guid, RuleDefinitions.ProficiencyType type,
            IEnumerable<string> proficiencies, GuiPresentation guiPresentation) : base(name, guid, guiPresentation)
        {
            Definition.SetProficiencyType(type);
            Definition.Proficiencies.AddRange(proficiencies);
        }

        public FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid,
            RuleDefinitions.ProficiencyType type, IEnumerable<string> proficiencies, Category category) : base(name, namespaceGuid, category)
        {
            Definition.SetProficiencyType(type);
            Definition.Proficiencies.AddRange(proficiencies);
        }

        public FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid,
            RuleDefinitions.ProficiencyType type, IEnumerable<string> proficiencies) : this(name, namespaceGuid, type, proficiencies, Category.None)
        {
            Definition.SetProficiencyType(type);
            Definition.Proficiencies.AddRange(proficiencies);
        }

        public FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid,
            RuleDefinitions.ProficiencyType type, params string[] proficiencies) : this(name, namespaceGuid, type, proficiencies.AsEnumerable())
        {
        }
        //-- to here

        public FeatureDefinitionProficiencyBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionProficiencyBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionProficiencyBuilder(FeatureDefinitionProficiency original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionProficiencyBuilder CreateCopyFrom(FeatureDefinitionProficiency original, string name, string guid)
        {
            return new FeatureDefinitionProficiencyBuilder(original, name, guid);
        }

        public FeatureDefinitionProficiencyBuilder AddProficiencies(RuleDefinitions.ProficiencyType type, params string[] proficiencies)
        {
            return AddProficiencies(type, proficiencies.AsEnumerable());
        }

        public FeatureDefinitionProficiencyBuilder AddProficiencies(RuleDefinitions.ProficiencyType type, IEnumerable<string> proficiencies)
        {
            Definition.SetProficiencyType(type);
            Definition.Proficiencies.AddRange(proficiencies);
            return this;
        }
    }
}
