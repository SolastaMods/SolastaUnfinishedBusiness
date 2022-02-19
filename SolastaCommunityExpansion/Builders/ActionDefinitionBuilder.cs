using System;

namespace SolastaCommunityExpansion.Builders
{
    public class ActionDefinitionBuilder : DefinitionBuilder<ActionDefinition, ActionDefinitionBuilder>
    {
        #region Constructors
        protected ActionDefinitionBuilder(ActionDefinition original) : base(original)
        {
        }

        protected ActionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected ActionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected ActionDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected ActionDefinitionBuilder(ActionDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected ActionDefinitionBuilder(ActionDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected ActionDefinitionBuilder(ActionDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        // TODO: add Create methods
    }

}
