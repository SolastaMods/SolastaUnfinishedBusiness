using System;

namespace SolastaCommunityExpansion.Builders
{
    public delegate bool IsFeatMacthingPrerequisites(
          FeatDefinition feat,
          RulesetCharacterHero hero,
          ref string prerequisiteOutput);

    public class FeatDefinitionWithPrereqs : FeatDefinition
    {
        public IsFeatMacthingPrerequisites IsFeatMacthingPrerequisites;
    }

    public class FeatDefinitionWithPrereqsBuilder : FeatDefinitionBuilder<FeatDefinitionWithPrereqs, FeatDefinitionWithPrereqsBuilder>
    {
        #region Constructors
        protected FeatDefinitionWithPrereqsBuilder(FeatDefinitionWithPrereqs original) : base(original)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(FeatDefinitionWithPrereqs original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(FeatDefinitionWithPrereqs original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatDefinitionWithPrereqsBuilder(FeatDefinitionWithPrereqs original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatDefinitionWithPrereqsBuilder SetValidationDelegate(IsFeatMacthingPrerequisites isFeatMacthingPrerequisites)
        {
            Definition.IsFeatMacthingPrerequisites = isFeatMacthingPrerequisites;

            return this;
        }
    }
}
