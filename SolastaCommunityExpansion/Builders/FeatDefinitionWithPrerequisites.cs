using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Builders
{
    public class FeatDefinitionWithPrerequisites : FeatDefinition
    {
        public readonly List<Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>> Validators = new();

        public (bool result, string output) Validate(FeatDefinition feat, RulesetCharacterHero hero)
        {
            var results = Validators.Select(v => v(feat, hero));

            if (results.Any())
            {
                return (results.All(r => r.result), string.Join("\n", results.Select(r => r.output)));
            }

            return (true, string.Empty);
        }
    }

    public class FeatDefinitionWithPrerequisitesBuilder : FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
    {
        #region Constructors
        protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original) : base(original)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatDefinitionWithPrerequisitesBuilder SetValidators(params Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>[] validations)
        {
            Definition.Validators.AddRange(validations);

            return this;
        }
    }
}
