using System;

namespace SolastaCommunityExpansion.Builders
{
    public class RestActivityDefinitionBuilder : DefinitionBuilder<RestActivityDefinition, RestActivityDefinitionBuilder>
    {
        #region Constructors
        protected RestActivityDefinitionBuilder(RestActivityDefinition original) : base(original)
        {
        }

        protected RestActivityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected RestActivityDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected RestActivityDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // TODO: add Create methods
    }
}
