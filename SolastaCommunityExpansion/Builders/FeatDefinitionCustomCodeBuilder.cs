using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class FeatDefinitionCustomCode : FeatDefinition
    {
        public List<Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>> Validators { get; }

        public abstract (bool result, string output) Validate(FeatDefinition feat, RulesetCharacterHero hero);
    }

    public class FeatDefinitionCustomCodeBuilder : FeatDefinitionBuilder<FeatDefinitionCustomCode, FeatDefinitionCustomCodeBuilder>
    {
        #region Constructors
        protected FeatDefinitionCustomCodeBuilder(FeatDefinitionCustomCode original) : base(original)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(FeatDefinitionCustomCode original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(FeatDefinitionCustomCode original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatDefinitionCustomCodeBuilder(FeatDefinitionCustomCode original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatDefinitionCustomCodeBuilder SetValidators(params Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>[] validators)
        {
            Definition.Validators.AddRange(validators);

            return this;
        }
    }
}
