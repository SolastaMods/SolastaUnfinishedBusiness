using System;
using SolastaModApi.Extensions;

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

        #region Create
        public static ActionDefinitionBuilder Create(ActionDefinition original)
        {
            return new ActionDefinitionBuilder(original);
        }

        public static ActionDefinitionBuilder Create(string name, Guid namespaceGuid) 
        {
            return new ActionDefinitionBuilder(name, namespaceGuid);
        }

        public static ActionDefinitionBuilder Create(string name, string definitionGuid) 
        {
            return new ActionDefinitionBuilder(name, definitionGuid);
        }

        public static ActionDefinitionBuilder Create(string name, bool createGuiPresentation = true) 
        {
            return new ActionDefinitionBuilder(name, createGuiPresentation);
        }

        public static ActionDefinitionBuilder Create(ActionDefinition original, string name, bool createGuiPresentation = true) 
        {
            return new ActionDefinitionBuilder(original, name, createGuiPresentation);
        }

        public static ActionDefinitionBuilder Create(ActionDefinition original, string name, Guid namespaceGuid)
        {
            return new ActionDefinitionBuilder(original, name, namespaceGuid);
        }

        public static ActionDefinitionBuilder Create(ActionDefinition original, string name, string definitionGuid)
        {
            return new ActionDefinitionBuilder(original, name, definitionGuid);
        }
        #endregion

        public ActionDefinitionBuilder SetId(ActionDefinitions.Id value)
        {
            Definition.SetId(value);
            return this;
        }
    }
}
