using System;

namespace SolastaCommunityExpansion.Builders
{
    public class LanguageDefinitionBuilder : DefinitionBuilder<LanguageDefinition, LanguageDefinitionBuilder>
    {
        #region Constructors
        protected LanguageDefinitionBuilder(LanguageDefinition original) : base(original)
        {
        }

        protected LanguageDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected LanguageDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected LanguageDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected LanguageDefinitionBuilder(LanguageDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected LanguageDefinitionBuilder(LanguageDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected LanguageDefinitionBuilder(LanguageDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
